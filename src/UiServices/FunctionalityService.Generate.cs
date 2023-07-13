using System.Diagnostics.CodeAnalysis;

using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;
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

using Database = Library.Data.SqlServer.Dynamics.Database;

namespace Services;

[Service]
internal sealed partial class FunctionalityService
{
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
        var processResult = await process.RunAsync(tokenSource.Token);

        #endregion Process

        #region Finalize and prepare the result

        var message = getResultMessage(processResult);
        this._reporter.Report(description: getTitle(message));
        tokenSource.Dispose();
        // Get the result from the processResult
        var result = processResult.Result;

        #endregion Finalize and prepare the result

        return result!;

        #region Local Methods

        // Get the title for the description
        ProgressData getTitle(in string description)
            => new(Description: description, Sender: nameof(FunctionalityService));

        // Validate the model
        static Result<FunctionalityViewModel?> validate(in FunctionalityViewModel model, CancellationToken token)
            => model.Check()
                    .RuleFor(_ => !token.IsCancellationRequested, () => new OperationCancelException("Cancelled by parent"))
                    .ArgumentNotNull().ThrowOnFail()
                    .NotNull(x => x.Name)
                    .NotNull(x => x.NameSpace)
                    .NotNull(x => x.DbObject)
                    .NotNull(x => x.DbObject.Name)
                    .RuleFor(x => x.ModuleId != 0, () => new ValidationException("Module is not selected."))
                    .Build()!;

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
            // If viewModel.DbTable is not null, use it
            if (viewModel.DbTable is not null)
            {
                dbTable = viewModel.DbTable;
            }
            // Otherwise, get the database from the connection string and get the table from the database
            else
            {
                var db = await Database.GetDatabaseAsync(connectionString!, cancellationToken: token);
                dbTable = db.NotNull(() => new ObjectNotFoundException("Database not found."))
                            .Tables[dataResult.DbObject.Name!].NotNull(() => new ObjectNotFoundException($"Table name `{dataResult.DbObject}` not found."));
            }
            // Create a linked tokenSource from the token
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            // Create a new CreationData with the dataResult, dbTable, and cancellationTokenSource
            var result = new CreationData(dataResult, dbTable, cancellationTokenSource);
            // Return a success result with the result and cancellationTokenSource
            return Result<(CreationData Data, CancellationTokenSource TokenSource)>.CreateSuccess((result, cancellationTokenSource));
        }

        // Initialize the steps for the process
        MultistepProcessRunner<CreationData> initSteps(in CreationData data)
            => MultistepProcessRunner<CreationData>.New(data, this._reporter, owner: nameof(FunctionalityService))
                .AddStep(this.CreateGetAllQuery, getTitle($"Creating `GetAll{StringHelper.Pluralize(data.ViewModel!.Name)}Query`…"))
                .AddStep(this.CreateGetByIdQuery, getTitle($"Creating `GetById{data.ViewModel.Name}Query`…"))

                .AddStep(this.CreateInsertCommand, getTitle($"Creating `Insert{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateUpdateCommand, getTitle($"Creating `Update{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateDeleteCommand, getTitle($"Creating `Delete{data.ViewModel.Name}Command`…"))

                .AddStep(this.CreateCodes, getTitle($"Generating {data.ViewModel.Name} Codes…"))

                .AddStep(this.CreateListComponent, getTitle($"Creating `{data.ViewModel.Name}ListComponent`…"))
                .AddStep(this.CreateDetailsComponent, getTitle($"Creating `{data.ViewModel.Name}DetailsComponent`…"))
                .AddStep(this.CreateBlazorPage, getTitle($"Creating {data.ViewModel.Name} Blazor Page…"));

        // Finalize the process
        static string getResultMessage(in CreationData result)
            => !result.Result.Message.IsNullOrEmpty()
                    ? result.Result.Message
                    : result.CancellationTokenSource.IsCancellationRequested
                        ? "Generating process is cancelled."
                        : result.Result.IsSucceed
                            ? "Functionality view model is created."
                            : "An error occurred while creating functionality view model";

        #endregion Local Methods
    }

    private Task CreateBlazorPage(CreationData data)
    {
        createPageViewModel();
        return Task.CompletedTask;

        void createPageViewModel()
        {
            var pageViewModel = this.RawDto(data, true);
            pageViewModel.Name = $"Get{data.ViewModel.DbObject.Name}ViewModel";
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

    private async Task CreateCodes(CreationData data)
    {
        // TODO: Not done yet.
        var codes = await this.GenerateCodesAsync(data.ViewModel, token: data.CancellationTokenSource.Token);
        if (!codes)
        {
            data.SetResult(codes);
            return;
        }
        //ToDo arg.ViewModel.Codes = codes;
    }

    private Task CreateDeleteCommand(CreationData data)
    {
        return TaskRunner.StartWith(createParams)
            .Then(createValidator)
            .Then(createHandler)
            .Then(createResult)
            .RunAsync(data.CancellationTokenSource.Token);

        Task createParams(CancellationToken token) => Task.CompletedTask;
        Task createValidator(CancellationToken token) => Task.CompletedTask;
        Task createHandler(CancellationToken token) => Task.CompletedTask;
        Task createResult(CancellationToken token) => Task.CompletedTask;
    }

    private Task CreateDetailsComponent(CreationData data)
    {
        return TaskRunner.StartWith(createDetailsViewModel)
            .Then(createDetailsFrontCode)
            .Then(createDetailsBackendCode)
            .RunAsync(data.CancellationTokenSource.Token);

        void createDetailsViewModel()
        {
            var rawDto = this.RawDto(data, true);
            data.ViewModel.DetailsViewModel = rawDto;
            data.ViewModel.DetailsViewModel.Name = $"Get{data.ViewModel.DbObject.Name}DetailsViewModel";
            data.ViewModel.DetailsViewModel.IsViewModel = true;
        }

        Task createDetailsFrontCode(CancellationToken token) =>
            Task.CompletedTask;

        Task createDetailsBackendCode(CancellationToken token) =>
            Task.CompletedTask;
    }

    private Task CreateGetAllQuery(CreationData data)
    {
        return TaskRunner.StartWith(createViewModel)
            .Then(createParams)
            .Then(createResult)
            .RunAsync(data.CancellationTokenSource.Token);

        async Task createViewModel(CancellationToken token)
        {
            data.GetAllQueryName = $"GetAll{StringHelper.Pluralize(data.DbTable.Name)}Query";
            data.ViewModel.GetAllQuery = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetAllQuery.Name = $"{data.GetAllQueryName}ViewModel";
            data.ViewModel.GetAllQuery.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetAllQuery.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries");
            data.ViewModel.GetAllQuery.DbObject = data.ViewModel.DbObject;
            data.ViewModel.GetAllQuery.FriendlyName = data.GetAllQueryName.SplitCamelCase().Merge(" ");
            data.ViewModel.GetAllQuery.Comment = data.COMMENT;
            data.ViewModel.GetAllQuery.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId, token: token);
        }

        void createParams()
        {
            data.ViewModel.GetAllQuery.ParamDto = this.RawDto(data, false);
            data.ViewModel.GetAllQuery.ParamDto.Name = $"{data.GetAllQueryName}Params";
            data.ViewModel.GetAllQuery.ParamDto.IsParamsDto = true;
        }

        void createResult()
        {
            data.ViewModel.GetAllQuery.ResultDto = this.RawDto(data, true);
            data.ViewModel.GetAllQuery.ResultDto.Name = $"GetAll{data.GetAllQueryName}Result";
            data.ViewModel.GetAllQuery.ResultDto.IsResultDto = true;
        }
    }

    private Task CreateGetByIdQuery(CreationData data)
    {
        return TaskRunner.StartWith(createQuery)
            .Then(createParams)
            .Then(createResult)
            .RunAsync(data.CancellationTokenSource.Token);

        async Task createQuery(CancellationToken token)
        {
            data.GetByIdQueryName = $"GetById{data.DbTable.Name}Query";
            data.ViewModel.GetByIdQuery = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetByIdQuery.Name = $"{data.GetByIdQueryName}ViewModel";
            data.ViewModel.GetByIdQuery.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetByIdQuery.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries");
            data.ViewModel.GetByIdQuery.DbObject = data.ViewModel.DbObject;
            data.ViewModel.GetByIdQuery.FriendlyName = data.GetByIdQueryName.SplitCamelCase().Merge(" ");
            data.ViewModel.GetByIdQuery.Comment = data.COMMENT;
            data.ViewModel.GetByIdQuery.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId, token: token);
        }

        void createParams()
        {
            data.ViewModel.GetByIdQuery.ParamDto = this.RawDto(data, false);
            data.ViewModel.GetByIdQuery.ParamDto.Name = $"GetById{data.DbTable.Name}Params";
            data.ViewModel.GetByIdQuery.ParamDto.IsParamsDto = true;
            data.ViewModel.GetByIdQuery.ParamDto.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long });
        }

        void createResult()
        {
            data.ViewModel.GetByIdQuery.ResultDto = this.RawDto(data, true);
            data.ViewModel.GetByIdQuery.ResultDto.Name = $"GetById{data.DbTable.Name}Result";
            data.ViewModel.GetByIdQuery.ResultDto.IsResultDto = true;
        }
    }

    private Task CreateInsertCommand(CreationData data)
    {
        return TaskRunner.StartWith(createParams)
            .Then(createValidator)
            .Then(createHandler)
            .Then(createResult)
            .RunAsync(data.CancellationTokenSource.Token);

        Task createValidator(CancellationToken token) => Task.CompletedTask;

        async Task createHandler(CancellationToken token)
        {
            data.ViewModel.UpdateCommand = await this._commandService.CreateAsync(token: token);
            data.ViewModel.UpdateCommand.Name = $"Insert{data.DbTable.Name}Command";
            data.ViewModel.UpdateCommand.Category = CqrsSegregateCategory.Create;
            data.ViewModel.UpdateCommand.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.UpdateCommand.DbObject = data.ViewModel.DbObject;
            data.ViewModel.UpdateCommand.FriendlyName = x.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.UpdateCommand.Comment = data.COMMENT;
            data.ViewModel.UpdateCommand.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId, token: token);
        }

        void createParams()
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
            data.ViewModel.InsertCommand.ParamDto = dto;
        }
        void createResult()
        {
            data.ViewModel.InsertCommand.ResultDto = this.RawDto(data, false);
            data.ViewModel.InsertCommand.ResultDto.Name = $"Insert{data.DbTable.Name}Result";
            data.ViewModel.InsertCommand.ResultDto.IsResultDto = true;
            data.ViewModel.InsertCommand.ResultDto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
        }
    }

    private Task CreateListComponent(CreationData data)
    {
        return TaskRunner.StartWith(createListViewModel)
            .Then(createListFrontCode)
            .Then(createListBackendCode)
            .RunAsync(data.CancellationTokenSource.Token);

        void createListViewModel()
        {
            data.ViewModel.ListViewModel = this.RawDto(data, true);
            data.ViewModel.ListViewModel.Name = $"Get{data.ViewModel.DbObject.Name}ListViewModel";
            data.ViewModel.ListViewModel.IsViewModel = true;
        }

        Task createListFrontCode(CancellationToken token)
            => Task.CompletedTask;

        Task createListBackendCode(CancellationToken token)
            => Task.CompletedTask;
    }

    private Task CreateUpdateCommand(CreationData data)
    {
        return TaskRunner.StartWith(createParams)
            .Then(createValidator)
            .Then(createHandler)
            .Then(createResult)
            .RunAsync(data.CancellationTokenSource.Token);

        Task createParams(CancellationToken token) => Task.CompletedTask;

        Task createValidator(CancellationToken token) => Task.CompletedTask;

        async Task createHandler(CancellationToken token)
        {
            data.ViewModel.UpdateCommand = await this._commandService.CreateAsync(token: token);
            data.ViewModel.UpdateCommand.Name = $"Update{data.DbTable.Name}Command";
            data.ViewModel.UpdateCommand.Category = CqrsSegregateCategory.Update;
            data.ViewModel.UpdateCommand.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.UpdateCommand.DbObject = data.ViewModel.DbObject;
            data.ViewModel.UpdateCommand.FriendlyName = data.ViewModel.UpdateCommand.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.UpdateCommand.Comment = data.COMMENT;
            data.ViewModel.UpdateCommand.Module = await this._moduleService.GetByIdAsync(data.ViewModel.ModuleId, token: token);
        }

        Task createResult(CancellationToken token) => Task.CompletedTask;
    }

    private DtoViewModel RawDto(CreationData data, bool addTableColumns = false)
    {
        var detailsViewModel = createViewModel(data);
        return addTableColumns ? addColumns(detailsViewModel) : detailsViewModel;

        DtoViewModel createViewModel(CreationData data) =>
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

    [Immutable]
    private class CreationData
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

        [DarkMethod(Reason = "Changes the class state.")]
        internal void SetResult(bool isSucceed, in string? message = null)
            => this._result = new(this.ViewModel) { Message = message, Succeed = isSucceed };

        [DarkMethod(Reason = "Changes the class state.")]
        internal void SetResult(Result result)
            => this._result = Result<FunctionalityViewModel>.From(result, this.ViewModel);
    }
}