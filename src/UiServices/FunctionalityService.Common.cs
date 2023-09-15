using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.Services;

using Library.Exceptions.Validations;
using Library.Interfaces;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;

using Microsoft.EntityFrameworkCore.Storage;

namespace Services;

[Service]
internal partial class FunctionalityService(
    InfraReadDbContext readDbContext,
    InfraWriteDbContext writeDbContext,
    IEntityViewModelConverter converter,
    IDtoService dtoService,
    IDtoCodeService dtoCodeService,
    ICqrsQueryService queryService,
    ICqrsCommandService commandService,
    ICqrsCodeGeneratorService cqrsCodeService,
    IModuleService moduleService,
    IProgressReport reporter,
    ILogger logger,
    IBlazorComponentService blazorComponentService,
    IBlazorComponentCodingService blazorComponentCodeService,
    IBlazorPageService blazorPageService,
    IBlazorPageCodingService blazorPageCodeService,
    IConverterCodeGeneratorService converterCodeService)
    : IFunctionalityService
    , IFunctionalityCodeService
    , IBusinessService
    , IValidator<FunctionalityViewModel>
    , IAsyncTransactionSave
    , ILoggerContainer
{
    #region Fields & Properties

    private readonly IBlazorComponentCodingService _blazorComponentCodeService = blazorComponentCodeService;
    private readonly IBlazorComponentService _blazorComponentService = blazorComponentService;
    private readonly IBlazorPageCodingService _blazorPageCodeService = blazorPageCodeService;
    private readonly IBlazorPageService _blazorPageService = blazorPageService;
    private readonly ICqrsCommandService _commandService = commandService;
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly IConverterCodeGeneratorService _converterCodeService = converterCodeService;
    private readonly ICqrsCodeGeneratorService _cqrsCodeService = cqrsCodeService;
    private readonly IDtoCodeService _dtoCodeService = dtoCodeService;
    private readonly IDtoService _dtoService = dtoService;
    private readonly IModuleService _moduleService = moduleService;
    private readonly ICqrsQueryService _queryService = queryService;
    private readonly InfraReadDbContext _readDbContext = readDbContext;
    private readonly IProgressReport _reporter = reporter;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;
    public ILogger Logger { get; } = logger;

    #endregion Fields & Properties

    Task<IDbContextTransaction> IAsyncTransactional.BeginTransactionAsync(CancellationToken cancellationToken) =>
        this._writeDbContext.BeginTransactionAsync(cancellationToken);

    Task<Result> IAsyncTransactional.CommitTransactionAsync(CancellationToken cancellationToken) =>
        this._writeDbContext.CommitTransactionAsync(cancellationToken);

    public async Task<FunctionalityViewModel> CreateAsync(CancellationToken token = default)
        => new FunctionalityViewModel
        {
            SourceDto = await this._dtoService.CreateAsync(token)
        };

    public void ResetChanges() =>
        this._writeDbContext.ResetChanges();

    Task IAsyncTransactional.RollbackTransactionAsync(CancellationToken cancellationToken) =>
        this._writeDbContext.Database.RollbackTransactionAsync(cancellationToken);

    public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken) =>
        this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    public Result<FunctionalityViewModel> Validate(in FunctionalityViewModel item) =>
        BasicChecks(item);

    private static ValidationResultSet<FunctionalityViewModel> BasicChecks(FunctionalityViewModel? model) =>
            model.Check()
             //x.RuleFor(_ => !cancellationToken.IsCancellationRequested, () => new OperationCancelException("Cancelled by parent"))
             .ArgumentNotNull()
             .NotNull(x => x!.Name)
             .NotNull(x => x!.SourceDto)
             .NotNull(x => x!.SourceDto.NameSpace, paramName: "namespace")
             .RuleFor(x => x!.SourceDto.Module?.Id > 0, () => new NullValueValidationException(nameof(model.SourceDto.Module)))!;
}