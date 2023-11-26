using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Data.SqlServer;
using Library.Results;
using Library.Threading;
using Library.Threading.MultistepProgress;
using Library.Validations;

using Services.Helpers;

namespace Services;

internal sealed partial class FunctionalityService
{
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
        var (data, tokenSource) = initResult.Value;

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
        [DebuggerStepThrough]
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
        //?! ☠ Don't change the sequence of the steps ☠
            MultistepProcessRunner<CreationData>.New(data, this._reporter, owner: nameof(FunctionalityService))
                .AddStep(this.InitializeWorkspace, getTitle("Initializing"))
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
        [DebuggerStepThrough]
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

    [DebuggerStepThrough]
    private static IEnumerable<ClaimViewModel> GetClaimViewModels(CreationData data, InfraViewModelBase model) =>
        data.ViewModel.SourceDto.SecurityClaims?.Any() ?? false
            ? data.ViewModel.SourceDto.SecurityClaims.Select(x => new ClaimViewModel(model.Name, null, x))
            : Enumerable.Empty<ClaimViewModel>();

    private static string GetMapperNameSpace(CreationData data) =>
        TypePath.Combine(GetNameSpace(data), "Mapper");

    [DebuggerStepThrough]
    private static string GetNameSpace(CreationData data) =>
        data.ViewModel.SourceDto.NameSpace;

