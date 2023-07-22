using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Data.SqlServer.Dynamics;
using Library.DesignPatterns.Markers;
using Library.Exceptions;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Threading.MultistepProgress;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal sealed partial class FunctionalityService
{
    public async Task<Result<Codes>> GenerateCodesAsync(FunctionalityViewModel viewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs? args = null, CancellationToken token = default)
    {
        if (!validate(viewModel).TryParse(out var result))
        {
            return Result<Codes>.From(result, Codes.Empty);
        }

        var results = await generateCodes(viewModel, (args?.UpdateModelView ?? false) ? viewModel.CodesResults : new(), token);

        return results.Merge();

        static Result<FunctionalityViewModel> validate(FunctionalityViewModel viewModel) =>
            viewModel.Check()
                     .ArgumentNotNull()
                     .NotNull(x => x.GetAllQueryViewModel)
                     .NotNull(x => x.GetByIdQueryViewModel)
                     .NotNull(x => x.InsertCommandViewModel)
                     .NotNull(x => x.UpdateCommandViewModel)
                     .NotNull(x => x.DeleteCommandViewModel)
                     .NotNull(x => x.BlazorListComponentViewModel)
                     .NotNull(x => x.BlazorDetailsComponentViewModel);

        async Task<FunctionalityViewModelCodesResults> generateCodes(FunctionalityViewModel viewModel, FunctionalityViewModelCodesResults results, CancellationToken token)
        {
            results.GetAllQueryCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.GetAllQueryViewModel, token: token);
            results.GetByIdQueryCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.GetByIdQueryViewModel, token: token);
            results.InsertCommandCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.InsertCommandViewModel, token: token);
            results.UpdateCommandCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.UpdateCommandViewModel, token: token);
            results.DeleteCommandCodes = await this._cqrsCodeService.GenerateCodeAsync(viewModel.DeleteCommandViewModel, token: token);
            results.BlazorListCodes = this._blazorCodingService.GenerateCodes(viewModel.BlazorListComponentViewModel);
            results.BlazorDetailsComponentViewModel = this._blazorCodingService.GenerateCodes(viewModel.BlazorDetailsComponentViewModel);
            return results;
        }
    }

    public async Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync(FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        #region Validate the viewModel argument

        if (!validate(viewModel, token).TryParse(out var validationChecks))
        {
            return validationChecks!;
        }

        #endregion Validate the viewModel argument

        #region Initialize

        this._reporter.Report(description: getTitle("Initializing..."));
        // If `viewModel.DbTable` is empty then the connection string:
        // `this._readDbContext.Database.GetConnectionString()` will be used to fill
        // `viewModel.DbTable`. Otherwise, `viewModel.DbTable` will be directly used (to be used in
        // Unit Test).
        var connectionString = viewModel.DbTable is not null ? null : this._readDbContext.Database.GetConnectionString();
        // Initialize the viewModel with the connection string
        var initResult = await initialize(viewModel, connectionString, token);
        // If the initialization fails, return the result
        if (!initResult.IsSucceed)
        {
            return Result<FunctionalityViewModel>.From(initResult, default!)!;
        }
        // Get the data and tokenSource from the initialization result
        var (data, tokenSource) = initResult.GetValue();
        // Initialize the steps for the process
        var process = initSteps(data);

        #endregion Initialize

        #region Process

        this._reporter.Report(description: getTitle("Running..."));
        // Run the process with the tokenSource
        var processResult = await process.RunAsync(data.CancellationTokenSource.Token);

        #endregion Process

        #region Finalize and prepare the result

        var message = getResultMessage(processResult, data.CancellationTokenSource.Token);
        this._reporter.Report(description: getTitle(message));
        tokenSource.Dispose();
        // Get the result from the processResult
        var result = processResult.Value.Result;

        #endregion Finalize and prepare the result

        return result!;

        #region Local Methods

        // Get the title for the description
        ProgressData getTitle(in string description)
            => new(Description: description, Sender: nameof(FunctionalityService));

        // Validate the model
        static Result<FunctionalityViewModel> validate(in FunctionalityViewModel model, CancellationToken token) =>
            model.Check()
                .RuleFor(_ => !token.IsCancellationRequested, () => new OperationCancelException("Cancelled by parent"))
                .ArgumentNotNull()
                .NotNull(x => x.Name)
                .NotNull(x => x.NameSpace)
                .NotNull(x => x.DbObjectViewModel)
                .NotNull(x => x.DbObjectViewModel.Name)
                .RuleFor(x => x.ModuleId != 0, () => new ValidationException("Module is not selected."));

        // Initialize the viewModel with the connection string
        static async Task<Result<(CreationData Data, CancellationTokenSource TokenSource)>> initialize(FunctionalityViewModel viewModel, string? connectionString, CancellationToken token)
        {
            // If the token is cancelled, return a failure result
            if (token.IsCancellationRequested)
            {
                return Result<(CreationData Data, CancellationTokenSource TokenSource)>.CreateFailure("Cancelled by parent", default);
            }

            // Get the dataResult from the viewModel
            var dataResult = viewModel;
            Table dbTable;
            // If viewModel.DbTable is not null, use it (FOR TESTING PURPOSES ONLY
            if (viewModel.DbTable is not null)
            {
                dbTable = viewModel.DbTable;
            }
            // Otherwise, get the database from the connection string and get the table from the database
            else
            {
                var db = await Database.GetDatabaseAsync(connectionString!, cancellationToken: token);
                dbTable = db.NotNull(() => new ObjectNotFoundException("Database not found."))
                            .Tables[dataResult.DbObjectViewModel.Name!].NotNull(() => new ObjectNotFoundException($"Table name `{dataResult.DbObjectViewModel}` not found."));
            }
            // Create a linked tokenSource from the token
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            // Create a new CreationData with the dataResult, dbTable, and cancellationTokenSource
            var result = new CreationData(dataResult, dbTable, tokenSource);
            // Return a success result with the result and cancellationTokenSource
            return Result<(CreationData Data, CancellationTokenSource TokenSource)>.CreateSuccess((result, tokenSource));
        }

        // Initialize the steps for the process
        MultistepProcessRunner<CreationData> initSteps(in CreationData data) =>
            MultistepProcessRunner<CreationData>.New(data, this._reporter, owner: nameof(FunctionalityService))
                .AddStep(this.CreateGetAllQuery, getTitle($"Creating `GetAll{StringHelper.Pluralize(data.ViewModel!.Name)}Query`…"))
                .AddStep(this.CreateGetByIdQuery, getTitle($"Creating `GetById{data.ViewModel.Name}Query`…"))

                .AddStep(this.CreateInsertCommand, getTitle($"Creating `Insert{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateUpdateCommand, getTitle($"Creating `Update{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateDeleteCommand, getTitle($"Creating `Delete{data.ViewModel.Name}Command`…"))

                .AddStep(this.GenerateCodes, getTitle($"Generating {data.ViewModel.Name} Codes…"))

                .AddStep(this.CreateListComponent, getTitle($"Creating `{data.ViewModel.Name}ListComponent`…"))
                .AddStep(this.CreateDetailsComponent, getTitle($"Creating `{data.ViewModel.Name}DetailsComponent`…"))
                .AddStep(this.CreateBlazorPage, getTitle($"Creating {data.ViewModel.Name} Blazor Page…"))
                ;

        // Finalize the process
        static string getResultMessage(in CreationData result, CancellationToken token) =>
            !result.Result.Message.IsNullOrEmpty()
                    ? result.Result.Message
                    : token.IsCancellationRequested
                        ? "Generating process is cancelled."
                        : result.Result.IsSucceed
                            ? "Functionality view model is created."
                            : "An error occurred while creating functionality view model";

        #endregion Local Methods
    }

    private Task CreateBlazorPage(CreationData data, CancellationToken token)
    {
        createPageViewModel();
        return Task.CompletedTask;

        void createPageViewModel()
        {
            var pageViewModel = this.RawDto(data, true);
            pageViewModel.Name = $"Get{data.ViewModel.DbObjectViewModel.Name}ViewModel";
            pageViewModel.IsViewModel = true;
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
        }
    }

    private Task CreateDeleteCommand(CreationData data, CancellationToken token)
    {
        return TaskRunner<CreationData>.StartWith(data)
            .Then(createHandler)
            .Then(createParams)
            .Then(createValidator)
            .Then(createResult)
            .RunAsync(token);

        void createParams(CreationData data)
        {
            var dto = this.RawDto(data, false);
            dto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            dto.Name = $"Delete{data.DbTable.Name}Params";
            dto.IsParamsDto = true;
            dto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
            data.ViewModel.DeleteCommandViewModel.ParamDto = dto;
        }
        Task createValidator(CancellationToken token) => Task.CompletedTask;
        async Task createHandler(CancellationToken token)
        {
            data.ViewModel.DeleteCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.DeleteCommandViewModel.Name = $"Delete{data.DbTable.Name}Command";
            data.ViewModel.DeleteCommandViewModel.Category = CqrsSegregateCategory.Delete;
            data.ViewModel.DeleteCommandViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.DeleteCommandViewModel.DbObject = data.ViewModel.DbObjectViewModel;
            data.ViewModel.DeleteCommandViewModel.FriendlyName = data.ViewModel.DeleteCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.DeleteCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.DeleteCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId, token);
        }
        void createResult(CreationData data)
        {
            var dto = this.RawDto(data, false);
            dto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            dto.Name = $"Delete{data.DbTable.Name}Result";
            dto.IsResultDto = true;
            data.ViewModel.DeleteCommandViewModel.ResultDto = dto;
        }
    }

    private Task CreateDetailsComponent(CreationData data, CancellationToken token)
    {
        return TaskRunner.StartWith(createDetailsViewModel)
            .Then(createDetailsFrontViewModel)
            .Then(createDetailsBackendViewModel)
            .RunAsync(token);

        void createDetailsViewModel()
        {
            var rawDto = this.RawDto(data, true);
            data.ViewModel.DetailsViewModel = rawDto;
            data.ViewModel.DetailsViewModel.Name = $"Get{data.ViewModel.DbObjectViewModel.Name}DetailsViewModel";
            data.ViewModel.DetailsViewModel.IsViewModel = true;
        }

        Task createDetailsFrontViewModel(CancellationToken token) =>
            Task.CompletedTask;

        Task createDetailsBackendViewModel(CancellationToken token) =>
            Task.CompletedTask;
    }

    private Task CreateGetAllQuery(CreationData data, CancellationToken token)
    {
        return TaskRunner.StartWith(createViewModel)
            .Then(createParams)
            .Then(createResult)
            .RunAsync(token);

        async Task createViewModel(CancellationToken token)
        {
            data.GetAllQueryName = $"GetAll{StringHelper.Pluralize(data.DbTable.Name)}Query";
            data.ViewModel.GetAllQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetAllQueryViewModel.Name = $"{data.GetAllQueryName}ViewModel";
            data.ViewModel.GetAllQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetAllQueryViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries");
            data.ViewModel.GetAllQueryViewModel.DbObject = data.ViewModel.DbObjectViewModel;
            data.ViewModel.GetAllQueryViewModel.FriendlyName = data.GetAllQueryName.SplitCamelCase().Merge(" ");
            data.ViewModel.GetAllQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetAllQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId, token: token);
        }

        void createParams()
        {
            data.ViewModel.GetAllQueryViewModel.ParamDto = this.RawDto(data, false);
            data.ViewModel.GetAllQueryViewModel.ParamDto.Name = $"{data.GetAllQueryName}Params";
            data.ViewModel.GetAllQueryViewModel.ParamDto.IsParamsDto = true;
        }

        void createResult()
        {
            data.ViewModel.GetAllQueryViewModel.ResultDto = this.RawDto(data, true);
            data.ViewModel.GetAllQueryViewModel.ResultDto.Name = $"GetAll{data.GetAllQueryName}Result";
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsResultDto = true;
        }
    }

    private Task CreateGetByIdQuery(CreationData data, CancellationToken token)
    {
        return TaskRunner.StartWith(createViewModel)
            .Then(createParams)
            .Then(createResult)
            .RunAsync(token);

        async Task createViewModel(CancellationToken token)
        {
            data.GetByIdQueryName = $"GetById{data.DbTable.Name}Query";
            data.ViewModel.GetByIdQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetByIdQueryViewModel.Name = $"{data.GetByIdQueryName}ViewModel";
            data.ViewModel.GetByIdQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetByIdQueryViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries");
            data.ViewModel.GetByIdQueryViewModel.DbObject = data.ViewModel.DbObjectViewModel;
            data.ViewModel.GetByIdQueryViewModel.FriendlyName = data.GetByIdQueryName.SplitCamelCase().Merge(" ");
            data.ViewModel.GetByIdQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetByIdQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId, token: token);
        }

        void createParams()
        {
            data.ViewModel.GetByIdQueryViewModel.ParamDto = this.RawDto(data, false);
            data.ViewModel.GetByIdQueryViewModel.ParamDto.Name = $"GetById{data.DbTable.Name}Params";
            data.ViewModel.GetByIdQueryViewModel.ParamDto.IsParamsDto = true;
            data.ViewModel.GetByIdQueryViewModel.ParamDto.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long });
        }

        void createResult()
        {
            data.ViewModel.GetByIdQueryViewModel.ResultDto = this.RawDto(data, true);
            data.ViewModel.GetByIdQueryViewModel.ResultDto.Name = $"GetById{data.DbTable.Name}Result";
            data.ViewModel.GetByIdQueryViewModel.ResultDto.IsResultDto = true;
        }
    }

    private Task CreateInsertCommand(CreationData data, CancellationToken token)
    {
        return TaskRunner<CreationData>.StartWith(data)
            .Then(createHandler)
            .Then(createParams)
            .Then(createValidator)
            .Then(createResult)
            .RunAsync(token);

        static Task createValidator(CreationData data, CancellationToken token) => Task.CompletedTask;

        async Task createHandler(CreationData data, CancellationToken token)
        {
            data.ViewModel.InsertCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.InsertCommandViewModel.Name = $"Insert{data.DbTable.Name}Command";
            data.ViewModel.InsertCommandViewModel.Category = CqrsSegregateCategory.Create;
            data.ViewModel.InsertCommandViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.InsertCommandViewModel.DbObject = data.ViewModel.DbObjectViewModel;
            data.ViewModel.InsertCommandViewModel.FriendlyName = data.ViewModel.InsertCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.InsertCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.InsertCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId, token);
        }

        void createParams(CreationData data)
        {
            var dto = this.RawDto(data, true);
            dto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            dto.Name = $"Insert{data.DbTable.Name}Params";
            dto.IsParamsDto = true;
            var idProp = dto.Properties.FirstOrDefault(y => y.Name?.EqualsTo("id") is true);
            if (idProp != null)
            {
                _ = dto.Properties.Remove(idProp);
            }
            data.ViewModel.InsertCommandViewModel.ParamDto = dto;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.InsertCommandViewModel.ResultDto = this.RawDto(data, false);
            data.ViewModel.InsertCommandViewModel.ResultDto.Name = $"Insert{data.DbTable.Name}Result";
            data.ViewModel.InsertCommandViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.InsertCommandViewModel.ResultDto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
        }
    }

    private Task CreateListComponent(CreationData data, CancellationToken token)
    {
        return TaskRunner.StartWith(createListViewModel)
            .Then(createListFrontCode)
            .Then(createListBackendCode)
            .RunAsync(token);

        void createListViewModel()
        {
            data.ViewModel.ListViewModel = this.RawDto(data, true);
            data.ViewModel.ListViewModel.Name = $"Get{data.ViewModel.DbObjectViewModel.Name}ListViewModel";
            data.ViewModel.ListViewModel.IsViewModel = true;
        }

        Task createListFrontCode(CancellationToken token)
            => Task.CompletedTask;

        Task createListBackendCode(CancellationToken token)
            => Task.CompletedTask;
    }

    private async Task CreateUpdateCommand(CreationData data, CancellationToken token)
    {
        data.ViewModel.UpdateCommandViewModel = await this._commandService.CreateAsync(token);
        _ = await TaskRunner<CreationData>.StartWith(data)
            .Then(createParamsAsync)
            .Then(createValidator)
            .Then(createHandler)
            .Then(createResult)
            .RunAsync(token);

        static Task createValidator(CreationData data, CancellationToken token) => Task.CompletedTask;

        async Task createHandler(CreationData data, CancellationToken token)
        {
            data.ViewModel.UpdateCommandViewModel.Name = $"Update{data.DbTable.Name}Command";
            data.ViewModel.UpdateCommandViewModel.Category = CqrsSegregateCategory.Update;
            data.ViewModel.UpdateCommandViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.UpdateCommandViewModel.DbObject = data.ViewModel.DbObjectViewModel;
            data.ViewModel.UpdateCommandViewModel.FriendlyName = data.ViewModel.UpdateCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.UpdateCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.UpdateCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId, token: token);
        }

        void createParamsAsync(CreationData data)
        {
            var dto = this.RawDto(data, true);
            dto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            dto.Name = $"Update{data.DbTable.Name}Params";
            dto.IsParamsDto = true;
            data.ViewModel.UpdateCommandViewModel.ParamDto = dto;
        }

        void createResult(CreationData data)
        {
            var dto = this.RawDto(data, false);
            dto.Name = $"Update{data.DbTable.Name}Result";
            dto.IsResultDto = true;
            dto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
            data.ViewModel.UpdateCommandViewModel.ResultDto = dto;
        }
    }

    private async Task GenerateCodes(CreationData data, CancellationToken token) =>
        await this.GenerateCodesAsync(data.ViewModel, token: token);

    private DtoViewModel RawDto(CreationData data, bool addTableColumns = false)
    {
        var detailsViewModel = create(data);
        return addTableColumns ? addColumns(detailsViewModel) : detailsViewModel;

        DtoViewModel create(CreationData data) =>
            this._dtoService.CreateByDbTable(DbTableViewModel.FromDbTable(data.DbTable), Enumerable.Empty<DbColumnViewModel>())
                .With(x => x.Comment = data.COMMENT)
                .With(x => x.Module.Id = data.ViewModel.ModuleId)
                .With(x => x.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos"))
                .With(x => x.Functionality = data.ViewModel);

        DtoViewModel addColumns(DtoViewModel detailsViewModel)
        {
            var columns = data.DbTable.Columns
                .Select(DbColumnViewModel.FromDbColumn)
                .Select(this._converter.ToPropertyViewModel)
                .Compact().Build();
            _ = detailsViewModel.Properties.AddRange(columns);
            return detailsViewModel;
        }
    }

    private class CreationData(FunctionalityViewModel result, Table dbTable, CancellationTokenSource tokenSource)
    {
        internal readonly string COMMENT = "Auto-generated by Functionality Service.";
        private Result<FunctionalityViewModel>? _result;

        internal CancellationTokenSource CancellationTokenSource { get; } = tokenSource;

        internal Table DbTable { get; } = dbTable;

        internal string? GetAllQueryName { get; set; }

        internal string? GetByIdQueryName { get; set; }

        internal Result<FunctionalityViewModel> Result => this._result ??= new(this.ViewModel);

        internal FunctionalityViewModel ViewModel { get; } = result;

        [DarkMethod(Reason = "Changes the class state.")]
        internal CreationData SetResult(bool isSucceed, in string? message = null)
            => this.Fluent(this._result = new(this.ViewModel) { Message = message, Succeed = isSucceed });

        [DarkMethod(Reason = "Changes the class state.")]
        internal CreationData SetResult(Result result)
            => this.Fluent(this._result = Result<FunctionalityViewModel>.From(result, this.ViewModel));
    }
}