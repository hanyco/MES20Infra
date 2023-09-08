using System.Diagnostics.CodeAnalysis;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration.Models;
using Library.Results;
using Library.Threading;
using Library.Threading.MultistepProgress;
using Library.Validations;

using Services.Helpers;

namespace Services;

internal sealed partial class FunctionalityService
{
    /// <summary>
    /// Generates codes asynchronously based on the provided FunctionalityViewModel and optional
    /// arguments. It is closely related to the FunctionalityViewModel and its associated codes.
    /// </summary>
    /// <param name="viewModel">
    /// The FunctionalityViewModel instance containing components and view models.
    /// </param>
    /// <param name="args">Optional arguments for code generation.</param>
    /// <param name="token">Cancellation token to cancel the code generation process.</param>
    /// <returns>
    /// A Result containing the generated codes or a failure message if no codes were generated.
    /// </returns>
    public async Task<Result<Codes>> GenerateCodesAsync(FunctionalityViewModel viewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs? args = null, CancellationToken token = default)
    {
        Check.MustBeArgumentNotNull(viewModel);

        // Determine whether to update existing codes or generate new ones.
        var codeResult = (args?.UpdateModelView ?? false) ? viewModel.Codes : new();

        // Generate codes asynchronously and combine the results.
        var results = await generateCodes(viewModel, codeResult, token);

        // Combine generated results if available, or return a failure message.
        return results.Any()
            ? Result<Codes>.Combine(results, Codes.Combine)
            : Result<Codes>.CreateFailure("No codes generated. ViewModel has no parameter to generate any codes.", Codes.Empty)!;

        // Internal method to generate codes for various components and view models.
        async Task<IEnumerable<Result<Codes>>> generateCodes(FunctionalityViewModel viewModel, FunctionalityViewModelCodes codes, CancellationToken token)
        {
            var result = new List<Result<Codes>>();

            if (viewModel.SourceDto != null)
            {
                var codeGenRes = addToList(this._dtoCodeService.GenerateCodes(viewModel.SourceDto));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.SourceDtoCodes = codeGenRes;
            }

            // Generate codes for GetAllQueryViewModel if available.
            if (viewModel.GetAllQueryViewModel != null)
            {
                var codeGenRes = await gatherAllCodesAsync(viewModel.GetAllQueryViewModel, token: token);
                if (codeGenRes.Any(x => !x))
                {
                    return result;
                }

                codes.GetAllQueryCodes = new(codeGenRes.Select(x => x.Value));
            }

            // Generate codes for GetByIdQueryViewModel if available.
            if (viewModel.GetByIdQueryViewModel != null)
            {
                var codeGenRes = await gatherAllCodesAsync(viewModel.GetByIdQueryViewModel, token: token);
                if (codeGenRes.Any(x => !x))
                {
                    return result;
                }

                codes.GetByIdQueryCodes = new(codeGenRes.Select(x => x.Value));
            }

            // Generate codes for InsertCommandViewModel if available.
            if (viewModel.InsertCommandViewModel != null)
            {
                var codeGenRes = await gatherAllCodesAsync(viewModel.InsertCommandViewModel, token: token);
                if (codeGenRes.Any(x => !x))
                {
                    return result;
                }

                codes.InsertCommandCodes = new(codeGenRes.Select(x => x.Value));
            }

            // Generate codes for UpdateCommandViewModel if available.
            if (viewModel.UpdateCommandViewModel != null)
            {
                var codeGenRes = await gatherAllCodesAsync(viewModel.UpdateCommandViewModel, token: token);
                if (codeGenRes.Any(x => !x))
                {
                    return result;
                }

                codes.UpdateCommandCodes = new(codeGenRes.Select(x => x.Value));
            }

            // Generate codes for DeleteCommandViewModel if available.
            if (viewModel.DeleteCommandViewModel != null)
            {
                var codeGenRes = await gatherAllCodesAsync(viewModel.DeleteCommandViewModel, token: token);
                if (codeGenRes.Any(x => !x))
                {
                    return result;
                }

                codes.DeleteCommandCodes = new(codeGenRes.Select(x => x.Value));
            }

            // Generate codes for BlazorPageViewModel if available.
            if (viewModel.BlazorPageViewModel != null)
            {
                var codeGenRes = addToList(this._blazorPageCodingService.GenerateCodes(viewModel.BlazorPageViewModel));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.BlazorPageCodes = codeGenRes;
            }

            if (viewModel.BlazorPageViewModel?.DataContext != null)
            {
                var codeGenRes = addToList(this._dtoCodeService.GenerateCodes(viewModel.BlazorPageViewModel.DataContext));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.BlazorPageDataContextCodes = codeGenRes;
            }

            // Generate codes for BlazorListComponentViewModel if available.
            if (viewModel.BlazorListComponentViewModel != null)
            {
                var codeGenRes = addToList(this._blazorComponentCodingService.GenerateCodes(viewModel.BlazorListComponentViewModel));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.BlazorListComponentCodes = codeGenRes;
            }

            // Generate codes for BlazorDetailsComponentViewModel if available.
            if (viewModel.BlazorDetailsComponentViewModel != null)
            {
                var codeGenRes = addToList(this._blazorComponentCodingService.GenerateCodes(viewModel.BlazorDetailsComponentViewModel));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.BlazorDetailsComponentCodes = codeGenRes;
            }

            return result;

            // Internal method to add a code result to the result list.
            Result<Codes> addToList(Result<Codes> codeResult)
            {
                result.Add(codeResult);
                return codeResult;
            }

            async Task<List<Result<Codes>>> gatherAllCodesAsync(CqrsViewModelBase cqrsViewModel, CancellationToken token)
            {
                var getAllCodes = new List<Result<Codes>>
                {
                    addToList(this._dtoCodeService.GenerateCodes(cqrsViewModel.ParamsDto)),
                    addToList(this._dtoCodeService.GenerateCodes(cqrsViewModel.ResultDto)),
                    addToList(await this._cqrsCodeService.GenerateCodeAsync(cqrsViewModel, token: token))
                };
                return getAllCodes;
            }
        }
    }

