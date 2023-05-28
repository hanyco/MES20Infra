using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Services;

using Library.CodeGeneration.Models;
using Library.Exceptions;
using Library.Interfaces;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;

using Microsoft.EntityFrameworkCore.Storage;

namespace Services;

internal partial class FunctionalityService : IFunctionalityService, IFunctionalityCodeService
    , IBusinessService, IAsyncValidator<FunctionalityViewModel>, IAsyncTransactionSave, ILoggerContainer
{
    #region Fields & Properties

    private readonly ICqrsCommandService _commandService;
    private readonly IEntityViewModelConverter _converter;
    private readonly ICqrsCodeGeneratorService _cqrsCodeService;
    private readonly IDtoCodeService _dtoCodeService;
    private readonly IDtoService _dtoService;
    private readonly IModuleService _moduleService;
    private readonly ICqrsQueryService _queryService;
    private readonly InfraReadDbContext _readDbContext;
    private readonly IMultistepProcess _reporter;
    private readonly InfraWriteDbContext _writeDbContext;
    public ILogger Logger { get; }

    #endregion Fields & Properties

    #region Ctors

    public FunctionalityService(
    InfraReadDbContext readDbContext,
    InfraWriteDbContext writeDbContext,

    IEntityViewModelConverter converter,
    IDtoService dtoService,
    IDtoCodeService dtoCodeService,
    ICqrsQueryService queryService,
    ICqrsCommandService commandService,
    ICqrsCodeGeneratorService cqrsCodeService,

    IModuleService moduleService,
    IMultistepProcess reporter,
    ILogger logger)
    {
        (this._readDbContext, this._writeDbContext) = (readDbContext, writeDbContext);
        this._converter = converter;
        this.Logger = logger;
        this._dtoService = dtoService;
        this._dtoCodeService = dtoCodeService;
        this._queryService = queryService;
        this._commandService = commandService;
        this._cqrsCodeService = cqrsCodeService;
        this._moduleService = moduleService;
        this._reporter = reporter;
    }

    #endregion Ctors

    Task<IDbContextTransaction> IAsyncTransactional.BeginTransactionAsync(CancellationToken cancellationToken)
        => this._writeDbContext.BeginTransactionAsync(cancellationToken);

    Task<Result> IAsyncTransactional.CommitTransactionAsync(CancellationToken cancellationToken)
        => this._writeDbContext.CommitTransactionAsync(cancellationToken);

    public void ResetChanges()
        => this._writeDbContext.ResetChanges();

    Task IAsyncTransactional.RollbackTransactionAsync(CancellationToken cancellationToken)
        => this._writeDbContext.Database.RollbackTransactionAsync(cancellationToken);

    public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken)
        => this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    Task<Result<FunctionalityViewModel>> IAsyncValidator<FunctionalityViewModel>.ValidateAsync(FunctionalityViewModel viewModel, CancellationToken cancellationToken)
        => viewModel.Check()
            .ArgumentNotNull()
            .NotNull(x => x.Name)
            .NotNull(x => x.NameSpace)
            .Build().ToAsync();

    public async Task<Result<Codes>> GenerateCodesAsync(FunctionalityViewModel viewModel, GenerateCodesParameters? arguments = null, CancellationToken token = default)
    {
        var result = Codes.New();

        var getAllQueryCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.GetAllQuery, token: token);
        if (!getAllQueryCodes)
        {
            return Result<Codes>.From(getAllQueryCodes, result);
        }
        if (token.IsCancellationRequested)
        {
            return Result<Codes>.CreateFailure(new OperationCancelException(), result)!;
        }

        viewModel.Codes.GetAllQueryCodes = getAllQueryCodes;


        return new(result);
    }
}