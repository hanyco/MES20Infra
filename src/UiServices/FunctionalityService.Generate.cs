using System.Diagnostics.CodeAnalysis;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Data.SqlServer.Dynamics;
using Library.DesignPatterns.Markers;
using Library.Exceptions;
using Library.Exceptions.Validations;
using Library.Interfaces;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

using Database = Library.Data.SqlServer.Dynamics.Database;

namespace Services;

[Service]
internal sealed partial class FunctionalityService : IFunctionalityService, IFunctionalityCodeService
    , IBusinesService, IAsyncValidator<FunctionalityViewModel>, IAsyncTransactionSaveService, ILoggerContainer
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

    public async Task<Result<FunctionalityViewModel?>> GenerateAsync(FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        //! Validation Checks
        Check.IfArgumentNotNull(viewModel);
        if (!validate(viewModel, token).TryParse(out var validationChecks))
        {
            return validationChecks!;
        }

        //! Initialize
        this._reporter.Report(description: getTitle("Initializing..."));
        // If `viewModel.DbTable` is empty then the connection string: `this._readDbContext.Database.GetConnectionString()` will be used to fill `viewModel.DbTable`.
        // Otherwise, `viewModel.DbTable` will be directly used (to be used in Unit Test).
        var connectionString = viewModel.DbTable is not null ? null : this._readDbContext.Database.GetConnectionString();
        var initResult = await initialize(viewModel, connectionString, token);

        if (!initResult.IsSucceed)
        {
            return Result<FunctionalityViewModel>.From(initResult, default!)!;
        }
        var (data, tokenSource) = initResult.Value;
        var process = initSteps(data);

        //! Process
        this._reporter.Report(description: getTitle("Running..."));
        var processResult = await process.RunAsync(tokenSource.Token);

        //! Finalize
        var message = finalize(processResult);
        tokenSource.Dispose();
        this._reporter.Report(description: getTitle(message));
        var result = processResult.Result;

        return result!;

        #region Local Methods

        ProgressData getTitle(in string description)
            => new(Description: description, Sender: nameof(FunctionalityService));

        static Result<FunctionalityViewModel?> validate(in FunctionalityViewModel model, CancellationToken token)
            => model.Check()
                    .RuleFor(_ => !token.IsCancellationRequested, () => new OperationCancelException("Cancelled by parent"))
                    .ArgumentNotNull()
                    .NotNull(x => x.Name)
                    .NotNull(x => x.NameSpace)
                    .NotNull(x => x.DbObject)
                    .NotNull(x => x.DbObject.Name)
                    .RuleFor(x => x.ModuleId != 0, () => new ValidationException("Module is not selected."))
                    .Build();

        static async Task<Result<(CreationData Data, CancellationTokenSource TokenSource)>> initialize(FunctionalityViewModel viewModel, string? connectionString, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return Result<(CreationData Data, CancellationTokenSource TokenSource)>.CreateFailure("Cancelled by parent", default);
            }

            var dataResult = viewModel;
            Table dbTable;
            if (viewModel.DbTable is not null)
            {
                dbTable = viewModel.DbTable;
            }
            else
            {
                var db = await Database.GetDatabaseAsync(connectionString!);
                dbTable = db.NotNull(() => new ObjectNotFoundException("Database not found."))
                            .Tables[dataResult.DbObject.Name!].NotNull(() => new ObjectNotFoundException($"Table name `{dataResult.DbObject}` not found."));
            }
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            var result = new CreationData(dataResult, dbTable, cancellationTokenSource);
            return Result<(CreationData Data, CancellationTokenSource TokenSource)>.CreateSuccess((result, cancellationTokenSource));
        }

        MultistepProcessRunner<CreationData> initSteps(in CreationData data)
            => MultistepProcessRunner<CreationData>.New(data, this._reporter, owner: nameof(FunctionalityService))
                .AddStep(this.CreateGetAllQuery, getTitle($"Creating `GetAll{StringHelper.Pluralize(data.ViewModel!.Name)}Query`…"))
                .AddStep(this.CreateGetByIdQuery, getTitle($"Creating `GetById{data.ViewModel.Name}Query`…"))

                .AddStep(this.CreateInsertCommand, getTitle($"Creating `Insert{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateUpdateCommand, getTitle($"Creating `Update{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateDeleteCommand, getTitle($"Creating `Delete{data.ViewModel.Name}Command`…"))

                .AddStep(this.CreateListComponent, getTitle($"Creating `{data.ViewModel.Name}ListComponent`…"))
                .AddStep(this.CreateDetailsComponent, getTitle($"Creating `{data.ViewModel.Name}DetailsComponent`…"))
                .AddStep(this.CreateBlazorPage, getTitle($"Creating {data.ViewModel.Name} Blazor Page…"))

                .AddStep(this.CreateCodes, getTitle($"Generating {data.ViewModel.Name} Codes…"));

        static string finalize(in CreationData result)
            => !result.Result.Message.IsNullOrEmpty()
                    ? result.Result.Message
                    : result.CancellationTokenSource.IsCancellationRequested
                        ? "Generating process is cancelled."
                        : result.Result.IsSucceed
                            ? "Functionality view model is created."
                            : "An error occurred while creating functionality view model";

        #endregion Local Methods
    }

    public Result<Codes> GenerateCodes(in FunctionalityViewModel viewModel, GenerateCodesParameters? arguments = null)
    {
        var result = new Codes();
        var buffer = this._dtoCodeService.GenerateCodes(viewModel.DetailsViewModel, arguments);
        if (buffer.IsFailure)
        {
            return buffer;
        }

        result += buffer;
        return new(result);
    }

    private static void Cancel(in CreationData data, in string reason)
    {
        data.CancellationTokenSource.Cancel();
        data.SetResult(false, reason);
    }

    private async Task CreateBlazorPage(CreationData data)
    {
        await createPageViewModel(data);

        Task createPageViewModel(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true);
            var pageViewModel = rawDto
                .WithName($"Get{data.ViewModel.DbObject.Name}ViewModel")
                .With(x => x.IsViewModel = true);
            pageViewModel.Properties.Add(new()
            {
                Comment = data.COMMENT,
                Dto = data.ViewModel.DetailsViewModel,
                Name = data.ViewModel.DetailsViewModel.Name,
                Type = PropertyType.Dto
            });
            pageViewModel.Properties.Add(new()
            {
                Comment = data.COMMENT,
                Dto = data.ViewModel.ListViewModel,
                Name = data.ViewModel.ListViewModel.Name,
                Type = PropertyType.Dto,
                IsList = true
            });
            return Task.CompletedTask;
        }
    }

    private Task CreateCodes(CreationData arg)
        => Task.CompletedTask;

    private async Task CreateDeleteCommand(CreationData data)
    {
        await createParams(data);
        await createValidator(data);
        await createHandler(data);
        await createResult(data);

        Task createParams(CreationData data) => Task.CompletedTask;
        Task createValidator(CreationData data) => Task.CompletedTask;
        Task createHandler(CreationData data) => Task.CompletedTask;
        Task createResult(CreationData data) => Task.CompletedTask;
    }

    private async Task CreateDetailsComponent(CreationData data)
    {
        await createDetailsViewModel(data);
        await createDetailsFrontCode(data);
        await createDetailsBackendCode(data);

        Task createDetailsViewModel(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true);
            data.ViewModel.DetailsViewModel = rawDto
                .WithName($"Get{data.ViewModel.DbObject.Name}DetailsViewModel")
                .With(x => x.IsViewModel = true);
            return Task.CompletedTask;
        }

        Task createDetailsFrontCode(CreationData data) => Task.CompletedTask;

        Task createDetailsBackendCode(CreationData data) => Task.CompletedTask;
    }

    private async Task CreateGetAllQuery(CreationData data)
    {
        await createViewModel(data);
        await createParams(data);
        await createResult(data);

        async Task createViewModel(CreationData data)
        {
            data.GetAllQueryName = $"GetAll{StringHelper.Pluralize(data.DbTable.Name)}Query";
            var query = await this._queryService.CreateAsync();
            data.ViewModel.GetAllQuery = query
                .WithName($"{data.GetAllQueryName}ViewModel")
                .With(x => x.Category = CqrsSegregateCategory.Read)
                .With(x => x.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries"))
                .With(x => x.DbObject = data.ViewModel.DbObject)
                .With(x => x.FriendlyName = data.GetAllQueryName.SplitCamelCase().Merge(" "))
                .With(x => x.Comment = data.COMMENT)
                .With(async x => x.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId));
        }

        Task createParams(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, false);
            data.ViewModel.GetAllQuery.ParamDto = rawDto
                .WithName($"{data.GetAllQueryName}Params")
                .With(x => x.IsParamsDto = true);
            return Task.CompletedTask;
        }

        Task createResult(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true);
            data.ViewModel.GetAllQuery.ResultDto = rawDto
                .WithName($"GetAll{data.GetAllQueryName}Result")
                .With(x => x.IsResultDto = true);
            return Task.CompletedTask;
        }
    }

    private async Task CreateGetByIdQuery(CreationData data)
    {
        await createQuery(data);
        await createParams(data);
        await createResult(data);

        async Task createQuery(CreationData data)
        {
            data.GetByIdQueryName = $"GetById{data.DbTable.Name}Query";
            var query = await this._queryService.CreateAsync();
            data.ViewModel.GetByIdQuery = query
                .WithName($"{data.GetByIdQueryName}ViewModel")
                .With(x => x.Category = CqrsSegregateCategory.Read)
                .With(x => x.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries"))
                .With(x => x.DbObject = data.ViewModel.DbObject)
                .With(x => x.FriendlyName = data.GetByIdQueryName.SplitCamelCase().Merge(" "))
                .With(x => x.Comment = data.COMMENT)
                .With(async x => x.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId));
        }

        Task createParams(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, false)
                .WithName($"GetById{data.DbTable.Name}Params")
                .With(x => x.IsParamsDto = true)
                .With(x => x.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long }));
            data.ViewModel.GetByIdQuery.ParamDto = rawDto;
            return Task.CompletedTask;
        }

        Task createResult(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true)
                .WithName($"GetById{data.DbTable.Name}Result")
                .With(x => x.IsResultDto = true);
            data.ViewModel.GetByIdQuery.ResultDto = rawDto;
            return Task.CompletedTask;
        }
    }

    private async Task CreateInsertCommand(CreationData data)
    {
        var command = await this._commandService.CreateAsync();
        data.ViewModel.InsertCommand = command
            .With(x => x.Category = CqrsSegregateCategory.Create)
            .With(x => x.Comment = data.COMMENT);
        await createParams(data);
        await createValidator(data);
        await createHandler(data);
        await createResult(data);

        Task createValidator(CreationData data) => Task.CompletedTask;
        Task createHandler(CreationData data) => Task.CompletedTask;

        Task createParams(CreationData data)
        {
            var dto = this.CreateRawDto(data, true)
                .WithName($"Insert{data.DbTable.Name}Params")
                .With(x => x.IsParamsDto = true);
            var idProp = dto.Properties.FirstOrDefault(y => y.Name?.EqualsTo("id") is true);
            if (idProp != null)
            {
                _ = dto.Properties.Remove(idProp);
            }
            data.ViewModel.InsertCommand.ParamDto = dto;
            return Task.CompletedTask;
        }
        Task createResult(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, false)
                .WithName($"Insert{data.DbTable.Name}Result")
                .With(x => x.IsResultDto = true)
                .With(x => x.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long }));
            data.ViewModel.InsertCommand.ResultDto = rawDto;
            return Task.CompletedTask;
        }
    }

    private async Task CreateListComponent(CreationData data)
    {
        await createListViewModel(data);
        await createListFrontCode(data);
        await createListBackendCode(data);

        Task createListViewModel(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true);
            data.ViewModel.ListViewModel = rawDto
                .WithName($"Get{data.ViewModel.DbObject.Name}ListViewModel")
                .With(x => x.IsViewModel = true);
            return Task.CompletedTask;
        }

        Task createListFrontCode(CreationData data)
            => Task.CompletedTask;

        Task createListBackendCode(CreationData data)
            => Task.CompletedTask;
    }

    private DtoViewModel CreateRawDto(CreationData data, bool addTableColumns = false, Action<DtoViewModel>? initialize = null)
    {
        var detailsViewModel = this._dtoService.CreateByDbTable(DbTableViewModel.FromDbTable(data.DbTable), Enumerable.Empty<DbColumnViewModel>())
            .With(x => x.Comment = data.COMMENT)
            .With(x => x.Module.Id = data.ViewModel.ModuleId)
            .With(x => x.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos"))
            .With(x => x.Functionality = data.ViewModel)
            .With(initialize);
        if (addTableColumns)
        {
            var columns = data.DbTable.Columns
                .Select(DbColumnViewModel.FromDbColumn)
                .Select(this._converter.ToPropertyViewModel).Compact()
                .Build();
            _ = detailsViewModel.Properties.AddRange(columns);
        }

        return detailsViewModel;
    }

    private async Task CreateUpdateCommand(CreationData data)
    {
        await createParams(data);
        await createValidator(data);
        await createHandler(data);
        await createResult(data);

        Task createParams(CreationData data) => Task.CompletedTask;
        Task createValidator(CreationData data) => Task.CompletedTask;
        Task createHandler(CreationData data) => Task.CompletedTask;
        Task createResult(CreationData data) => Task.CompletedTask;
    }

    private class CreationData : IDisposable
    {
        internal readonly string COMMENT = "Auto-generated by Functionality Service.";
        private Result<FunctionalityViewModel>? _result;

        internal CreationData(FunctionalityViewModel result, Table dbTable, CancellationTokenSource tokenSource)
            => (this.ViewModel, this.DbTable, this.CancellationTokenSource) = (result, dbTable, tokenSource);

        internal CancellationTokenSource CancellationTokenSource { get; }

        internal Table DbTable { get; }

        internal string? GetAllQueryName { get; set; }

        internal string? GetByIdQueryName { get; set; }

        [NotNull]
        internal Result<FunctionalityViewModel> Result => this._result ??= new(this.ViewModel);

        internal FunctionalityViewModel ViewModel { get; }

        public void Dispose()
            => this.CancellationTokenSource.Dispose();

        [DarkMethod]
        internal void SetResult(bool isSucceed, in string? message = null)
            => this._result = new(this.ViewModel) { Message = message, Succeed = isSucceed };
    }
}

static file class Extensions
{
    public static TViewModel WithName<TViewModel>(this TViewModel model, string name)
        where TViewModel : InfraViewModelBase<long?>
    {
        model.Name = name;
        return model;
    }
}