    public async Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync(FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        if (!this.Validate(viewModel).TryParse(out var validationResult))
        {
            return validationResult!;
        }

        this._reporter.Report(description: getTitle("Initializing..."));
        // Initialize the result with the viewModel
        var initResult = initialize(viewModel, token);
        // If the initialization fails, return the result
        if (!initResult.IsSucceed)
        {
            return Result<FunctionalityViewModel>.From(initResult, viewModel)!;
        }
        // Get the data and tokenSource from the initialization result
        var (data, tokenSource) = initResult.GetValue();
        // Initialize the steps for the process
        var process = initSteps(data);

        this._reporter.Report(description: getTitle("Running..."));
        // Run the process with the tokenSource
        var processResult = await process.RunAsync(tokenSource.Token);

        var message = getResultMessage(processResult, tokenSource.Token);
        this._reporter.Report(description: getTitle(message));
        tokenSource.Dispose();
        // Get the result from the processResult
        var result = processResult.Value.Result;

        return result!;

        // Get the title for the description
        ProgressData getTitle(in string description) =>
            new(Description: description, Sender: nameof(FunctionalityService));

        // Initialize the viewModel with the connection string
        static Result<(CreationData Data, CancellationTokenSource TokenSource)> initialize(FunctionalityViewModel viewModel, CancellationToken token)
        {
            // Create a linked tokenSource from the token
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            // Create a new CreationData with the dataResult, dbTable, and cancellationTokenSource
            var result = new CreationData(viewModel, (viewModel.Name ?? viewModel.SourceDto.Name).NotNull(), tokenSource);
            // Return a success result with the result and cancellationTokenSource
            return Result<(CreationData, CancellationTokenSource)>.CreateSuccess((result, tokenSource));
        }

        // Initialize the steps for the process
        MultistepProcessRunner<CreationData> initSteps(in CreationData data) =>
            MultistepProcessRunner<CreationData>.New(data, this._reporter, owner: nameof(FunctionalityService))
                .AddStep(this.CreateGetAllQuery, getTitle($"Creating `GetAll{StringHelper.Pluralize(data.ViewModel.Name)}Query`…"))
                .AddStep(this.CreateGetByIdQuery, getTitle($"Creating `GetById{data.ViewModel.Name}Query`…"))

                .AddStep(this.CreateInsertCommand, getTitle($"Creating `Insert{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateUpdateCommand, getTitle($"Creating `Update{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateDeleteCommand, getTitle($"Creating `Delete{data.ViewModel.Name}Command`…"))

                .AddStep(this.CreateBlazorPage, getTitle($"Creating {data.ViewModel.Name} Blazor Page…"))
                .AddStep(this.CreateBlazorListComponent, getTitle($"Creating Blazor `{data.ViewModel.Name}ListComponent`…"))
                .AddStep(this.CreateBlazorDetailsComponent, getTitle($"Creating Blazor `{data.ViewModel.Name}DetailsComponent`…"));

        // Finalize the process
        static string getResultMessage(in CreationData result, CancellationToken token)
        {
            if (!result.Result.Message.IsNullOrEmpty())
            {
                return result.Result.Message;
            }
            if (result.Result.IsFailure)
            {
                return "An error occurred while creating functionality view model";
            }
            if (token.IsCancellationRequested)
            {
                return "Generating process is cancelled.";
            };
            return "Functionality view model is created.";
        }
    }

    private static DtoViewModel RawDto(CreationData data, bool addTableColumns = false)
    {
        // Create an initial DTO based on the input data
        var dto = new DtoViewModel(data.ViewModel.SourceDto.Id, data.ViewModel.Name!)
        {
            Comment = data.COMMENT, // Set DTO comment
            NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos"), // Set DTO namespace
            Functionality = data.ViewModel // Set DTO functionality
        }
        .With(x => x.Module.Id = data.ViewModel.SourceDto.Module.Id); // Set DTO module ID

        // If the addTableColumns parameter is true, add table columns to the DTO
        return addTableColumns ? AddColumns(data, dto) : dto;

        // Internal method for adding table columns to DTO
        static DtoViewModel AddColumns(CreationData data, DtoViewModel dto)
        {
            _ = dto.Properties.ClearAndAddRange(data.ViewModel.SourceDto.Properties.Select(x => new PropertyViewModel(x))); // Add columns to DTO
            return dto;
        }
    }

    private Task CreateBlazorDetailsComponent(CreationData data, CancellationToken token)
    {
        var name = CommonHelper.Purify(data.ViewModel.SourceDto.Name);
        return TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(addActions)
            .RunAsync(token);

        void createViewModel(CreationData data)
        {
            data.ViewModel.BlazorDetailsComponentViewModel = this._blazorComponentCodingService.CreateViewModel(data.ViewModel.SourceDto);
            data.ViewModel.BlazorDetailsComponentViewModel.Name = $"{name}DetailsComponent";
            data.ViewModel.BlazorDetailsComponentViewModel.ClassName = $"{name}DetailsComponent";
            data.ViewModel.BlazorDetailsComponentViewModel.IsGrid = false;
            data.ViewModel.BlazorDetailsComponentViewModel.PageDataContext = data.ViewModel.BlazorPageViewModel.DataContext;
            data.ViewModel.BlazorDetailsComponentViewModel.PageDataContextProperty = data.ViewModel.BlazorPageViewModel.DataContext.Properties.First(x => x.IsList != true);
            data.ViewModel.BlazorPageViewModel.Components.Add(data.ViewModel.BlazorDetailsComponentViewModel);
        }

        static void addActions(CreationData data)
        {
            HanyCo.Infra.UI.ViewModels.UiComponentActionViewModel saveButton = new()
            {
                Caption = "Save",
                //CqrsSegregate = data.ViewModel.InsertCommandViewModel,
                EventHandlerName = "SaveButton_OnClick",
                Guid = Guid.NewGuid(),
                IsEnabled = true,
                Name = "SaveButton",
                TriggerType = TriggerType.FormButton,
                Description = "Save the data to database",
                Position = new()
                {
                    Col = 2,
                    Offset = 2,
                    Row = 1,
                }
            };
            HanyCo.Infra.UI.ViewModels.UiComponentActionViewModel cancelButton = new()
            {
                Caption = "Cancel",
                EventHandlerName = "CancelButton_OnClick",
                Guid = Guid.NewGuid(),
                IsEnabled = true,
                Name = "CancelButton",
                TriggerType = TriggerType.FormButton,
                Position = new()
                {
                    Col = 2,
                    Offset = 1
                }
            };
            data.ViewModel.BlazorDetailsComponentViewModel.UiActions.Add(saveButton);
            data.ViewModel.BlazorDetailsComponentViewModel.UiActions.Add(cancelButton);
        }
    }

    private Task CreateBlazorListComponent(CreationData data, CancellationToken token)
    {
        var name = StringHelper.Pluralize(CommonHelper.Purify(data.ViewModel.SourceDto.Name));
        return TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(addActions)
            .RunAsync(token);

        void createViewModel(CreationData data)
        {
            data.ViewModel.BlazorListComponentViewModel = this._blazorComponentCodingService.CreateViewModel(data.ViewModel.SourceDto);
            data.ViewModel.BlazorListComponentViewModel.Name = $"{name}ListComponent";
            data.ViewModel.BlazorListComponentViewModel.ClassName = $"{name}ListComponent";
            data.ViewModel.BlazorListComponentViewModel.IsGrid = true;
            data.ViewModel.BlazorListComponentViewModel.PageDataContext = data.ViewModel.BlazorPageViewModel.DataContext;
            data.ViewModel.BlazorListComponentViewModel.PageDataContextProperty = data.ViewModel.BlazorPageViewModel.DataContext.Properties.First(x => x.IsList == true);
            data.ViewModel.BlazorPageViewModel.Components.Add(data.ViewModel.BlazorListComponentViewModel);
        }

        void addActions(CreationData data)
        {
            HanyCo.Infra.UI.ViewModels.UiComponentActionViewModel newButton = new()
            {
                Caption = "New",
                EventHandlerName = "NewButton_OnClick",
                Guid = Guid.NewGuid(),
                Name = "NewButton",
                TriggerType = TriggerType.FormButton,
                Description = $"Creates new {name}"
            };
            HanyCo.Infra.UI.ViewModels.UiComponentActionViewModel editButton = new()
            {
                Caption = "Edit",
                EventHandlerName = "Edit",
                Guid = Guid.NewGuid(),
                Name = "EditButton",
                TriggerType = TriggerType.RowButton,
                Description = $"Edits selected {name}"
            };
            HanyCo.Infra.UI.ViewModels.UiComponentActionViewModel deleteButton = new()
            {
                Caption = "Delete",
                EventHandlerName = "Delete",
                Guid = Guid.NewGuid(),
                Name = "DeleteButton",
                TriggerType = TriggerType.RowButton,
                Description = $"Deletes selected {name}"
            };
            data.ViewModel.BlazorListComponentViewModel.UiActions.AddRange(new[] { newButton, editButton, deleteButton });
        }
    }

    private Task CreateBlazorPage(CreationData data, CancellationToken token)
    {
        createPageViewModel(data);
        return Task.CompletedTask;

        void createPageViewModel(CreationData data) =>
            data.ViewModel.BlazorPageViewModel = this._blazorPageService.CreateViewModel(data.ViewModel.SourceDto);
    }

    private Task CreateDeleteCommand(CreationData data, CancellationToken token)
    {
        var name = $"Delete{CommonHelper.Purify(data.SourceDtoName)}";
        return TaskRunner.StartWith(data)
            .Then(createHandler)
            .Then(createParams)
            .Then(createValidator)
            .Then(createResult)
            .RunAsync(token);

        async Task createHandler(CancellationToken token)
        {
            data.ViewModel.DeleteCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.DeleteCommandViewModel.Name = $"{name}Command";
            data.ViewModel.DeleteCommandViewModel.Category = CqrsSegregateCategory.Delete;
            data.ViewModel.DeleteCommandViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.DeleteCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.DeleteCommandViewModel.FriendlyName = data.ViewModel.DeleteCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.DeleteCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.DeleteCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.DeleteCommandViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.DeleteCommandViewModel.ParamsDto.Name = $"{name}Params";
            data.ViewModel.DeleteCommandViewModel.ParamsDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.DeleteCommandViewModel.ParamsDto.IsParamsDto = true;
            data.ViewModel.DeleteCommandViewModel.ParamsDto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
        }

        void createResult(CreationData data)
        {
            data.ViewModel.DeleteCommandViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.DeleteCommandViewModel.ResultDto.Name = $"{name}Result";
            data.ViewModel.DeleteCommandViewModel.ResultDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.DeleteCommandViewModel.ResultDto.IsResultDto = true;
        }
        Task createValidator(CancellationToken token) =>
            Task.CompletedTask;
    }

    private async Task CreateGetAllQuery(CreationData data, CancellationToken token)
    {
        var name = $"GetAll{StringHelper.Pluralize(CommonHelper.Purify(data.SourceDtoName))}";
        var result = await TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(createParams)
            .Then(createResult)
            .RunAsync(token);

        async Task createViewModel(CreationData data, CancellationToken token)
        {
            data.ViewModel.GetAllQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetAllQueryViewModel.Name = $"{name}Query";
            data.ViewModel.GetAllQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetAllQueryViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries");
            data.ViewModel.GetAllQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetAllQueryViewModel.FriendlyName = data.ViewModel.GetAllQueryViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetAllQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetAllQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.GetAllQueryViewModel.ParamsDto.Name = $"{name}Params";
            data.ViewModel.GetAllQueryViewModel.ParamsDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.GetAllQueryViewModel.ParamsDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetAllQueryViewModel.ResultDto.Name = $"{name}Result";
            data.ViewModel.GetAllQueryViewModel.ResultDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsList = true;
        }
    }

    private Task CreateGetByIdQuery(CreationData data, CancellationToken token)
    {
        var name = $"GetById{CommonHelper.Purify(data.SourceDtoName)}";
        return TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(createParams)
            .Then(createResult)
            .RunAsync(token);

        async Task createViewModel(CancellationToken token)
        {
            data.ViewModel.GetByIdQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetByIdQueryViewModel.Name = $"{name}Query";
            data.ViewModel.GetByIdQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetByIdQueryViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Queries");
            data.ViewModel.GetByIdQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetByIdQueryViewModel.FriendlyName = data.ViewModel.GetByIdQueryViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetByIdQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetByIdQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParams()
        {
            data.ViewModel.GetByIdQueryViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.Name = $"{name}Params";
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.IsParamsDto = true;
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long });
        }

        void createResult()
        {
            data.ViewModel.GetByIdQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetByIdQueryViewModel.ResultDto.Name = $"{name}Result";
            data.ViewModel.GetByIdQueryViewModel.ResultDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.GetByIdQueryViewModel.ResultDto.IsResultDto = true;
        }
    }

    private Task CreateInsertCommand(CreationData data, CancellationToken token)
    {
        var name = $"Insert{CommonHelper.Purify(data.SourceDtoName)}";
        return TaskRunner.StartWith(data)
            .Then(createHandler)
            .Then(createParams)
            .Then(createValidator)
            .Then(createResult)
            .RunAsync(token);

        static Task createValidator(CreationData data, CancellationToken token) => Task.CompletedTask;

        async Task createHandler(CreationData data, CancellationToken token)
        {
            data.ViewModel.InsertCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.InsertCommandViewModel.Name = $"{name}Command";
            data.ViewModel.InsertCommandViewModel.Category = CqrsSegregateCategory.Create;
            data.ViewModel.InsertCommandViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.InsertCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.InsertCommandViewModel.FriendlyName = data.ViewModel.InsertCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.InsertCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.InsertCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.InsertCommandViewModel.ParamsDto = RawDto(data, true);
            data.ViewModel.InsertCommandViewModel.ParamsDto.Name = $"{name}Params";
            data.ViewModel.InsertCommandViewModel.ParamsDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.InsertCommandViewModel.ParamsDto.IsParamsDto = true;
            var idProp = data.ViewModel.InsertCommandViewModel.ParamsDto.Properties.FirstOrDefault(y => y.Name?.EqualsTo("id") is true);
            if (idProp != null)
            {
                _ = data.ViewModel.InsertCommandViewModel.ParamsDto.Properties.Remove(idProp);
            }
        }

        void createResult(CreationData data)
        {
            data.ViewModel.InsertCommandViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.InsertCommandViewModel.ResultDto.Name = $"{name}Result";
            data.ViewModel.InsertCommandViewModel.ResultDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.InsertCommandViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.InsertCommandViewModel.ResultDto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
        }
    }

    private Task CreateUpdateCommand(CreationData data, CancellationToken token)
    {
        var name = $"Update{CommonHelper.Purify(data.SourceDtoName)}";
        return TaskRunner.StartWith(data)
            .Then(createHandler)
            .Then(createParamsAsync)
            .Then(createResult)
            .Then(createValidator)
            .RunAsync(token);

        static Task createValidator(CreationData data, CancellationToken token) =>
            Task.CompletedTask;

        async Task createHandler(CreationData data, CancellationToken token)
        {
            data.ViewModel.UpdateCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.UpdateCommandViewModel.Name = $"{name}Command";
            data.ViewModel.UpdateCommandViewModel.Category = CqrsSegregateCategory.Update;
            data.ViewModel.UpdateCommandViewModel.CqrsNameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Commands");
            data.ViewModel.UpdateCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.UpdateCommandViewModel.FriendlyName = data.ViewModel.UpdateCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.UpdateCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.UpdateCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParamsAsync(CreationData data)
        {
            data.ViewModel.UpdateCommandViewModel.ParamsDto = RawDto(data, true);
            data.ViewModel.UpdateCommandViewModel.ParamsDto.Name = $"{name}Params";
            data.ViewModel.UpdateCommandViewModel.ParamsDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.UpdateCommandViewModel.ParamsDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.UpdateCommandViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.UpdateCommandViewModel.ResultDto.Name = $"{name}Result";
            data.ViewModel.UpdateCommandViewModel.ResultDto.NameSpace = TypePath.Combine(data.ViewModel.NameSpace, "Dtos");
            data.ViewModel.UpdateCommandViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.UpdateCommandViewModel.ResultDto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
        }
    }

    private sealed class CreationData(FunctionalityViewModel result, string sourceDtoName, CancellationTokenSource tokenSource)
    {
        internal readonly string COMMENT = "Auto-generated by Functionality Service.";
        private Result<FunctionalityViewModel>? _result;

        [NotNull]
        internal CancellationTokenSource CancellationTokenSource { get; } = tokenSource;

        [NotNull]
        internal Result<FunctionalityViewModel> Result => this._result ??= new(this.ViewModel);

        internal string? SourceDtoName { get; } = sourceDtoName;

        [NotNull]
        internal FunctionalityViewModel ViewModel { get; } = result.ArgumentNotNull();
    }
}