    [DebuggerStepThrough]
    private static DtoViewModel RawDto(CreationData data, bool addTableColumns = false)
    {
        // Create an initial DTO based on the input data
        var dto = new DtoViewModel(data.ViewModel.SourceDto.Id, data.ViewModel.Name!)
        {
            Comment = data.COMMENT, // Set DTO comment
            NameSpace = TypePath.Combine(GetNameSpace(data), "Dtos"), // Set DTO namespace
            Functionality = data.ViewModel,
            DbObject = data.ViewModel.SourceDto.DbObject
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

    [DebuggerStepThrough]
    private static string? ReplaceVariables(in DtoViewModel paramsDto, in string? statement, in string paramName)
    {
        if (statement.IsNullOrEmpty())
        {
            return string.Empty;
        }

        var result = statement;
        foreach (var p in paramsDto.Properties)
        {
            result = result
                .Replace($"%{p!.DbObject?.Name}%", $"{{{paramName}.{p.Name}}}")
                .Replace($"^{p!.DbObject?.Name}^", p.Name)
                ;
        }
        return result;
    }

    private Task CreateBlazorDetailsComponent(CreationData data, CancellationToken token)
    {
        var name = CommonHelpers.Purify(data.SourceDtoName);
        return TaskRunner.StartWith(data)
            .Then(createViewModel)
            .Then(addActions)
            .Then(addParameters)
            .Then(createConverters)
            .RunAsync(token);

        void createViewModel(CreationData data)
        {
            data.ViewModel.BlazorDetailsComponentViewModel = this._blazorComponentService.CreateViewModel(data.ViewModel.SourceDto);
            data.ViewModel.BlazorDetailsComponentViewModel.Name = $"{name}DetailsComponent";
            data.ViewModel.BlazorDetailsComponentViewModel.ClassName = $"{name}DetailsComponent";
            data.ViewModel.BlazorDetailsComponentViewModel.IsGrid = false;
            data.ViewModel.BlazorDetailsComponentViewModel.PageDataContext = data.ViewModel.BlazorDetailsPageViewModel.DataContext;
            data.ViewModel.BlazorDetailsComponentViewModel.PageDataContextProperty = data.ViewModel.BlazorDetailsPageViewModel.DataContext.Properties.First(x => x.IsList != true);
            data.ViewModel.BlazorDetailsComponentViewModel.Attributes.Add(new("@bind-EntityId", "this.Id"));
            data.ViewModel.BlazorDetailsComponentViewModel.AdditionalUsingNameSpaces.Add(GetMapperNameSpace(data));
            data.ViewModel.BlazorDetailsPageViewModel.Components.Add(data.ViewModel.BlazorDetailsComponentViewModel);
        }

        void addActions(CreationData data)
        {
            var pageRoute = BlazorPage.GetPageRoute(CommonHelpers.Purify(data.ViewModel.SourceDto.Name!), data.ViewModel.SourceDto.Module.Name, null);
            data.ViewModel.BlazorListPageViewModel.Routes.Add(pageRoute);
            // The Save button
            var saveButton = new UiComponentCustomButtonViewModel()
            {
                Caption = "Save",
                CodeStatement = CodeSnippets.BlazorDetailsComponent_SaveButton_OnClick_Body(data.ViewModel.InsertCommandViewModel, data.ViewModel.UpdateCommandViewModel),
                EventHandlerName = "SaveButton_OnClick",
                ReturnType = "async void",
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
                CodeStatement = CodeSnippets.NavigateTo(pageRoute.TrimStart("@page").Trim()),
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
            var onLoad = new UiComponentCustomLoadViewModel
            {
                CodeStatement = CodeSnippets.GetById_LoadMethodBody(data.ViewModel.GetByIdQueryViewModel),
            };
            data.ViewModel.BlazorDetailsComponentViewModel.Actions.Add(saveButton);
            data.ViewModel.BlazorDetailsComponentViewModel.Actions.Add(cancelButton);
            data.ViewModel.BlazorDetailsComponentViewModel.Actions.Add(onLoad);
        }

        static void addParameters(CreationData data)
        {
            data.ViewModel.BlazorDetailsComponentViewModel.Parameters.Add(new(TypePath.New("long"), "EntityId"));
            data.ViewModel.BlazorDetailsComponentViewModel.Parameters.Add(new(TypePath.New("Microsoft.AspNetCore.Components.EventCallback<long?>"), "EntityIdChanged"));
        }

        void createConverters(CreationData data)
        {
            var mapperNameSpace = GetMapperNameSpace(data);
            var dataContext = data.ViewModel.BlazorDetailsComponentViewModel.PageDataContextProperty.Dto;

            var insert = data.ViewModel.InsertCommandViewModel;
            var insertDstTypePath = insert.GetSegregateParamsType("Command");
            var args = new MapperSourceGeneratorArguments((dataContext, null), (insert.ResultDto, insertDstTypePath), mapperNameSpace, MethodName: $"To{insertDstTypePath.Name}");
            data.ViewModel.MapperGeneratorViewModel.Arguments.Add(args);

            var update = data.ViewModel.UpdateCommandViewModel;
            var updateDstTypePath = update.GetSegregateParamsType("Command");
            args = new MapperSourceGeneratorArguments((dataContext, null), (update.ResultDto, updateDstTypePath), mapperNameSpace, MethodName: $"To{updateDstTypePath.Name}");
            data.ViewModel.MapperGeneratorViewModel.Arguments.Add(args);

            data.ViewModel.BlazorDetailsComponentViewModel.AdditionalUsingNameSpaces.Add(GetMapperNameSpace(data));
        }
    }

    private Task CreateBlazorDetailsPage(CreationData data, CancellationToken token)
    {
        var name = CommonHelpers.Purify(data.ViewModel.SourceDto.Name)?.AddEnd("DetailsPage");
        return TaskRunner.StartWith(data)
            .Then(createPageViewModel)
            .Then(addParameters)
            .RunAsync(token);

        void createPageViewModel(CreationData data) =>
            data.ViewModel.BlazorDetailsPageViewModel = this._blazorPageService.CreateViewModel(data.ViewModel.SourceDto)
                .With(x => x.Name = name)
                .With(x => x.ClassName = name);
        static void addParameters(CreationData data) =>
            data.ViewModel.BlazorDetailsPageViewModel.Parameters.Add(new(TypePath.New<long>(), "Id"));
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
            data.ViewModel.BlazorListComponentViewModel.AdditionalUsingNameSpaces.Add(GetMapperNameSpace(data));
            data.ViewModel.BlazorListPageViewModel.Components.Add(data.ViewModel.BlazorListComponentViewModel);
        }

        void addActions(CreationData data)
        {
            var pageName = $"{CommonHelpers.Purify(data.ViewModel.SourceDto.Name!)}/details";
            var pureRoute = BlazorPage.GetPageRoute(pageName, data.ViewModel.SourceDto.Module.Name, null);
            var routeWithId = BlazorPage.GetPageRoute(pageName, data.ViewModel.SourceDto.Module.Name, null, "{Id:long}");
            data.ViewModel.BlazorDetailsPageViewModel.Routes.AddRange(pureRoute, routeWithId);

            var newButton = new UiComponentCustomButtonViewModel
            {
                CodeStatement = CodeSnippets.NavigateTo(pureRoute.TrimStart("@page").Trim()),
                Caption = "New",
                EventHandlerName = "NewButton_OnClick",
                Guid = Guid.NewGuid(),
                Name = "NewButton",
                Placement = Placement.FormButton,
                Description = $"Creates new {name}"
            };
            var editButton = new UiComponentCustomButtonViewModel
            {
                CodeStatement = CodeSnippets.NavigateTo($"${pureRoute.TrimStart("@page").TrimEnd("\"").Trim()}/{{id.ToString()}}\""), //$"this._navigationManager.NavigateTo(${pureRoute.TrimStart("@page").TrimEnd("\"").Trim()}/{{id.ToString()}}\");",
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
            data.ViewModel.BlazorListComponentViewModel.Actions.AddRange(new IUiComponentContent[] { newButton, editButton, deleteButton, onLoad });
        }
    }

    private Task CreateBlazorListPage(CreationData data, CancellationToken token)
    {
        var name = CommonHelpers.Purify(data.ViewModel.SourceDto.Name)?.AddEnd("ListPage");
        return TaskRunner.StartWith(data)
            .Then(createPageViewModel)
            .RunAsync(token);

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
            data.ViewModel.DeleteCommandViewModel.DtoNameSpace = TypePath.Combine(GetNameSpace(data), "Dtos");
            data.ViewModel.DeleteCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.DeleteCommandViewModel.FriendlyName = data.ViewModel.DeleteCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.DeleteCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.DeleteCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
            data.ViewModel.DeleteCommandViewModel.MapperNameSpace = GetMapperNameSpace(data);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.DeleteCommandViewModel.ParamsDto = RawDto(data, true);
            data.ViewModel.DeleteCommandViewModel.ParamsDto.Name = $"{name}";
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
            .Then(createHandleMethodBody)
            .Then(setupSecurity)
            .RunAsync(token);

        async Task createViewModel(CreationData data, CancellationToken token)
        {
            data.ViewModel.GetAllQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetAllQueryViewModel.Name = $"{name}Query";
            data.ViewModel.GetAllQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetAllQueryViewModel.CqrsNameSpace = TypePath.Combine(GetNameSpace(data), "Queries");
            data.ViewModel.GetAllQueryViewModel.DtoNameSpace = TypePath.Combine(GetNameSpace(data), "Dtos");
            data.ViewModel.GetAllQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetAllQueryViewModel.FriendlyName = data.ViewModel.GetAllQueryViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetAllQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetAllQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
            data.ViewModel.GetAllQueryViewModel.MapperNameSpace = GetMapperNameSpace(data);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.GetAllQueryViewModel.ParamsDto.Name = $"{name}";
            data.ViewModel.GetAllQueryViewModel.ParamsDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.GetAllQueryViewModel.ResultDto = RawDto(data, true);
            data.ViewModel.GetAllQueryViewModel.ResultDto.Name = $"{name}Result";
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsResultDto = true;
            data.ViewModel.GetAllQueryViewModel.ResultDto.IsList = true;
        }
        static void createHandleMethodBody(CreationData data) =>
            data.ViewModel.GetAllQueryViewModel.HandleMethodBody = CodeSnippets.CreateGetAllQueryHandleMethodBody(data.ViewModel.GetAllQueryViewModel);

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
            .Then(createHandleMethodBody)
            .Then(setupSecurity)
            .Then(createConverters)
            .RunAsync(token);

        async Task createViewModel(CancellationToken token)
        {
            data.ViewModel.GetByIdQueryViewModel = await this._queryService.CreateAsync(token: token);
            data.ViewModel.GetByIdQueryViewModel.Name = $"{name}Query";
            data.ViewModel.GetByIdQueryViewModel.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetByIdQueryViewModel.CqrsNameSpace = TypePath.Combine(GetNameSpace(data), "Queries");
            data.ViewModel.GetByIdQueryViewModel.DtoNameSpace = TypePath.Combine(GetNameSpace(data), "Dtos");
            data.ViewModel.GetByIdQueryViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetByIdQueryViewModel.FriendlyName = data.ViewModel.GetByIdQueryViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetByIdQueryViewModel.Comment = data.COMMENT;
            data.ViewModel.GetByIdQueryViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParams()
        {
            data.ViewModel.GetByIdQueryViewModel.ParamsDto = RawDto(data, false);
            data.ViewModel.GetByIdQueryViewModel.ParamsDto.Name = $"{name}";
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

        static void createHandleMethodBody(CreationData data) =>
            data.ViewModel.GetByIdQueryViewModel.HandleMethodBody = CodeSnippets.CreateGetByIdQueryHandleMethodBody(data.ViewModel.GetByIdQueryViewModel);

        void createConverters(CreationData data)
        {
            var mapperNameSpace = GetMapperNameSpace(data);
            var getByIdQueryViewModel = data.ViewModel.GetByIdQueryViewModel;
            var args = new MapperSourceGeneratorArguments(getByIdQueryViewModel.ResultDto, getByIdQueryViewModel.ResultDto, mapperNameSpace);
            data.ViewModel.MapperGeneratorViewModel.Arguments.Add(args);
        }
    }

    private Task CreateInsertCommand(CreationData data, CancellationToken token)
    {
        var name = $"Insert{CommonHelpers.Purify(data.SourceDtoName)}";
        return TaskRunner.StartWith(data)
            .Then(createHandler)
            .Then(createParams)
            .Then(createResult)
            .Then(createValidator)
            .Then(setupSecurity)
            .RunAsync(token);

        static Task createValidator(CreationData data, CancellationToken token) => Task.CompletedTask;

        async Task createHandler(CreationData data, CancellationToken token)
        {
            data.ViewModel.InsertCommandViewModel = await this._commandService.CreateAsync(token);
            data.ViewModel.InsertCommandViewModel.Name = $"{name}Command";
            data.ViewModel.InsertCommandViewModel.Category = CqrsSegregateCategory.Create;
            data.ViewModel.InsertCommandViewModel.CqrsNameSpace = TypePath.Combine(GetNameSpace(data), "Commands");
            data.ViewModel.InsertCommandViewModel.DtoNameSpace = TypePath.Combine(GetNameSpace(data), "Dtos");
            data.ViewModel.InsertCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.InsertCommandViewModel.FriendlyName = data.ViewModel.InsertCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.InsertCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.InsertCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.InsertCommandViewModel.ParamsDto = RawDto(data, true);
            data.ViewModel.InsertCommandViewModel.ParamsDto.Name = $"{name}";
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
            data.ViewModel.UpdateCommandViewModel.DtoNameSpace = TypePath.Combine(GetNameSpace(data), "Dtos");
            data.ViewModel.UpdateCommandViewModel.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.UpdateCommandViewModel.FriendlyName = data.ViewModel.UpdateCommandViewModel.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.UpdateCommandViewModel.Comment = data.COMMENT;
            data.ViewModel.UpdateCommandViewModel.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token: token);
        }

        void createParamsAsync(CreationData data)
        {
            data.ViewModel.UpdateCommandViewModel.ParamsDto = RawDto(data, true);
            data.ViewModel.UpdateCommandViewModel.ParamsDto.Name = $"{name}";
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

    private Task InitializeWorkspace(CreationData data, CancellationToken token)
    {
        data.ViewModel.MapperGeneratorViewModel.Arguments.Clear();
        data.ViewModel.Codes.Clear();
        return Task.CompletedTask;
    }

    private class CodeSnippets
    {
        public static string BlazorDetailsComponent_SaveButton_OnClick_Body(CqrsCommandViewModel insert, CqrsCommandViewModel update) =>
            new StringBuilder()
                .AppendLine($"if (DataContext.Id == default)")
                .AppendLine($"{{")
                .AppendLine($"    var @params = this.DataContext.To{insert.GetSegregateParamsType("Command").Name}();")
                .AppendLine($"    var cqParams = new {insert.GetSegregateType("Command").FullPath}(@params);")
                .AppendLine($"    var cqResult = await this._commandProcessor.ExecuteAsync<{insert.GetSegregateType("Command").FullPath}, {insert.GetSegregateResultType("Command").FullPath}>(cqParams);")
                .AppendLine($"}}")
                .AppendLine($"else")
                .AppendLine($"{{")
                .AppendLine($"    var @params = this.DataContext.To{update.GetSegregateParamsType("Command").Name}();")
                .AppendLine($"    var cqParams = new {update.GetSegregateType("Command").FullPath}(@params);")
                .AppendLine($"    var cqResult = await this._commandProcessor.ExecuteAsync<{update.GetSegregateType("Command").FullPath}, {update.GetSegregateResultType("Command").FullPath}>(cqParams);")
                .AppendLine($"}}")
                .Build();

        public static string CreateGetAllQueryHandleMethodBody(CqrsQueryViewModel model) =>
            CreateQueryHandleMethodBody(model);

        public static string CreateGetByIdQueryHandleMethodBody(CqrsQueryViewModel model) =>
            CreateQueryHandleMethodBody(model, "[ID] = %Id%");

        [DebuggerStepThrough]
        public static string GetById_LoadMethodBody(CqrsViewModelBase cqrsViewModel) =>
            new StringBuilder()
                .AppendLine("if (this.EntityId is { } entityId)")
                .AppendLine("{")
                .AppendLine($"// Setup segregation parameters")
                .AppendLine($"var @params = new {cqrsViewModel.GetSegregateParamsType("Query").FullPath}()")
                .AppendLine("{")
                .AppendLine("    Id = entityId,")
                .AppendLine("};")
                .AppendLine("")
                .AppendLine($"var cqParams = new {cqrsViewModel.GetSegregateType("Query").FullPath}(@params);")
                .AppendLine($"// Invoke the query handler to retrieve all entities")
                .AppendLine($"var cqResult = await this._queryProcessor.ExecuteAsync<{cqrsViewModel.GetSegregateResultType("Query").FullPath}>(cqParams);")
                .AppendLine($"")
                .AppendLine($"")
                .AppendLine($"// Now, set the data context.")
                .AppendLine($"this.DataContext = cqResult.Result.ToViewModel();")
                .AppendLine("}")
                .AppendLine("else")
                .AppendLine("{")
                .AppendLine("this.DataContext = new();")
                .AppendLine("}")
                .Build();

        [DebuggerStepThrough]
        public static string NavigateTo(string url) =>
            $"this._navigationManager.NavigateTo({url});";

        private static string CreateQueryHandleMethodBody(CqrsQueryViewModel model, string? additionalWhereClause = null)
        {
            // Create query to be used inside the body code.
            var bodyQuery = SqlStatementBuilder
                .Select(model.ParamsDto.DbObject.Name!)
                .SetTopCount(model.ResultDto.IsList ? null : 1)
                .Where(ReplaceVariables(model.ParamsDto, additionalWhereClause, "query.Params"))
                .Columns(model.ResultDto.Properties.Select(x => x.DbObject?.Name).Compact())
                .Build()
                .Replace(Environment.NewLine, " ").Replace("  ", " ");
            // Create body code.
            (var sqlMethod, var toListMethod) = model.ResultDto.IsList
                ? (nameof(Sql.Select), ".ToList()")
                : (nameof(Sql.FirstOrDefault), string.Empty);
            var result = new StringBuilder()
                .AppendLine($"var dbQuery = $@\"{bodyQuery}\";")
                .AppendLine($"var dbResult = this._sql.{sqlMethod}<{model.GetSegregateResultParamsType("Query").FullPath}>(dbQuery){toListMethod};")
                .AppendLine($"var result = new {model.GetSegregateResultType("Query").FullPath}(dbResult);")
                .Append($"return Task.FromResult(result);")
                .Build();
            return result;
        }
    }
}