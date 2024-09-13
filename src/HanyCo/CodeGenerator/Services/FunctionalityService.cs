using HanyCo.Infra.CodeGen.Contracts.CodeGen.Services;
using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;

using Library.CodeGeneration.v2;
using Library.DesignPatterns.Markers;
using Library.Exceptions.Validations;
using Library.Interfaces;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;

using Microsoft.EntityFrameworkCore.Storage;

namespace Services;

[Service]
[Stateless]
internal sealed partial class FunctionalityService(
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
    IBlazorComponentCodeService blazorComponentCodeService,
    IBlazorPageService blazorPageService,
    IBlazorPageCodeService blazorPageCodeService,
    IMapperSourceGenerator mapperSourceGenerator,
    IApiCodingService apiCodeGenerator,
    ICodeGeneratorEngine generatorEngine)
    : IFunctionalityService
    , IFunctionalityCodeService
    , IValidator<FunctionalityViewModel>
    , IAsyncTransactionalSave
    , ILoggerContainer
{
    private readonly IApiCodingService _apiCodeGenerator = apiCodeGenerator;
    private readonly IBlazorComponentCodeService _blazorComponentCodeService = blazorComponentCodeService;
    private readonly IBlazorComponentService _blazorComponentService = blazorComponentService;
    private readonly IBlazorPageCodeService _blazorPageCodeService = blazorPageCodeService;
    private readonly IBlazorPageService _blazorPageService = blazorPageService;
    private readonly ICqrsCommandService _commandService = commandService;
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly ICqrsCodeGeneratorService _cqrsCodeService = cqrsCodeService;
    private readonly IDtoCodeService _dtoCodeService = dtoCodeService;
    private readonly IDtoService _dtoService = dtoService;
    private readonly ICodeGeneratorEngine _generatorEngine = generatorEngine;
    private readonly IMapperSourceGenerator _mapperSourceGenerator = mapperSourceGenerator;
    private readonly IModuleService _moduleService = moduleService;
    private readonly ICqrsQueryService _queryService = queryService;
    private readonly InfraReadDbContext _readDbContext = readDbContext;
    private readonly IProgressReport _reporter = reporter;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;
    public ILogger Logger { get; } = logger;

    Task<IDbContextTransaction> IAsyncTransactional.BeginTransactionAsync(CancellationToken cancellationToken) =>
        this._writeDbContext.BeginTransactionAsync(cancellationToken);

    Task<Result> IAsyncTransactional.CommitTransactionAsync(CancellationToken cancellationToken) =>
        this._writeDbContext.CommitTransactionAsync(cancellationToken);

    public async Task<FunctionalityViewModel> CreateAsync(CancellationToken token = default) =>
        new FunctionalityViewModel
        {
            SourceDto = await this._dtoService.CreateAsync(token)
        };

    public void ResetChanges() =>
        this._writeDbContext.ResetChanges();

    Task IAsyncTransactional.RollbackTransactionAsync(CancellationToken cancellationToken) =>
        this._writeDbContext.Database.RollbackTransactionAsync(cancellationToken);

    public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken) =>
        this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    public Result<FunctionalityViewModel?> Validate(in FunctionalityViewModel? item) =>
        BasicChecks(item);

    private static ValidationResultSet<FunctionalityViewModel?> BasicChecks(FunctionalityViewModel? model) =>
        model.Check()
            .ArgumentNotNull()
            .NotNull(x => x!.Name)
            .NotNull(x => x!.SourceDto)
            .NotNull(x => x!.SourceDto.Name)
            .NotNull(x => x!.SourceDto.NameSpace, paramName: "namespace")
            .RuleFor(x => x!.SourceDto.Module?.Id is null or > 0, () => new NullValueValidationException(nameof(model.SourceDto.Module)));
}