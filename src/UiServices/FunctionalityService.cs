using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.Services;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Data.SqlServer.Dynamics;
using Library.Exceptions;
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

    #region Generate Families

    public async Task<Result<FunctionalityViewModel?>> GenerateAsync(FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        Check.IfArgumentNotNull(viewModel);

        if (!validate(viewModel).TryParse(out var validationChecks))
        {
            return validationChecks!;
        }

        this._reporter.Report(description: "Initializing...");
        var data = await initialize(viewModel, viewModel.DbTable is not null ? null : this._readDbContext.Database.GetConnectionString());

        var process = MultistepProcessRunner<CreationData>.New(data, this._reporter)
            .AddStep(this.CreateGetAllQuery, getTitle($"Creating `GetAll{StringHelper.Pluralize(data.ViewModel.Name)}Query`…"))
            .AddStep(this.CreateGetByIdQuery, getTitle($"Creating `GetById{data.ViewModel.Name}Query`…"))

            .AddStep(this.CreateInsertCommand, getTitle($"Creating `Insert{data.ViewModel.Name}Command`…"))
            .AddStep(this.CreateUpdateCommand, getTitle($"Creating `Update{data.ViewModel.Name}Command`…"))
            .AddStep(this.CreateDeleteCommand, getTitle($"Creating `Delete{data.ViewModel.Name}Command`…"))

            .AddStep(this.CreateListComponent, getTitle($"Creating `{data.ViewModel.Name}ListComponent`…"))
            .AddStep(this.CreateDetailsComponent, getTitle($"Creating `{data.ViewModel.Name}DetailsComponent`…"))
            .AddStep(this.CreateBlazorPage, getTitle($"Creating {data.ViewModel.Name} Blazor Page…"))

            .AddStep(this.CreateCodes, getTitle($"Generating {data.ViewModel.Name} Codes…"));

        var result = await process.RunAsync(token);
        this._reporter.Report(description: result.Result.IsSucceed ? "Functionality view model is generated." : result.Result.Message);

        return result.Result!;

        string getTitle(string description)
            => $"{nameof(FunctionalityService)}: {data.ViewModel.Name} - ${description}";

        static async Task<CreationData> initialize(FunctionalityViewModel viewModel, string? connectionString)
        {
            var dataResult = viewModel;
            Table dbTable;
            if (viewModel.DbTable is not null)
            {
                dbTable = viewModel.DbTable;
            }
            else
            {
                var db = await Database.GetDatabaseAsync(connectionString!);
                dbTable = db.NotNull(() => "Database not found.")
                    .Tables[dataResult.DbObject.Name!].NotNull(() => new ObjectNotFoundException($"Table name `{dataResult.DbObject}` not found."));
            }
            var data = new CreationData(dataResult, dbTable);
            return data;
        }

        static Result<FunctionalityViewModel> validate(FunctionalityViewModel model)
            => model.Check()
                .ArgumentNotNull()
                .NotNull(x => x.DbObject)
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
                .With(x => x.Name = $"Get{data.ViewModel.DbObject.Name}ViewModel")
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
        => throw new NotImplementedException();

    private async Task CreateDeleteCommand(CreationData data)
    {
        await createParams(data);
        await createValidator(data);
        await createHandler(data);
        await createResult(data);

        Task createParams(CreationData data) => throw new NotImplementedException();
        Task createValidator(CreationData data) => throw new NotImplementedException();
        Task createHandler(CreationData data) => throw new NotImplementedException();
        Task createResult(CreationData data) => throw new NotImplementedException();
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
                .With(x => x.Name = $"Get{data.ViewModel.DbObject.Name}DetailsViewModel")
                .With(x => x.IsViewModel = true);
            return Task.CompletedTask;
        }

        Task createDetailsFrontCode(CreationData data) => throw new NotImplementedException();

        Task createDetailsBackendCode(CreationData data) => throw new NotImplementedException();
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
            data.ViewModel.GetAllQueryViewModel = query.Fluent()
                .With(x => x.Name = $"{data.GetAllQueryName}ViewModel")
                .With(x => x.Category = CqrsSegregateCategory.Read)
                .With(x => x.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries"))
                .With(x => x.DbObject = data.ViewModel.DbObject)
                .With(x => x.HasPartialHandller = true)
                .With(x => x.HasPartialOnInitialize = true)
                .With(x => x.FriendlyName = data.GetAllQueryName.SplitCamelCase().Merge(" "))
                .With(x => x.Comment = data.COMMENT)
                .IfTrue(data.ViewModel.ModuleId != 0, async x => x.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId));
        }

        Task createParams(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, false);
            data.ViewModel.GetAllQueryViewModel.ParamDto = rawDto
                .With(x => x.IsParamsDto = true)
                .With(x => x.Name = $"{data.GetAllQueryName}Params");
            return Task.CompletedTask;
        }

        Task createResult(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true);
            data.ViewModel.GetAllQueryViewModel.ResultDto = rawDto
                .With(x => x.IsResultDto = true)
                .With(x => x.Name = $"GetAll{data.GetAllQueryName}Result");
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
            data.ViewModel.GetByIdQueryViewModel = query.Fluent()
                .With(x => x.Name = $"{data.GetByIdQueryName}ViewModel")
                .With(x => x.Category = CqrsSegregateCategory.Read)
                .With(x => x.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries"))
                .With(x => x.DbObject = data.ViewModel.DbObject)
                .With(x => x.HasPartialHandller = true)
                .With(x => x.HasPartialOnInitialize = true)
                .With(x => x.FriendlyName = data.GetByIdQueryName.SplitCamelCase().Merge(" "))
                .With(x => x.Comment = data.COMMENT)
                .IfTrue(data.ViewModel.ModuleId != 0, async x => x.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId));
        }

        Task createParams(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, false)
                .With(x => x.IsParamsDto = true)
                .With(x => x.Name = $"GetById{data.DbTable.Name}Params")
                .With(x => x.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long }));
            data.ViewModel.GetByIdQueryViewModel.ParamDto = rawDto;
            return Task.CompletedTask;
        }

        Task createResult(CreationData data)
        {
            var rawDto = this.CreateRawDto(data, true)
                .With(x => x.IsResultDto = true)
                .With(x => x.Name = $"GetById{data.DbTable.Name}Result");
            data.ViewModel.GetByIdQueryViewModel.ResultDto = rawDto;
            return Task.CompletedTask;
        }
    }

    private async Task CreateInsertCommand(CreationData data)
    {
        await createParams(data);
        await createValidator(data);
        await createHandler(data);
        await createResult(data);

        Task createValidator(CreationData data) => throw new NotImplementedException();
        Task createHandler(CreationData data) => throw new NotImplementedException();

        Task createParams(CreationData data)
        {
            var dto = this.CreateRawDto(data, true)
                .With(x => x.IsParamsDto = true)
                .With(x => x.Name = $"Insert{data.DbTable.Name}Params");
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
                .With(x => x.IsResultDto = true)
                .With(x => x.Name = $"Insert{data.DbTable.Name}Result")
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
                .With(x => x.Name = $"Get{data.ViewModel.DbObject.Name}ListViewModel")
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

        Task createParams(CreationData data) => throw new NotImplementedException();
        Task createValidator(CreationData data) => throw new NotImplementedException();
        Task createHandler(CreationData data) => throw new NotImplementedException();
        Task createResult(CreationData data) => throw new NotImplementedException();
    }

    private struct CreationData
    {
        internal readonly string COMMENT = "Auto-generated by Functionality Service.";
        private Result<FunctionalityViewModel>? _result;

        internal CreationData(FunctionalityViewModel result, Table dbTable)
            => (this.ViewModel, this.DbTable) = (result, dbTable);

        public Result<FunctionalityViewModel> Result => this._result ??= new(this.ViewModel);

        internal Table DbTable { get; }

        internal string? GetAllQueryName { get; set; }

        internal string? GetByIdQueryName { get; set; }

        internal FunctionalityViewModel ViewModel { get; }
    }

    #endregion Generate Families

    Task<IDbContextTransaction> IAsyncTransactionalService.BeginTransactionAsync(CancellationToken cancellationToken)
        => this._writeDbContext.BeginTransactionAsync(cancellationToken);

    Task<Result> IAsyncTransactionalService.CommitTransactionAsync(CancellationToken cancellationToken)
        => this._writeDbContext.CommitTransactionAsync(cancellationToken);

    public Result<Codes> GenerateCodes(in FunctionalityViewModel viewModel, GenerateCodesParameters? arguments = null)
        => throw new NotImplementedException();

    public void ResetChanges()
        => this._writeDbContext.ResetChanges();

    Task IAsyncTransactionalService.RollbackTransactionAsync(CancellationToken cancellationToken)
        => this._writeDbContext.Database.RollbackTransactionAsync(cancellationToken);

    public Task<Result<int>> SaveChangesAsync()
        => this._writeDbContext.SaveChangesResultAsync();

    Task<Result<FunctionalityViewModel>> IAsyncValidator<FunctionalityViewModel>.ValidateAsync(FunctionalityViewModel viewModel)
        => viewModel.Check()
            .ArgumentNotNull()
            .NotNull(x => x.Name)
            .NotNull(x => x.NameSpace)
            .Build().ToAsync();
}