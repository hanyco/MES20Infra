using Contracts.Services;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Data.SqlServer.Dynamics;
using Library.Exceptions.Validations;
using Library.Interfaces;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Database = Library.Data.SqlServer.Dynamics.Database;

namespace Services;

[Service]
internal sealed class FunctionalityService : IFunctionalityService, IFunctionalityCodeService
    , IBusinesService, IAsyncValidator<FunctionalityViewModel>, IAsyncTransactionSaveService, ILoggerContainer
{
    private readonly ICqrsCommandService _commandService;
    private readonly IEntityViewModelConverter _converter;
    private readonly ICqrsCodeGeneratorService _cqrsCodeService;
    private readonly IDtoService _dtoService;
    private readonly IModuleService _moduleService;
    private readonly ICqrsQueryService _queryService;
    private readonly InfraReadDbContext _readDbContext;
    private readonly IMultistepProcess _reporter;
    private readonly InfraWriteDbContext _writeDbContext;

    public FunctionalityService(
        InfraReadDbContext readDbContext,
        InfraWriteDbContext writeDbContext,
        IEntityViewModelConverter converter,
        IDtoService dtoService,
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
        this._queryService = queryService;
        this._commandService = commandService;
        this._cqrsCodeService = cqrsCodeService;
        this._moduleService = moduleService;
        this._reporter = reporter;
    }

    public ILogger Logger { get; }

    #region CRUD

    public Task<Result> DeleteAsync(FunctionalityViewModel model, bool persist = true)
        => this.DeleteAsync<FunctionalityViewModel, Functionality>(this._writeDbContext, model, persist, persist, this.Logger);

    public Task<IReadOnlyList<FunctionalityViewModel>> GetAllAsync()
        => this.GetAllAsync<FunctionalityViewModel, Functionality>(this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<FunctionalityViewModel?> GetByIdAsync(long id)
        => this.GetByIdAsync<FunctionalityViewModel, Functionality>(id, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<Result<FunctionalityViewModel>> InsertAsync(FunctionalityViewModel model, bool persist = true)
        => this.InsertAsync(this._readDbContext, model, this._converter.ToDbEntity, persist, logger: this.Logger).ModelResult();

    public Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true)
        => this.UpdateAsync(this._readDbContext, model, this._converter.ToDbEntity, persist, logger: this.Logger).ModelResult();

    #endregion CRUD

    #region Generate Family

    public async Task<Result<FunctionalityViewModel?>> GenerateAsync(FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        Check.IfArgumentNotNull(viewModel);
        if (!validateArgs(viewModel).TryParse(out var validationChecks))
        {
            return validationChecks!;
        }

        this._reporter.Report(description: "Initializing...");
        var data = await initialize(viewModel, this._readDbContext.Database.GetConnectionString()!);

        var process = MultistepProcessRunner<CreationData>.New(data, this._reporter)
            .AddStep(this.CreateGetAllQuery, getTitle($"Creating `GetAll{StringHelper.Pluralize(data.Result.Name)}Query`…"))
            .AddStep(this.CreateGetByIdQuery, getTitle($"Creating `GetById{data.Result.Name}Query`…"))

            .AddStep(this.CreateInsertCommand, getTitle($"Creating `Insert{data.Result.Name}Command`…"))
            .AddStep(this.CreateUpdateCommand, getTitle($"Creating `Update{data.Result.Name}Command`…"))
            .AddStep(this.CreateDeleteCommand, getTitle($"Creating `Delete{data.Result.Name}Command`…"))

            .AddStep(this.CreateListComponent, getTitle($"Creating `{data.Result.Name}ListComponent`…"))
            .AddStep(this.CreateDetailsComponent, getTitle($"Creating `{data.Result.Name}DetailsComponent`…"))
            .AddStep(this.CreateBlazorPage, getTitle($"Creating {data.Result.Name} Blazor Page…"))

            .AddStep(this.CreateCodes, getTitle($"Generating {data.Result.Name} Codes…"));

        var result = await process.RunAsync(token);

        return Result<FunctionalityViewModel?>.CreateSuccess(result.Result, "Functionality generated successfully.");

        string getTitle(string description)
            => $"{nameof(FunctionalityService)}: {data.Result.Name} - ${description}";

        static async Task<CreationData> initialize(FunctionalityViewModel viewModel, string connectionString)
        {
            var dataResult = viewModel;
            var db = await Database.GetDatabaseAsync(connectionString);
            var dbTable = db.NotNull(() => "Database not found.")
                .Tables[dataResult.RootDto!.DbObject.Name!].NotNull(() => new Library.Exceptions.ObjectNotFoundException($"Table name `{dataResult.RootDto!.DbObject}` not found."));
            var data = new CreationData(dataResult, dbTable);
            return data;
        }

        static Result<FunctionalityViewModel> validateArgs(FunctionalityViewModel args) 
            => args.Check().ArgumentNotNull()
                .NotNull(x => x.RootDto)
                .NotNull(x => x.RootDto.DbObject)
                .NotNull(x => x.Name)
                .NotNull(x => x.NameSpace)
                .RuleFor(x => x.ModuleId != 0, () => new ValidationException("Module is not selected."))
                .Build();
    }

    private async Task CreateBlazorPage(CreationData data)
    {
        await createPageViewModel(data);

        Task createPageViewModel(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true);
            var pageViewModel = rawDto
                .With(x => x.Name = $"Get{data.Result.RootDto!.DbObject.Name}ViewModel")
                .With(x => x.IsViewModel = true);
            pageViewModel.Properties.Add(new()
            {
                Comment = data.COMMENT,
                Dto = data.Result.DetailsViewModel,
                Name = data.Result.DetailsViewModel.Name,
                Type = PropertyType.Dto
            });
            pageViewModel.Properties.Add(new()
            {
                Comment = data.COMMENT,
                Dto = data.Result.ListViewModel,
                Name = data.Result.ListViewModel.Name,
                Type = PropertyType.Dto,
                IsList = true
            });
            return Task.CompletedTask;
        }
    }

    private Task CreateCodes(CreationData arg)
        => throw new NotImplementedException();

    private async Task CreateDeleteCommand(CreationData data)
    {
        await createDeleteCommandParams(data);
        await createDeleteCommandValidator(data);
        await createDeleteCommandHandler(data);
        await createDeleteCommandResult(data);

        Task createDeleteCommandParams(CreationData data) => throw new NotImplementedException();
        Task createDeleteCommandValidator(CreationData data) => throw new NotImplementedException();
        Task createDeleteCommandHandler(CreationData data) => throw new NotImplementedException();
        Task createDeleteCommandResult(CreationData data) => throw new NotImplementedException();
    }

    private async Task CreateDetailsComponent(CreationData data)
    {
        await createDetailsViewModel(data);
        await createDetailsFrontCode(data);
        await createDetailsBackendCode(data);

        Task createDetailsViewModel(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true);
            data.Result.DetailsViewModel = rawDto
                .With(x => x.Name = $"Get{data.Result.RootDto!.DbObject.Name}DetailsViewModel")
                .With(x => x.IsViewModel = true);
            return Task.CompletedTask;
        }

        Task createDetailsFrontCode(CreationData data) => throw new NotImplementedException();

        Task createDetailsBackendCode(CreationData data) => throw new NotImplementedException();
    }

    private async Task CreateGetAllQuery(CreationData data)
    {
        await createGetAllQueryViewModel(data);
        await createGetAllQueryParams(data);
        await createGetAllQueryResult(data);

        async Task createGetAllQueryViewModel(CreationData data)
        {
            data.GetAllQueryName = $"GetAll{StringHelper.Pluralize(data.DbTable.Name)}Query";
            var query = await this._queryService.CreateAsync();
            data.Result.GetAllQueryViewModel = query
                .With(x => x.Name = $"{data.GetAllQueryName}ViewModel")
                .With(x => x.Category = CqrsSegregateCategory.Read)
                .With(x => x.CqrsNameSpace = TypePath.Combine(data.Result.NameSpace, "Queries"))
                .With(x => x.DbObject = data.Result.RootDto.DbObject)
                .With(x => x.HasPartialHandller = true)
                .With(x => x.HasPartialOnInitialize = true)
                .With(x => x.FriendlyName = data.GetAllQueryName.SplitCamelCase().Merge(" "))
                .With(x => x.Comment = data.COMMENT).Fluent()
                .IfTrue(data.Result.ModuleId != 0, async x => x.Module = await this._moduleService.GetByIdAsync(data.Result.ModuleId));
        }

        Task createGetAllQueryParams(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, false);
            data.Result.GetAllQueryViewModel.ParamDto = rawDto
                .With(x => x.IsParamsDto = true)
                .With(x => x.Name = $"{data.GetAllQueryName}Params");
            return Task.CompletedTask;
        }

        Task createGetAllQueryResult(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true);
            data.Result.GetAllQueryViewModel.ResultDto = rawDto
                .With(x => x.IsResultDto = true)
                .With(x => x.Name = $"GetAll{data.GetAllQueryName}Result");
            return Task.CompletedTask;
        }
    }

    private async Task CreateGetByIdQuery(CreationData data)
    {
        await createQuery(data);
        await createParams(data);
        await createGetByIdQueryResult(data);

        async Task createQuery(CreationData data)
        {
            data.GetByIdQueryName = $"GetById{data.DbTable.Name}Query";
            var query = await this._queryService.CreateAsync();
            data.Result.GetByIdQueryViewModel = query.Fluent()
                .With(x => x.Name = $"{data.GetByIdQueryName}ViewModel")
                .With(x => x.Category = CqrsSegregateCategory.Read)
                .With(x => x.CqrsNameSpace = TypePath.Combine(data.Result.NameSpace, "Queries"))
                .With(x => x.DbObject = data.Result.RootDto.DbObject)
                .With(x => x.HasPartialHandller = true)
                .With(x => x.HasPartialOnInitialize = true)
                .With(x => x.FriendlyName = data.GetByIdQueryName.SplitCamelCase().Merge(" "))
                .With(x => x.Comment = data.COMMENT)
                .IfTrue(data.Result.ModuleId != 0, async x => x.Module = await this._moduleService.GetByIdAsync(data.Result.ModuleId));
        }

        Task createParams(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, false);
            data.Result.GetByIdQueryViewModel.ParamDto = rawDto
                .With(x => x.IsParamsDto = true)
                .With(x => x.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long }))
                .With(x => x.Name = $"GetById{data.DbTable.Name}Params");
            return Task.CompletedTask;
        }

        Task createGetByIdQueryResult(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true);
            data.Result.GetByIdQueryViewModel.ResultDto = rawDto
                .With(x => x.IsResultDto = true)
                .With(x => x.Name = $"GetById{data.DbTable.Name}Result");
            return Task.CompletedTask;
        }
    }

    private async Task CreateInsertCommand(CreationData data)
    {
        await createInsertCommandParams(data);
        await createInsertCommandResult(data);

        Task createInsertCommandParams(CreationData data)
            => throw new NotImplementedException();
        Task createInsertCommandResult(CreationData data)
            => throw new NotImplementedException();
    }

    private async Task CreateListComponent(CreationData data)
    {
        await createListViewModel(data);
        await createListFrontCode(data);
        await createListBackendCode(data);

        Task createListViewModel(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true);
            data.Result.ListViewModel = rawDto
                .With(x => x.Name = $"Get{data.Result.RootDto!.DbObject.Name}ListViewModel")
                .With(x => x.IsViewModel = true);
            return Task.CompletedTask;
        }

        Task createListFrontCode(CreationData data)
            => throw new NotImplementedException();

        Task createListBackendCode(CreationData data)
            => throw new NotImplementedException();
    }

    private DtoViewModel CreateRawDto(CreationData data, bool addTableColumns = false, Action<DtoViewModel>? initialize = null)
    {
        var rawDto = () => this._dtoService.CreateByDbTable(DbTableViewModel.FromDbTable(data.DbTable), Enumerable.Empty<DbColumnViewModel>());
        var detailsViewModel = rawDto()
            .With(x => x.Comment = data.COMMENT)
            .With(x => x.Module.Id = data.Result.ModuleId)
            .With(x => x.NameSpace = TypePath.Combine(data.Result.NameSpace, "Dtos"))
            .With(x => x.Functionality = data.Result)
            .With(initialize);
        if (addTableColumns)
        {
            var column = data.DbTable.Columns
                .Select(DbColumnViewModel.FromDbColumn)
                .Select(this._converter.ToPropertyViewModel).Compact()
                .Build();
            _ = detailsViewModel.Properties.AddRange(column);
        }

        return detailsViewModel;
    }

    private async Task CreateUpdateCommand(CreationData data)
    {
        await createUpdateCommandParams(data);
        await createUpdateCommandValidator(data);
        await createUpdateCommandHandler(data);
        await createUpdateCommandResult(data);

        Task createUpdateCommandParams(CreationData data) => throw new NotImplementedException();

        Task createUpdateCommandValidator(CreationData data) => throw new NotImplementedException();

        Task createUpdateCommandHandler(CreationData data) => throw new NotImplementedException();

        Task createUpdateCommandResult(CreationData data) => throw new NotImplementedException();
    }

    private struct CreationData
    {
        public readonly string COMMENT = "Auto-generated by Functionality Service.";

        public CreationData(FunctionalityViewModel result, Table dbTable)
            => (this.Result, this.DbTable) = (result, dbTable);

        public Table DbTable { get; }

        public string? GetAllQueryName { get; set; }

        public string? GetByIdQueryName { get; set; }

        public FunctionalityViewModel Result { get; }
    }

    #endregion Generate Family

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => this._writeDbContext.BeginTransactionAsync(cancellationToken);

    public Task<Result> CommitTransactionAsync(CancellationToken cancellationToken = default)
        => this._writeDbContext.CommitTransactionAsync(cancellationToken);

    public Codes GenerateCodes(in FunctionalityViewModel viewModel, GenerateCodesParameters? arguments = null)
        => throw new NotImplementedException();

    public void ResetChanges()
        => this._writeDbContext.ResetChanges();

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        => this._writeDbContext.Database.RollbackTransactionAsync(cancellationToken);

    public Task<Result<int>> SaveChangesAsync()
        => this._writeDbContext.SaveChangesResultAsync();

    public Task<Result<FunctionalityViewModel>> ValidateAsync(FunctionalityViewModel viewModel)
        => Check.MustBeNotNull(viewModel).TryParse(out var viewModelNullCheck)
            ? viewModelNullCheck.ToAsync()
            : (Check.MustBeNotNull(viewModel, viewModel.Name)
                   + Check.MustBeNotNull(viewModel, viewModel.RootDto)
                   + Check.MustBeNotNull(viewModel, viewModel.NameSpace)).ToAsync();
}