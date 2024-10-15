using HanyCo.Infra.CodeGen.Domain;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Threading;
using Library.Threading.MultistepProgress;
using Library.Validations;

using MediatR;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

using Services.Helpers;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Xml.Linq;

namespace Services;

internal sealed partial class FunctionalityService
{
    public async Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync(FunctionalityViewModel viewModel, CancellationToken token = default)
    {
        if (!BasicChecks(viewModel).TryParse(out var validationResult))
        {
            return validationResult!;
        }

        this._reporter.Report(description: getTitle("Initializing..."));
        var initResult = InitializeWorkspace(viewModel, token);
        if (!initResult.IsSucceed)
        {
            return Result.From(initResult, viewModel)!;
        }

        var (data, tokenSource) = initResult.Value;
        var process = initSteps(data);

        this._reporter.Report(description: getTitle("Running..."));
        var processResult = await process.RunAsync(tokenSource.Token);
        var message = getResultMessage(processResult, tokenSource.Token);

        this._reporter.Report(description: getTitle(message));
        var result = processResult.Value.Result;

        tokenSource.Dispose();

        this._reporter.End();
        return result!;

        [DebuggerStepThrough]
        ProgressData getTitle(in string description)
            => new(Description: description, Sender: nameof(FunctionalityService));

        [DebuggerStepThrough]
        MultistepProcessRunner<CreationData> initSteps(in CreationData data)
             //?! ☠ Don't change the sequence of the steps ☠
             => MultistepProcessRunner<CreationData>.New(data, this._reporter, owner: nameof(FunctionalityService))
                .AddStep(this.ResetWorkspace, getTitle("Initializing…"))

                .AddStep(this.CreateGetAllQuery, getTitle($"Creating `GetAll{StringHelper.Pluralize(data.ViewModel.Name)}Query`…"))
                .AddStep(this.CreateGetByIdQuery, getTitle($"Creating `GetById{data.ViewModel.Name}Query`…"))
                .AddStep(this.CreateInsertCommand, getTitle($"Creating `Insert{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateUpdateCommand, getTitle($"Creating `Update{data.ViewModel.Name}Command`…"))
                .AddStep(this.CreateDeleteCommand, getTitle($"Creating `Delete{data.ViewModel.Name}Command`…"))

                .AddStep(this.CreateController, getTitle($"Creating `{data.ViewModel.Name}Controller`…"))

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

    private static void AddClaimViewModel(InfraViewModelBase viewModel, string? key, object? value, ClaimViewModel? parent)
        => new ClaimViewModel(key ?? viewModel.Name, value, parent).With(claim => viewModel.SecurityClaims.Add(claim));

    private static void AddClaimViewModel(InfraViewModelBase viewModel, string? key, object? value, InfraViewModelBase? parent)
        => AddClaimViewModel(viewModel, key, value, parent?.SecurityClaims.FirstOrDefault());

    private static void AddClaimViewModel(InfraViewModelBase viewModel, string? key, InfraViewModelBase parent)
        => AddClaimViewModel(viewModel, key, null, parent.SecurityClaims.FirstOrDefault());

    private static void AddClaimViewModel(InfraViewModelBase viewModel, string? key, CreationData data)
        => AddClaimViewModel(viewModel, key, null, data.ViewModel.SourceDto);

    private static void AddClaimViewModel(InfraViewModelBase viewModel, CreationData data)
        => AddClaimViewModel(viewModel, viewModel.Name, null, data.ViewModel.SourceDto);

    private static UiComponentViewModel AddHttpClientInjection(UiComponentViewModel model)
    {
        var httpClient = (Type: typeof(HttpClient), FieldName: "_http");
        _ = model.AddUsings(httpClient.Type.Namespace!, typeof(HttpClientJsonExtensions).Namespace!);
        _ = model.AdditionalInjects.Add(httpClient);
        return model;
    }

    private static DbColumnViewModel GetDbColumn(string name, PropertyType type, string? dtoFullName = null) =>
        new(name, -1, PropertyTypeHelper.ToFullTypeName(type, dtoFullName), false);

    private static PropertyViewModel GetIdDbColumnProperty(CreationData data) =>
        new(new DbColumnViewModel("Id", -1, PropertyTypeHelper.ToFullTypeName(PropertyType.Long), false)) { Comment = data.COMMENT };

    [Obsolete("Mappers are no longer used in this project", true)]
    private static string GetMapperNameSpace(CreationData data)
        => TypePath.Combine(GetRootNameSpace(data), "Mappers");

    private static PropertyViewModel GetPluralizedSourceDtoAsPropertyModel(CreationData data) =>
        new(StringHelper.Pluralize(CommonHelpers.Purify(data.SourceDtoName!)), PropertyType.Dto)
        {
            Comment = data.COMMENT,
            IsList = true,
            Dto = data.ViewModel.SourceDto,
            DbObject = data.ViewModel.SourceDto.DbObject,
        };

    private static string GetRootNameSpace(CreationData data) =>
        data.ViewModel.SourceDto.NameSpace.TrimSuffix(".Dtos").TrimSuffix(".Dto");

    private static PropertyViewModel GetSourceDtoAsPropertyModel(CreationData data) => new(CommonHelpers.Purify(data.SourceDtoName!), PropertyType.Dto)
    {
        Comment = data.COMMENT,
        IsList = false,
        Dto = data.ViewModel.SourceDto,
        TypeFullName = data.ViewModel.SourceDto.FullName,
        Type = PropertyType.Dto,
        DbObject = data.ViewModel.SourceDto.DbObject,
    };

    [DebuggerStepThrough]
    private static Result<(CreationData Data, CancellationTokenSource TokenSource)> InitializeWorkspace(FunctionalityViewModel viewModel, CancellationToken token)
    {
        var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        var result = new CreationData(viewModel, viewModel.SourceDto.Name!, tokenSource);
        if (result.ViewModel.SourceDto.NameSpace.IsNullOrEmpty())
        {
            result.ViewModel.SourceDto.NameSpace = TypePath.Combine(result.ViewModel.SourceDto.NameSpace, result.ViewModel.SourceDto.Module.Name!.Remove(" "), "Dtos");
        }
        if (result.ViewModel.Module is null)
        {
            result.ViewModel.Module = result.ViewModel.SourceDto.Module;
        }

        // Return a success result with the result and cancellationTokenSource
        return Result.Success<(CreationData, CancellationTokenSource)>((result, tokenSource));
    }

    //[DebuggerStepThrough]
    private static DtoViewModel RawDto(CreationData data)
    {
        // Create an initial DTO based on the input data
        var dto = new DtoViewModel(data.ViewModel.SourceDto.Id, data.ViewModel.Name!)
        {
            Comment = data.COMMENT, // Set DTO comment
            NameSpace = TypePath.Combine(GetRootNameSpace(data), "Dtos"), // Set DTO namespace
            Functionality = data.ViewModel,
            DbObject = data.ViewModel.SourceDto.DbObject
        }
        .With(x => x.Module.Id = data.ViewModel.SourceDto.Module.Id); // Set DTO module ID

        return dto;
    }

    private static PropertyViewModel SourceDtoToProperty(CreationData data) =>
        new(GetDbColumn(data.SourceDtoName, PropertyType.Dto, data.SourceDtoType)) { Comment = data.COMMENT, TypeFullName = data.SourceDtoType };

    private Task CreateBlazorDetailsComponent(CreationData data, CancellationToken token)
    {
        var name = CommonHelpers.Purify(data.SourceDtoName);
        return TaskRunner.SetState(data)
            .Then(createViewModel)
            .Then(setupEditForm)
            .Then(addActions)
            .Then(addParameters)
            //.Then(createConverters)
            .Then(setupSecurity)
            .RunAsync(token);

        void createViewModel(CreationData data)
        {
            data.ViewModel.BlazorDetailsComponent = this._blazorComponentService.CreateViewModel(data.ViewModel.SourceDto);
            data.ViewModel.BlazorDetailsComponent.Name = $"{name}DetailsComponent";
            data.ViewModel.BlazorDetailsComponent.ClassName = $"{name}DetailsComponent";
            data.ViewModel.BlazorDetailsComponent.IsGrid = false;
            data.ViewModel.BlazorDetailsComponent.PageDataContext = data.ViewModel.BlazorDetailsPage.DataContext;
            data.ViewModel.BlazorDetailsComponent.PageDataContextProperty = data.ViewModel.BlazorDetailsPage.DataContext.Properties.First(x => x.IsList != true);
            data.ViewModel.BlazorDetailsComponent.Attributes.Add(new("@bind-EntityId", "this.Id"));
            _ = data.ViewModel.BlazorDetailsComponent.AddUsings("Web.UI.Components.Shared", typeof(Microsoft.AspNetCore.Components.ElementReference).Namespace!, data.ViewModel.SourceDto!.NameSpace);
            //data.ViewModel.BlazorDetailsComponent.AdditionalUsingNameSpaces.Add(GetMapperNameSpace(data));
            _ = AddHttpClientInjection(data.ViewModel.BlazorDetailsComponent);
            data.ViewModel.BlazorDetailsPage.Components.Add(data.ViewModel.BlazorDetailsComponent);
        }

        void addActions(CreationData data)
        {
            var pageRoute = BlazorPage.GetPageRoute(CommonHelpers.Purify(data.ViewModel.SourceDto.Name!), data.ViewModel.SourceDto.Module.Name, null);
            data.ViewModel.BlazorListPage.Routes.Add(pageRoute);
            // The Save button
            var saveButton = new UiComponentCustomButton()
            {
                CodeStatement = CodeSnippets.BlazorDetailsComponent_SaveButton_OnClick_Body(data.ViewModel.Controller.Name, data.SourceDtoName),
                Caption = "Save",
                ButtonType = ButtonType.Submit,
                Guid = Guid.NewGuid(),
                IsEnabled = true,
                Name = "SaveButton",
                Placement = Placement.FormButton,
                Description = "Saves the data to database",
                Position = new()
                {
                    Col = 2,
                    Offset = 2,
                    Row = 1,
                }
            };
            AddClaimViewModel(saveButton, "Save", data.ViewModel.BlazorDetailsComponent);

            // The Back button. Same as the cancel button.
            var cancelButton = new UiComponentCustomButton()
            {
                Caption = "Back",
                CodeStatement = CodeSnippets.NavigateTo(pageRoute.TrimStart("@page").Trim()),
                ButtonType = ButtonType.Button,
                EventHandlerName = "BackButton_OnClick",
                Guid = Guid.NewGuid(),
                IsEnabled = true,
                Name = "BackButton",
                Placement = Placement.FormButton,
                Position = new()
                {
                    Col = 2,
                    Offset = 2
                }
            };
            var onLoad = new UiComponentCustomLoad
            {
                CodeStatement = CodeSnippets.BlazorDetailsComponent_LoadPage_Body(data.ViewModel.Controller.Name, data.SourceDtoName),
            };
            data.ViewModel.BlazorDetailsComponent.Actions.Add(saveButton);
            data.ViewModel.BlazorDetailsComponent.Actions.Add(cancelButton);
            data.ViewModel.BlazorDetailsComponent.Actions.Add(onLoad);
        }

        static void addParameters(CreationData data)
        {
            data.ViewModel.BlazorDetailsComponent.Parameters.Add(new(TypePath.New("long"), "EntityId"));
            data.ViewModel.BlazorDetailsComponent.Parameters.Add(new(TypePath.New("Microsoft.AspNetCore.Components.EventCallback<long?>"), "EntityIdChanged"));
        }

        void setupEditForm(CreationData data)
        {
            var info = data.ViewModel.BlazorDetailsComponent.EditFormInfo;
            info.IsEditForm = true;
            _ = info.Events.Add(new(nameof(EditForm.OnValidSubmit), new Library.CodeGeneration.v2.Back.Method("SaveData")
            {
                Body = CodeSnippets.BlazorDetailsComponent_SaveButton_OnClick_Body(data.ViewModel.Controller.Name, data.SourceDtoName),
                ReturnType = "async Task"
            }, true));
        }

        void setupSecurity(CreationData data)
            => AddClaimViewModel(data.ViewModel.BlazorDetailsComponent, $"{name}Details", data);
    }

    private Task CreateBlazorDetailsPage(CreationData data, CancellationToken token)
    {
        var name = CommonHelpers.Purify(data.ViewModel.SourceDto.Name)?.AddToEnd("DetailsPage");
        return TaskRunner.SetState(data)
            .Then(createPageViewModel)
            .Then(addParameters)
            .Then(setupSecurity)
            .RunAsync(token);
        void createPageViewModel(CreationData data) =>
            data.ViewModel.BlazorDetailsPage = this._blazorPageService.CreateViewModel(data.ViewModel.SourceDto)
                .AddUsings(typeof(Microsoft.AspNetCore.Components.ElementReference).Namespace!)
                .With(x => x.Name = name)
                .With(x => x.ClassName = name);
        static void addParameters(CreationData data) =>
            data.ViewModel.BlazorDetailsPage.Parameters.Add(new(TypePath.New<long>(), "Id"));

        void setupSecurity(CreationData data) =>
            AddClaimViewModel(data.ViewModel.BlazorDetailsPage, $"{name}Details", data);
    }

    private Task CreateBlazorListComponent(CreationData data, CancellationToken token)
    {
        var name = StringHelper.Pluralize(CommonHelpers.Purify(data.SourceDtoName));
        return TaskRunner.SetState(data)
            .Then(createViewModel)
            .Then(addActions)
            .Then(setupSecurity)
            .RunAsync(token);

        void createViewModel(CreationData data)
        {
            data.ViewModel.BlazorListComponent = this._blazorComponentService.CreateViewModel(data.ViewModel.SourceDto);
            data.ViewModel.BlazorListComponent.Name = $"{name}ListComponent";
            data.ViewModel.BlazorListComponent.ClassName = $"{name}ListComponent";
            data.ViewModel.BlazorListComponent.IsGrid = true;
            data.ViewModel.BlazorListComponent.PageDataContext = data.ViewModel.BlazorListPage.DataContext;
            data.ViewModel.BlazorListComponent.PageDataContextProperty = data.ViewModel.BlazorListPage.DataContext.Properties.First(x => x.IsList == true);
            _ = data.ViewModel.BlazorListComponent.AddUsings("Web.UI.Components.Shared", data.ViewModel.SourceDto!.NameSpace);
            //data.ViewModel.BlazorListComponent.AdditionalUsingNameSpaces.Add(GetMapperNameSpace(data));
            _ = AddHttpClientInjection(data.ViewModel.BlazorListComponent);
            data.ViewModel.BlazorListPage.Components.Add(data.ViewModel.BlazorListComponent);
        }

        void addActions(CreationData data)
        {
            var pageName = $"{CommonHelpers.Purify(data.ViewModel.SourceDto.Name!)}/details";
            var pureRoute = BlazorPage.GetPageRoute(pageName, data.ViewModel.SourceDto.Module.Name, null);
            var routeWithId = BlazorPage.GetPageRoute(pageName, data.ViewModel.SourceDto.Module.Name, null, "{Id:long}");
            _ = data.ViewModel.BlazorDetailsPage.Routes.AddRange(pureRoute, routeWithId);

            var newButton = new UiComponentCustomButton
            {
                CodeStatement = CodeSnippets.NavigateTo(pureRoute.TrimStart("@page").Trim()),
                Caption = "New",
                EventHandlerName = "NewButton_OnClick",
                Guid = Guid.NewGuid(),
                Name = "NewButton",
                Placement = Placement.FormButton,
                Description = $"Creates new {name}",
            };
            var editButton = new UiComponentCustomButton
            {
                CodeStatement = CodeSnippets.NavigateTo($"${pureRoute.TrimStart("@page").TrimEnd("\"").Trim()}/{{id.ToString()}}\""), //$"this._navigationManager.NavigateTo(${pureRoute.TrimStart("@page").TrimEnd("\"").Trim()}/{{id.ToString()}}\");",
                Caption = "Edit",
                EventHandlerName = "EditButton_OnClick",
                Guid = Guid.NewGuid(),
                Name = "EditButton",
                Placement = Placement.RowButton,
                Description = $"Edits selected {name}"
            };
            var deleteButton = new UiComponentCustomButton
            {
                CodeStatement = CodeSnippets.BlazorListComponent_DeleteButton_OnClick_Body(data.ViewModel.Controller.Name, data.SourceDtoName),
                Caption = "Delete",
                EventHandlerName = "DeleteButton_OnClick",
                Guid = Guid.NewGuid(),
                Name = "DeleteButton",
                Placement = Placement.RowButton,
                Description = $"Deletes selected {name}",
                ReturnType = "async void"
            };
            var onLoad = new UiComponentCustomLoad
            {
                CodeStatement = CodeSnippets.BlazorListComponent_LoadPage_Body(data.ViewModel.Controller.Name, data.SourceDtoName, data.SourceDtoName)
            };
            _ = data.ViewModel.BlazorListComponent.Actions.AddRange(new IUiComponentContent[] { newButton, editButton, deleteButton, onLoad });
        }

        void setupSecurity(CreationData data) =>
            AddClaimViewModel(data.ViewModel.BlazorListComponent, $"{name}List", data);
    }

    private Task CreateBlazorListPage(CreationData data, CancellationToken token)
    {
        var name = CommonHelpers.Purify(data.ViewModel.SourceDto.Name)?.AddToEnd("ListPage");
        return TaskRunner.SetState(data)
            .Then(createPageViewModel)
            .Then(setupSecurity)
            .RunAsync(token);

        void createPageViewModel(CreationData data) =>
            data.ViewModel.BlazorListPage = this._blazorPageService.CreateViewModel(data.ViewModel.SourceDto)
                .With(x => x.Name = name)
                .With(x => x.ClassName = name);

        void setupSecurity(CreationData data) =>
            AddClaimViewModel(data.ViewModel.BlazorListPage, $"{name}List", data);
    }

    private Task CreateController(CreationData data, CancellationToken token)
    {
        return TaskRunner.SetState(data)
            .Then(initialize)
            .Then(createCtor)
            .Then(createGetAllApi)
            .Then(createGetByIdApi)
            .Then(createInsertApi)
            .Then(createUpdateApi)
            .Then(createDeleteApi)
            .RunAsync(token);

        void initialize(CreationData data)
            => data.ViewModel.Controller
                .With(x => x.AdditionalUsings.Add(data.ViewModel.GetAllQuery.DtoNameSpace))
                .With(x => x.AdditionalUsings.Add(typeof(Result).Namespace!))
                .With(x => x.AdditionalUsings.Add(typeof(Result<>).Namespace!))
                .With(x => x.NameSpace = TypePath.Combine(GetRootNameSpace(data), "Controllers"))
                .With(x => x.Name = string.Concat(CommonHelpers.Purify(data.SourceDtoName), "Controller"));

        void createCtor(CreationData data)
            => data.ViewModel.Controller.CtorParams.Add((MethodArgument.New(TypePath.New<IMediator>()), true));

        void createGetAllApi(CreationData data)
        {
            var api = ControllerMethodViewModel
                .New("GetAll")
                .AddHttpMethod<HttpGetAttribute>()
                .AddBodyLine($"var result = await this._mediator.Send(new {data.ViewModel.GetAllQuery}());")
                .AddBodyLine($"return this.Ok(result.{data.ViewModel.GetAllQuery.ResultDto.Properties[0].Name});")
                .WithReturnType(TypePath.NewTask<IActionResult>())
                .IsAsync(true);
            _ = data.ViewModel.Controller.Apis.Add(api);
            _ = data.ViewModel.Controller.AdditionalUsings.Add(data.ViewModel.SourceDto.NameSpace!);
        }

        void createGetByIdApi(CreationData data)
        {
            var api = ControllerMethodViewModel
                .New("GetById")
                .AddHttpMethod<HttpGetAttribute>("{id:long}")
                .AddArgument(TypePath.New<long>(), "id")
                .AddBodyLine($"var result = await this._mediator.Send(new {data.ViewModel.GetByIdQuery}(id));")
                .AddBodyLine($"return this.Ok(result.{data.ViewModel.GetByIdQuery.ResultDto.Properties[0].Name});")
                .WithReturnType(TypePath.NewTask<IActionResult>())
                .IsAsync(true);
            _ = data.ViewModel.Controller.Apis.Add(api);
        }

        void createInsertApi(CreationData data)
        {
            var argName = TypeMemberNameHelper.ToArgName(data.SourceDtoName!);
            var api = ControllerMethodViewModel
                .New("Insert")
                .AddHttpMethod<HttpPostAttribute>()
                .AddArgument(data.SourceDtoName!, argName)
                .AddBodyLine($"var result = await this._mediator.Send(new {data.ViewModel.InsertCommand}({argName}));")
                .AddBodyLine($"return this.Ok(result.{data.ViewModel.InsertCommand.ResultDto.Properties[0].Name});")
                .WithReturnType(TypePath.NewTask<IActionResult>())
                .IsAsync(true);
            _ = data.ViewModel.Controller.Apis.Add(api);
        }

        void createUpdateApi(CreationData data)
        {
            var argName = TypeMemberNameHelper.ToArgName(data.SourceDtoName!);
            var api = ControllerMethodViewModel
                .New("Update")
                .AddHttpMethod<HttpPutAttribute>("{id:long}")
                .AddArgument(TypePath.New<long>(), "id")
                .AddArgument(data.SourceDtoName!, argName)
                .AddBodyLine($"var result = await this._mediator.Send(new {data.ViewModel.UpdateCommand}(id, {argName}));")
                .AddBodyLine("return this.Ok(result);")
                .WithReturnType(TypePath.NewTask<IActionResult>())
                .IsAsync(true);
            _ = data.ViewModel.Controller.Apis.Add(api);
        }

        void createDeleteApi(CreationData data)
        {
            var api = ControllerMethodViewModel
                .New("Delete")
                .AddHttpMethod<HttpDeleteAttribute>("{id:long}")
                .AddArgument(TypePath.New<long>(), "id")
                .AddBodyLine($"var result = await this._mediator.Send(new {data.ViewModel.DeleteCommand}(id));")
                .AddBodyLine("return this.Ok(true);")
                .WithReturnType(TypePath.NewTask<IActionResult>())
                .IsAsync(true);
            _ = data.ViewModel.Controller.Apis.Add(api);
        }
    }

    private Task CreateDeleteCommand(CreationData data, CancellationToken token)
    {
        var name = $"Delete{CommonHelpers.Purify(data.SourceDtoName)}";
        return TaskRunner.SetState(data)
            .Then(createViewModel)
            .Then(createValidator)
            .Then(createResult)
            .Then(createParams)
            .Then(createHandleMethodBody)
            .Then(setupSecurity)
            .RunAsync(token);

        async Task createViewModel(CancellationToken token)
        {
            data.ViewModel.DeleteCommand = await this._commandService.CreateAsync(token);
            data.ViewModel.DeleteCommand.Name = $"{name}Command";
            data.ViewModel.DeleteCommand.Category = CqrsSegregateCategory.Delete;
            data.ViewModel.DeleteCommand.CqrsNameSpace = TypePath.Combine(GetRootNameSpace(data), "Commands");
            data.ViewModel.DeleteCommand.DtoNameSpace = TypePath.Combine(GetRootNameSpace(data), "Dtos");
            data.ViewModel.DeleteCommand.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.DeleteCommand.FriendlyName = data.ViewModel.DeleteCommand.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.DeleteCommand.Comment = data.COMMENT;
            data.ViewModel.DeleteCommand.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
            //data.ViewModel.DeleteCommand.MapperNameSpace = GetMapperNameSpace(data);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.DeleteCommand.ParamsDto = RawDto(data);
            data.ViewModel.DeleteCommand.ParamsDto.Name = $"{name}Command";
            data.ViewModel.DeleteCommand.ParamsDto.IsParamsDto = true;
            data.ViewModel.DeleteCommand.ParamsDto.Properties.Add(GetIdDbColumnProperty(data));
        }

        void createResult(CreationData data)
        {
            data.ViewModel.DeleteCommand.ResultDto = RawDto(data);
            data.ViewModel.DeleteCommand.ResultDto.Name = $"{name}CommandResult";
            data.ViewModel.DeleteCommand.ResultDto.IsResultDto = true;
        }

        void createValidator(CreationData data)
        {
        }

        static void createHandleMethodBody(CreationData data) =>
            data.ViewModel.DeleteCommand.HandleMethodBody = CodeSnippets.DeleteCommandHandler_Handle_Body(data.ViewModel.DeleteCommand, data.ViewModel.SourceDto);

        void setupSecurity(CreationData data) =>
            AddClaimViewModel(data.ViewModel.DeleteCommand, data);
    }

    private async Task CreateGetAllQuery(CreationData data, CancellationToken token)
    {
        var name = $"GetAll{StringHelper.Pluralize(CommonHelpers.Purify(data.SourceDtoName))}";
        var result = await TaskRunner.SetState(data)
            .Then(createViewModel)
            .Then(createResult)
            .Then(createParams)
            .Then(createHandleMethodBody)
            .Then(setupSecurity)
            .RunAsync(token);

        async Task createViewModel(CreationData data, CancellationToken token)
        {
            data.ViewModel.GetAllQuery = await this._queryService.CreateAsync(token);
            data.ViewModel.GetAllQuery.Name = $"{name}Query";
            data.ViewModel.GetAllQuery.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetAllQuery.CqrsNameSpace = TypePath.Combine(GetRootNameSpace(data), "Queries");
            data.ViewModel.GetAllQuery.DtoNameSpace = TypePath.Combine(GetRootNameSpace(data), "Dtos");
            data.ViewModel.GetAllQuery.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetAllQuery.FriendlyName = data.ViewModel.GetAllQuery.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetAllQuery.Comment = data.COMMENT;
            data.ViewModel.GetAllQuery.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
            //data.ViewModel.GetAllQuery.MapperNameSpace = GetMapperNameSpace(data);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.GetAllQuery.ParamsDto = RawDto(data);
            data.ViewModel.GetAllQuery.ParamsDto.Name = $"{name}Query";
            data.ViewModel.GetAllQuery.ParamsDto.BaseType = TypePath.New<IRequest>([data.ViewModel.GetAllQuery.ResultDto.Name!]);
            data.ViewModel.GetAllQuery.ParamsDto.IsParamsDto = true;
        }

        void createResult(CreationData data)
        {
            data.ViewModel.GetAllQuery.ResultDto = RawDto(data);
            data.ViewModel.GetAllQuery.ResultDto.Name = $"{name}QueryResult";
            data.ViewModel.GetAllQuery.ResultDto.IsResultDto = true;
            data.ViewModel.GetAllQuery.ResultDto.IsList = true;
            try
            {
                data.ViewModel.GetAllQuery.ResultDto.Properties.Add(GetPluralizedSourceDtoAsPropertyModel(data));
            }
            catch
            {
                throw;
            }
        }

        static void createHandleMethodBody(CreationData data) =>
            data.ViewModel.GetAllQuery.HandleMethodBody = CodeSnippets.QueryHandler_Handle_Body(data.ViewModel.GetAllQuery, data.ViewModel.SourceDto);

        void setupSecurity(CreationData data) =>
            AddClaimViewModel(data.ViewModel.GetAllQuery, data);
    }

    private Task CreateGetByIdQuery(CreationData data, CancellationToken token)
    {
        var name = $"GetById{CommonHelpers.Purify(data.SourceDtoName)}";

        return TaskRunner.SetState(data)
            .Then(createModel)
            .Then(createResult)
            .Then(createParams)
            .Then(createHandleMethodBody)
            .Then(setupSecurity)
            //.Then(createConverters)
            .RunAsync(token);

        async Task createModel(CancellationToken token)
        {
            data.ViewModel.GetByIdQuery = await this._queryService.CreateAsync(cancellationToken: token);
            data.ViewModel.GetByIdQuery.Name = $"{name}Query";
            data.ViewModel.GetByIdQuery.Category = CqrsSegregateCategory.Read;
            data.ViewModel.GetByIdQuery.CqrsNameSpace = TypePath.Combine(GetRootNameSpace(data), "Queries");
            data.ViewModel.GetByIdQuery.DtoNameSpace = TypePath.Combine(GetRootNameSpace(data), "Dtos");
            data.ViewModel.GetByIdQuery.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.GetByIdQuery.FriendlyName = data.ViewModel.GetByIdQuery.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.GetByIdQuery.Comment = data.COMMENT;
            data.ViewModel.GetByIdQuery.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, cancellationToken: token);
        }

        void createParams()
        {
            data.ViewModel.GetByIdQuery.ParamsDto = RawDto(data);
            data.ViewModel.GetByIdQuery.ParamsDto.Name = $"{name}Query";
            data.ViewModel.GetByIdQuery.ParamsDto.IsParamsDto = true;
            data.ViewModel.GetByIdQuery.ParamsDto.BaseType = TypePath.New<IRequest>([data.ViewModel.GetByIdQuery.ResultDto.Name!]);
            data.ViewModel.GetByIdQuery.ParamsDto.Properties.Add(new(GetIdDbColumnProperty(data)));
        }

        void createResult()
        {
            data.ViewModel.GetByIdQuery.ResultDto = RawDto(data);
            data.ViewModel.GetByIdQuery.ResultDto.Name = $"{name}QueryResult";
            data.ViewModel.GetByIdQuery.ResultDto.IsResultDto = true;
            try
            {
                data.ViewModel.GetByIdQuery.ResultDto.Properties.Add(SourceDtoToProperty(data));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        void setupSecurity(CreationData data) =>
            AddClaimViewModel(data.ViewModel.GetAllQuery, data);

        static void createHandleMethodBody(CreationData data) =>
            data.ViewModel.GetByIdQuery.HandleMethodBody = CodeSnippets.QueryHandler_Handle_Body(data.ViewModel.GetByIdQuery, data.ViewModel.SourceDto, "[Id] = {request.Id}");
    }

    private Task CreateInsertCommand(CreationData data, CancellationToken token)
    {
        var name = $"Insert{CommonHelpers.Purify(data.SourceDtoName)}";
        return TaskRunner.SetState(data)
            .Then(createModel)
            .Then(createResult)
            .Then(createParams)
            .Then(createValidator)
            .Then(createHandleMethodBody)
            .Then(setupSecurity)
            .RunAsync(token);

        static void createValidator(CreationData data)
        {
            data.ViewModel.InsertCommand.ValidatorBody = CodeSnippets.InsertCommandValidator_Handle_Body(data.ViewModel.InsertCommand, data.ViewModel.SourceDto);
            _ = data.ViewModel.InsertCommand.ValidatorAdditionalUsings.Add(typeof(ValidationExtensions).Namespace);
        }

        async Task createModel(CreationData data, CancellationToken token)
        {
            data.ViewModel.InsertCommand = await this._commandService.CreateAsync(token);
            data.ViewModel.InsertCommand.Name = $"{name}Command";
            data.ViewModel.InsertCommand.Category = CqrsSegregateCategory.Create;
            data.ViewModel.InsertCommand.CqrsNameSpace = TypePath.Combine(GetRootNameSpace(data), "Commands");
            data.ViewModel.InsertCommand.DtoNameSpace = TypePath.Combine(GetRootNameSpace(data), "Dtos");
            data.ViewModel.InsertCommand.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.InsertCommand.FriendlyName = data.ViewModel.InsertCommand.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.InsertCommand.Comment = data.COMMENT;
            data.ViewModel.InsertCommand.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.InsertCommand.ParamsDto = RawDto(data);
            data.ViewModel.InsertCommand.ParamsDto.Name = $"{name}Command";
            data.ViewModel.InsertCommand.ParamsDto.IsParamsDto = true;
            data.ViewModel.InsertCommand.ParamsDto.Properties.Add(GetSourceDtoAsPropertyModel(data));
        }

        void createResult(CreationData data)
        {
            data.ViewModel.InsertCommand.ResultDto = RawDto(data);
            data.ViewModel.InsertCommand.ResultDto.Name = $"{name}CommandResult";
            data.ViewModel.InsertCommand.ResultDto.IsResultDto = true;
            data.ViewModel.InsertCommand.ResultDto.Properties.Add(GetIdDbColumnProperty(data));
        }

        void setupSecurity(CreationData data) =>
            AddClaimViewModel(data.ViewModel.InsertCommand, data);

        static void createHandleMethodBody(CreationData data) =>
            data.ViewModel.InsertCommand.HandleMethodBody = CodeSnippets.InsertCommandHandler_Handle_Body(data.ViewModel.InsertCommand, data.ViewModel.SourceDto);
    }

    private Task CreateUpdateCommand(CreationData data, CancellationToken token)
    {
        var name = $"Update{CommonHelpers.Purify(data.SourceDtoName)}";
        return TaskRunner.SetState(data)
            .Then(createHandler)
            .Then(createResult)
            .Then(createParams)
            .Then(createValidator)
            .Then(createHandleMethodBody)
            .Then(setupSecurity)
            .RunAsync(token);

        static void createValidator(CreationData data)
        {
            data.ViewModel.UpdateCommand.ValidatorBody = CodeSnippets.UpdateCommandValidator_Handle_Body(data.ViewModel.UpdateCommand);
            _ = data.ViewModel.UpdateCommand.ValidatorAdditionalUsings.Add(typeof(ValidationExtensions).Namespace);
        }

        async Task createHandler(CreationData data, CancellationToken token)
        {
            data.ViewModel.UpdateCommand = await this._commandService.CreateAsync(token);
            data.ViewModel.UpdateCommand.Name = $"{name}Command";
            data.ViewModel.UpdateCommand.Category = CqrsSegregateCategory.Update;
            data.ViewModel.UpdateCommand.CqrsNameSpace = TypePath.Combine(GetRootNameSpace(data), "Commands");
            data.ViewModel.UpdateCommand.DtoNameSpace = TypePath.Combine(GetRootNameSpace(data), "Dtos");
            data.ViewModel.UpdateCommand.DbObject = data.ViewModel.SourceDto.DbObject;
            data.ViewModel.UpdateCommand.FriendlyName = data.ViewModel.UpdateCommand.Name.SplitCamelCase().Merge(" ");
            data.ViewModel.UpdateCommand.Comment = data.COMMENT;
            data.ViewModel.UpdateCommand.Module = await this._moduleService.GetByIdAsync(data.ViewModel.SourceDto.Module.Id!.Value, cancellationToken: token);
        }

        void createParams(CreationData data)
        {
            data.ViewModel.UpdateCommand.ParamsDto = RawDto(data);
            data.ViewModel.UpdateCommand.ParamsDto.Name = $"{name}Command";
            data.ViewModel.UpdateCommand.ParamsDto.IsParamsDto = true;
            data.ViewModel.UpdateCommand.ParamsDto.Properties.Add(GetIdDbColumnProperty(data));
            data.ViewModel.UpdateCommand.ParamsDto.Properties.Add(GetSourceDtoAsPropertyModel(data));
        }

        void createResult(CreationData data)
        {
            data.ViewModel.UpdateCommand.ResultDto = RawDto(data);
            data.ViewModel.UpdateCommand.ResultDto.Name = $"{name}CommandResult";
            data.ViewModel.UpdateCommand.ResultDto.IsResultDto = true;
        }

        void setupSecurity(CreationData data) =>
            AddClaimViewModel(data.ViewModel.UpdateCommand, data);

        static void createHandleMethodBody(CreationData data) =>
            data.ViewModel.UpdateCommand.HandleMethodBody = CodeSnippets.UpdateCommandHandler_Handle_Body(data.ViewModel.UpdateCommand, data.ViewModel.SourceDto);
    }

    private Task ResetWorkspace(CreationData data, CancellationToken token)
    {
        data.ViewModel.MapperGeneratorViewModel.Arguments.Clear();
        data.ViewModel.Codes.Clear();
        return Task.CompletedTask;
    }
}