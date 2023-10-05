using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Results;
using Library.Threading;
using Library.Threading.MultistepProgress;
using Library.Validations;

using Services.Helpers;

namespace Services;

internal sealed partial class FunctionalityService
{
    public async Task<Result<Codes>> GenerateCodesAsync(FunctionalityViewModel viewModel, FunctionalityCodeServiceAsyncCodeGeneratorArgs? args = null, CancellationToken token = default)
    {
        // Check if viewModel is not null.
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

            // Generate codes for SourceDto if available.
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

            // Generate codes for BlazorListPageViewModel if available.
            if (viewModel.BlazorListPageViewModel != null)
            {
                var codeGenRes = addToList(this._blazorPageCodeService.GenerateCodes(viewModel.BlazorListPageViewModel));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.BlazorListPageCodes = codeGenRes;
            }

            // Generate codes for BlazorListPageDataContext if available.
            if (viewModel.BlazorListPageViewModel?.DataContext != null)
            {
                var codeGenRes = addToList(this._dtoCodeService.GenerateCodes(viewModel.BlazorListPageViewModel.DataContext));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.BlazorListPageDataContextCodes = codeGenRes;
            }

            // Generate codes for BlazorDetailsPageViewModel if available.
            if (viewModel.BlazorDetailsPageViewModel != null)
            {
                var codeGenRes = addToList(this._blazorPageCodeService.GenerateCodes(viewModel.BlazorDetailsPageViewModel));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.BlazorDetailsPageCodes = codeGenRes;
            }

            // Generate codes for BlazorDetailsPageDataContext if available.
            if (viewModel.BlazorDetailsPageViewModel?.DataContext != null)
            {
                var codeGenRes = addToList(this._dtoCodeService.GenerateCodes(viewModel.BlazorDetailsPageViewModel.DataContext));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.BlazorListPageDataContextCodes = codeGenRes;
            }

            // Generate codes for BlazorListComponentViewModel if available.
            if (viewModel.BlazorListComponentViewModel != null)
            {
                var codeGenRes = addToList(this._blazorComponentCodeService.GenerateCodes(viewModel.BlazorListComponentViewModel));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.BlazorListComponentCodes = codeGenRes;
            }

            // Generate codes for BlazorDetailsComponentViewModel if available.
            if (viewModel.BlazorDetailsComponentViewModel != null)
            {
                var codeGenRes = addToList(this._blazorComponentCodeService.GenerateCodes(viewModel.BlazorDetailsComponentViewModel));
                if (!codeGenRes)
                {
                    return result;
                }

                codes.BlazorDetailsComponentCodes = codeGenRes;
            }
            // TODO Fix this
            //if (true)
            //{
            //    var codeGenRes = addToList(this._converterCodeService.GenerateCode(viewModel.SourceDto!, viewModel.GetAllQueryViewModel!.ResultDto.Name!, $"{viewModel.SourceDto!.Name}", null));
            //    if (!codeGenRes)
            //    {
            //        return result;
            //    }
            //    codes.ConverterCodes = codeGenRes;
            //}

            return result;

            // Internal method to add a code result to the result list.
            [DebuggerStepThrough]
            Result<Codes> addToList(Result<Codes> codeResult)
            {
                result.Add(codeResult);
                return codeResult;
            }

            async Task<List<Result<Codes>>> gatherAllCodesAsync(CqrsViewModelBase cqrsViewModel, CancellationToken token)
            {
                var getAllCodes = new List<Result<Codes>>
                {
                    // Generate the codes of CQRS parameters.
                    addToList(this._dtoCodeService.GenerateCodes(cqrsViewModel.ParamsDto)),
                    // Generate the codes of CQRS result.
                    addToList(this._dtoCodeService.GenerateCodes(cqrsViewModel.ResultDto)),
                    // Generate the codes of CQRS handler.
                    addToList(await this._cqrsCodeService.GenerateCodesAsync(cqrsViewModel, token: token))
                };
                return getAllCodes;
            }
        }
    }

    public async Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync(FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        // Validate the input viewModel
        if (!this.Validate(viewModel).TryParse(out var validationResult))
        {
            return validationResult!;
        }

        // Report the initialization progress
        this._reporter.Report(description: getTitle("Initializing..."));

        // Initialize the viewModel and check if it fails
        var initResult = initialize(viewModel, token);
        if (!initResult.IsSucceed)
        {
            return Result<FunctionalityViewModel>.From(initResult, viewModel)!;
        }

        // Get the initialized data and token source
        var (data, tokenSource) = initResult.GetValue();

        // Perform the initialization steps
        var process = initSteps(data);

        // Report the running progress
        this._reporter.Report(description: getTitle("Running..."));

        // Run the process asynchronously
        var processResult = await process.RunAsync(tokenSource.Token);

        // Get the result message based on the process result and cancellation token
        var message = getResultMessage(processResult, tokenSource.Token);

        // Report the result message
        this._reporter.Report(description: getTitle(message));

        // Get the final result
        var result = processResult.Value.Result;

        // Dispose the token source
        tokenSource.Dispose();

        // Return the final result
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
            result.ViewModel.SourceDto.NameSpace = TypePath.Combine(result.ViewModel.SourceDto.NameSpace, result.ViewModel.SourceDto.Module.Name!.Remove(" "));

            // Return a success result with the result and cancellationTokenSource
            return Result<(CreationData, CancellationTokenSource)>.CreateSuccess((result, tokenSource));
        }

        // Initialize the steps for the process
        MultistepProcessRunner<CreationData> initSteps(in CreationData data) =>
        //?! Don't change the sequence of steps !
            MultistepProcessRunner<CreationData>.New(data, this._reporter, owner: nameof(FunctionalityService))
                .AddStep(this.CreateGetAllQuery, getTitle($"Creating `GetAll{StringHelper.Pluralize(data.ViewModel.Name)}Query`…"))
                .AddStep(this.CreateGetByIdQuery, getTitle($"Creating `GetById{data.ViewModel.Name}Query`…"))

                .AddStep(this.CreateInsertCommand, getTitle($"Creating `Insert{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateUpdateCommand, getTitle($"Creating `Update{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateDeleteCommand, getTitle($"Creating `Delete{data.ViewModel.Name}Command`…"))

                .AddStep(this.CreateBlazorListPage, getTitle($"Creating {data.ViewModel.Name} Blazor List Page…"))
                .AddStep(this.CreateBlazorDetailsPage, getTitle($"Creating {data.ViewModel.Name} Blazor Details Page…"))
                .AddStep(this.CreateBlazorListComponent, getTitle($"Creating Blazor `{data.ViewModel.Name}ListComponent`…"))
                .AddStep(this.CreateBlazorDetailsComponent, getTitle($"Creating Blazor `{data.ViewModel.Name}DetailsComponent`…"));

        // Finalize the process
        static string getResultMessage(in Result result, CancellationToken token)
        {
            if (!result.Message.IsNullOrEmpty())
            {
                return result.Message;
            }
            if (result.IsFailure)
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

    private static IEnumerable<ClaimViewModel> GetClaimViewModels(CreationData data, InfraViewModelBase model) =>
        data.ViewModel.SourceDto.SecurityClaims?.Any() ?? false
            ? data.ViewModel.SourceDto.SecurityClaims.Select(x => new ClaimViewModel(model.Name, null, x))
            : Enumerable.Empty<ClaimViewModel>();

    private static string GetNameSpace(CreationData data) =>
        data.ViewModel.SourceDto.NameSpace;

    private static DtoViewModel RawDto(CreationData data, bool addTableColumns = false)
    {
        // Create an initial DTO based on the input data
        var dto = new DtoViewModel(data.ViewModel.SourceDto.Id, data.ViewModel.Name!)
        {
            Comment = data.COMMENT, // Set DTO comment
            NameSpace = TypePath.Combine(GetNameSpace(data), "Dtos"), // Set DTO namespace
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
        var name = CommonHelpers.Purify(data.SourceDtoName);
        return TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(addActions)
            .RunAsync(token);

        void createViewModel(CreationData data)
        {
            data.ViewModel.BlazorDetailsComponentViewModel = this._blazorComponentService.CreateViewModel(data.ViewModel.SourceDto);
            data.ViewModel.BlazorDetailsComponentViewModel.Name = $"{name}DetailsComponent";
            data.ViewModel.BlazorDetailsComponentViewModel.ClassName = $"{name}DetailsComponent";
            data.ViewModel.BlazorDetailsComponentViewModel.IsGrid = false;
            data.ViewModel.BlazorDetailsComponentViewModel.PageDataContext = data.ViewModel.BlazorDetailsPageViewModel.DataContext;
            data.ViewModel.BlazorDetailsComponentViewModel.PageDataContextProperty = data.ViewModel.BlazorDetailsPageViewModel.DataContext.Properties.First(x => x.IsList != true);
            data.ViewModel.BlazorDetailsPageViewModel.Components.Add(data.ViewModel.BlazorDetailsComponentViewModel);
        }

        static void addActions(CreationData data)
        {
            data.ViewModel.BlazorListPageViewModel.Route = BlazorPage.GetPageRoute(CommonHelpers.Purify(data.ViewModel.SourceDto.Name!), data.ViewModel.SourceDto.Module.Name, null);
            // The Save button
            var saveButton = new UiComponentCqrsButtonViewModel()
            {
                Caption = "Save",
                CqrsSegregate = data.ViewModel.InsertCommandViewModel,
                EventHandlerName = "SaveButton_OnClick",
                Guid = Guid.NewGuid(),
                IsEnabled = true,
                Name = "SaveButton",
                Placement = Placement.FormButton,
                Description = "Save the data to database",
                Position = new()
                {
                    Col = 2,
                    Offset = 2,
                    Row = 1,
                }
            };
            // The Back button. Same as the cancel button.
            var cancelButton = new UiComponentCustomButtonViewModel()
            {
                Caption = "Back",
                CodeStatement = $"this._navigationManager.NavigateTo({data.ViewModel.BlazorListPageViewModel.Route.TrimStart("@page").Trim()});",
                EventHandlerName = "BackButton_OnClick",
                Guid = Guid.NewGuid(),
                IsEnabled = true,
                Name = "BackButton",
                Placement = Placement.FormButton,
                Position = new()
                {
                    Col = 2,
                    Offset = 1
                }
            };
            data.ViewModel.BlazorDetailsComponentViewModel.Actions.Add(saveButton);
            data.ViewModel.BlazorDetailsComponentViewModel.Actions.Add(cancelButton);
        }
    }

    private Task CreateBlazorDetailsPage(CreationData data, CancellationToken token)
    {
        var name = CommonHelpers.Purify(data.ViewModel.SourceDto.Name)?.AddEnd("DetailsPage");
        createPageViewModel(data);
        return Task.CompletedTask;

        void createPageViewModel(CreationData data) =>
            data.ViewModel.BlazorDetailsPageViewModel = this._blazorPageService.CreateViewModel(data.ViewModel.SourceDto)
                .With(x => x.Name = name)
                .With(x => x.ClassName = name);
    }

    private Task CreateBlazorListComponent(CreationData data, CancellationToken token)
    {
        var name = StringHelper.Pluralize(CommonHelpers.Purify(data.SourceDtoName));
        return TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(addActions)
            .RunAsync(token);

        void createViewModel(CreationData data)
        {
            data.ViewModel.BlazorListComponentViewModel = this._blazorComponentService.CreateViewModel(data.ViewModel.SourceDto);
            data.ViewModel.BlazorListComponentViewModel.Name = $"{name}ListComponent";
            data.ViewModel.BlazorListComponentViewModel.ClassName = $"{name}ListComponent";
            data.ViewModel.BlazorListComponentViewModel.IsGrid = true;
            data.ViewModel.BlazorListComponentViewModel.PageDataContext = data.ViewModel.BlazorListPageViewModel.DataContext;
            data.ViewModel.BlazorListComponentViewModel.PageDataContextProperty = data.ViewModel.BlazorListPageViewModel.DataContext.Properties.First(x => x.IsList == true);
            data.ViewModel.BlazorListPageViewModel.Components.Add(data.ViewModel.BlazorListComponentViewModel);
        }

        void addActions(CreationData data)
        {
            var route = BlazorPage.GetPageRoute(CommonHelpers.Purify(data.ViewModel.SourceDto.Name!), data.ViewModel.SourceDto.Module.Name, null);
            data.ViewModel.BlazorDetailsPageViewModel.Route = route.Insert(route.Length - 1, "/details");

            var newButton = new UiComponentCustomButtonViewModel
            {
                CodeStatement = $"this._navigationManager.NavigateTo({data.ViewModel.BlazorDetailsPageViewModel.Route.TrimStart("@page").Trim()});",
                Caption = "New",
                EventHandlerName = "NewButton_OnClick",
                Guid = Guid.NewGuid(),
                Name = "NewButton",
                Placement = Placement.FormButton,
                Description = $"Creates new {name}"
            };
            var editButton = new UiComponentCustomButtonViewModel
            {
                CodeStatement = $"this._navigationManager.NavigateTo({data.ViewModel.BlazorDetailsPageViewModel.Route.TrimStart("@page").Trim()} + \"/\" + id.ToString());",
                Caption = "Edit",
                EventHandlerName = "Edit",
                Guid = Guid.NewGuid(),
                Name = "EditButton",
                Placement = Placement.RowButton,
                Description = $"Edits selected {name}"
            };
            var deleteButton = new UiComponentCqrsButtonViewModel
            {
                CqrsSegregate = data.ViewModel.DeleteCommandViewModel,
                Caption = "Delete",
                EventHandlerName = "Delete",
                Guid = Guid.NewGuid(),
                Name = "DeleteButton",
                Placement = Placement.RowButton,
                Description = $"Deletes selected {name}"
            };
            var onLoad = new UiComponentCqrsLoadViewModel
            {
                CqrsSegregate = data.ViewModel.GetAllQueryViewModel
            };
            _ = data.ViewModel.BlazorListComponentViewModel.Actions.AddRange(new IUiComponentContent[] { newButton, editButton, deleteButton, onLoad });
        }
    }

    private Task CreateBlazorListPage(CreationData data, CancellationToken token)
    {
        var name = CommonHelpers.Purify(data.ViewModel.SourceDto.Name)?.AddEnd("ListPage");
        createPageViewModel(data);
        return Task.CompletedTask;

        void createPageViewModel(CreationData data) =>
            data.ViewModel.BlazorListPageViewModel = this._blazorPageService.CreateViewModel(data.ViewModel.SourceDto)
                .With(x => x.Name = name)
                .With(x => x.ClassName = name);
    }

    private Task CreateDeleteCommand(CreationData data, CancellationToken token)
    {
        var name = $"Delete{CommonHelpers.Purify(data.SourceDtoName)}";
        return TaskRunner.StartWith(data)
            .Then(createHandler)
            .Then(createParams)
            .Then(createValidator)
            .Then(createResult)
            .Then(setupSecurity)
            .RunAsync(token);

        async Task createHandler(CancellationToken token)
        {
            data.ViewModel.DeleteCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.DeleteCommandViewModel.Name = $"{name}Command";
            data.ViewModel.DeleteCommandViewModel.Category = CqrsSegregateCategory.Delete;
            data.ViewModel.DeleteCommandViewModel.CqrsNameSpace = TypePath.Combine(GetNameSpace(data), "Commands");
            data.ViewModel.DeleteCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.DeleteCommandViewModel.FriendlyName = data.ViewModel.DeleteCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.DeleteCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.DeleteCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.DeleteCommandViewModel.ParamsDto = RawDto(data, true);
            data.ViewModel.DeleteCommandViewModel.ParamsDto.Name = $"{name}Params";
            data.ViewModel.DeleteCommandViewModel.ParamsDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.DeleteCommandViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.DeleteCommandViewModel.ResultDto.Name = $"{name}Result";
            data.ViewModel.DeleteCommandViewModel.ResultDto.IsResultDto = true;
        }

        Task createValidator(CancellationToken token) =>
            Task.CompletedTask;

        void setupSecurity(CreationData data) =>
            data.ViewModel.DeleteCommandViewModel.SecurityClaims = GetClaimViewModels(data, data.ViewModel.DeleteCommandViewModel);
    }

    private async Task CreateGetAllQuery(CreationData data, CancellationToken token)
    {
        var name = $"GetAll{StringHelper.Pluralize(CommonHelpers.Purify(data.SourceDtoName))}";
        var result = await TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(createParams)
            .Then(createResult)
            .Then(setupSecurity)
            .RunAsync(token);

        async Task createViewModel(CreationData data, CancellationToken token)
        {
            data.ViewModel.GetAllQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetAllQueryViewModel.Name = $"{name}Query";
            data.ViewModel.GetAllQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetAllQueryViewModel.CqrsNameSpace = TypePath.Combine(GetNameSpace(data), "Queries");
            data.ViewModel.GetAllQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetAllQueryViewModel.FriendlyName = data.ViewModel.GetAllQueryViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetAllQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetAllQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.GetAllQueryViewModel.ParamsDto.Name = $"{name}Params";
            data.ViewModel.GetAllQueryViewModel.ParamsDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetAllQueryViewModel.ResultDto.Name = $"{name}Result";
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsList = true;
        }

        void setupSecurity(CreationData data) =>
            data.ViewModel.GetAllQueryViewModel.SecurityClaims = GetClaimViewModels(data, data.ViewModel.GetAllQueryViewModel);
    }

    private Task CreateGetByIdQuery(CreationData data, CancellationToken token)
    {
        var name = $"GetById{CommonHelpers.Purify(data.SourceDtoName)}";
        return TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(createParams)
            .Then(createResult)
            .Then(setupSecurity)
            .RunAsync(token);

        async Task createViewModel(CancellationToken token)
        {
            data.ViewModel.GetByIdQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetByIdQueryViewModel.Name = $"{name}Query";
            data.ViewModel.GetByIdQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetByIdQueryViewModel.CqrsNameSpace = TypePath.Combine(GetNameSpace(data), "Queries");
            data.ViewModel.GetByIdQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetByIdQueryViewModel.FriendlyName = data.ViewModel.GetByIdQueryViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetByIdQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetByIdQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParams()
        {
            data.ViewModel.GetByIdQueryViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.Name = $"{name}Params";
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.IsParamsDto = true;
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.Properties.Add(new() { Comment = data.COMMENT, Name = "Id", Type = PropertyType.Long });
        }

        void createResult()
        {
            data.ViewModel.GetByIdQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetByIdQueryViewModel.ResultDto.Name = $"{name}Result";
            data.ViewModel.GetByIdQueryViewModel.ResultDto.IsResultDto = true;
        }

        void setupSecurity(CreationData data) =>
            data.ViewModel.GetByIdQueryViewModel.SecurityClaims = GetClaimViewModels(data, data.ViewModel.GetByIdQueryViewModel);
    }

    private Task CreateInsertCommand(CreationData data, CancellationToken token)
    {
        var name = $"Insert{CommonHelpers.Purify(data.SourceDtoName)}";
        return TaskRunner.StartWith(data)
            .Then(createHandler)
            .Then(createParams)
            .Then(createValidator)
            .Then(createResult)
            .Then(setupSecurity)
            .RunAsync(token);

        static Task createValidator(CreationData data, CancellationToken token) => Task.CompletedTask;

        async Task createHandler(CreationData data, CancellationToken token)
        {
            data.ViewModel.InsertCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.InsertCommandViewModel.Name = $"{name}Command";
            data.ViewModel.InsertCommandViewModel.Category = CqrsSegregateCategory.Create;
            data.ViewModel.InsertCommandViewModel.CqrsNameSpace = TypePath.Combine(GetNameSpace(data), "Commands");
            data.ViewModel.InsertCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.InsertCommandViewModel.FriendlyName = data.ViewModel.InsertCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.InsertCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.InsertCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.InsertCommandViewModel.ParamsDto = RawDto(data, true);
            data.ViewModel.InsertCommandViewModel.ParamsDto.Name = $"{name}Params";
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
            data.ViewModel.InsertCommandViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.InsertCommandViewModel.ResultDto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
        }

        void setupSecurity(CreationData data) =>
            data.ViewModel.InsertCommandViewModel.SecurityClaims = GetClaimViewModels(data, data.ViewModel.InsertCommandViewModel);
    }

    private Task CreateUpdateCommand(CreationData data, CancellationToken token)
    {
        var name = $"Update{CommonHelpers.Purify(data.SourceDtoName)}";
        return TaskRunner.StartWith(data)
            .Then(createHandler)
            .Then(createParamsAsync)
            .Then(createResult)
            .Then(createValidator)
            .Then(setupSecurity)
            .RunAsync(token);

        static Task createValidator(CreationData data, CancellationToken token) => Task.CompletedTask;

        async Task createHandler(CreationData data, CancellationToken token)
        {
            data.ViewModel.UpdateCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.UpdateCommandViewModel.Name = $"{name}Command";
            data.ViewModel.UpdateCommandViewModel.Category = CqrsSegregateCategory.Update;
            data.ViewModel.UpdateCommandViewModel.CqrsNameSpace = TypePath.Combine(GetNameSpace(data), "Commands");
            data.ViewModel.UpdateCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.UpdateCommandViewModel.FriendlyName = data.ViewModel.UpdateCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.UpdateCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.UpdateCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParamsAsync(CreationData data)
        {
            data.ViewModel.UpdateCommandViewModel.ParamsDto = RawDto(data, true);
            data.ViewModel.UpdateCommandViewModel.ParamsDto.Name = $"{name}Params";
            data.ViewModel.UpdateCommandViewModel.ParamsDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.UpdateCommandViewModel.ResultDto = RawDto(data, false);
            data.ViewModel.UpdateCommandViewModel.ResultDto.Name = $"{name}Result";
            data.ViewModel.UpdateCommandViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.UpdateCommandViewModel.ResultDto.Properties.Add(new("Id", PropertyType.Long) { Comment = data.COMMENT });
        }

        void setupSecurity(CreationData data) =>
            data.ViewModel.UpdateCommandViewModel.SecurityClaims = GetClaimViewModels(data, data.ViewModel.UpdateCommandViewModel);
    }

    private sealed class CreationData(FunctionalityViewModel result, string sourceDtoName, CancellationTokenSource tokenSource)
    {
        internal readonly string COMMENT = "Auto-generated by Functionality Service.";
        private Result<FunctionalityViewModel>? _result;

        [NotNull]
        internal CancellationTokenSource CancellationTokenSource { get; } = tokenSource.ArgumentNotNull();

        [NotNull]
        internal Result<FunctionalityViewModel> Result => this._result ??= new(this.ViewModel);

        internal string? SourceDtoName { get; } = sourceDtoName;

        [NotNull]
        internal FunctionalityViewModel ViewModel { get; } = result.ArgumentNotNull();
    }
}