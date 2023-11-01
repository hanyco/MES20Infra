using System.Diagnostics.CodeAnalysis;
using System.Text;

using Contracts;
using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Validations;

using static HanyCo.Infra.CodeGeneration.Definitions.CodeConstants;

using ButtonViewModelBase = Contracts.ViewModels.UiComponentButtonViewModelBase;
using CqrsButtonViewModel = Contracts.ViewModels.UiComponentCqrsButtonViewModel;
using CqrsLoadViewModel = Contracts.ViewModels.UiComponentCqrsLoadViewModel;
using CstmButtonViewModel = Contracts.ViewModels.UiComponentCustomButtonViewModel;
using CstmLoadViewModel = Contracts.ViewModels.UiComponentCustomLoadViewModel;
using PropertyViewModel = Contracts.ViewModels.UiPropertyViewModel;
using UiViewModel = Contracts.ViewModels.UiComponentViewModel;

namespace Services;

internal sealed class BlazorCodingService(ILogger logger) : IBlazorComponentCodingService, IBlazorPageCodingService
{
    private readonly ILogger _logger = logger;

    public bool ControlTypeHasPropertiesPage(ControlType controlType) =>
        controlType switch
        {
            ControlType.None => false,
            ControlType.RadioButton => false,
            ControlType.CheckBox => false,
            ControlType.TextBox => false,
            ControlType.DropDown => false,
            ControlType.DateTimePicker => false,
            ControlType.NumericTextBox => false,
            ControlType.LookupBox => false,
            ControlType.ImageUpload => false,
            ControlType.FileUpload => false,
            ControlType.DataGrid => false,
            ControlType.CurrencyBox => false,
            ControlType.ExternalViewBox => false,
            ControlType.Component => false,
            _ => throw new NotImplementedException(),
        };

    public Result<Codes> GenerateCodes(UiViewModel model, GenerateCodesParameters? arguments = null)
    {
        if (!Check.IfArgumentIsNull(model).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }

        try
        {
            var component = createComponent(model);
            _ = processBackendActions(model, component);
            _ = processFrontActions(model, component);
            var codes = component.GenerateCodes(CodeCategory.Component, arguments);
            return Result<Codes>.CreateSuccess(codes);
        }
        catch (Exception ex)
        {
            return Result<Codes>.CreateFailure(ex, Codes.Empty);
        }

        static BlazorComponent createComponent(in UiViewModel model)
        {
            Check.MustBeArgumentNotNull(model.Name);

            var (dataContextType, dataContextPropType) = createDataContext(model);
            var result = getNewComponent(model, dataContextType);
            setDataContext(model, dataContextPropType, result);
            return result;

            static (TypePath DataContextType, TypePath? DataContextPropType) createDataContext(UiViewModel model)
            {
                var dataContextType = TypePath.New(model.PageDataContext?.Name, model.PageDataContext?.NameSpace);
                var dataContextPropType = model.PageDataContextProperty is not null and { Name: not null }
                    ? TypePath.New(model.PageDataContextProperty.TypeFullName)
                    : null;
                return (dataContextType, dataContextPropType);
            }

            static BlazorComponent getNewComponent(UiViewModel model, TypePath dataContextType) =>
                BlazorComponent.New(model.Name!)
                    .With(x => x.NameSpace = model.NameSpace)
                    .With(x => x.IsGrid = model.IsGrid)
                    .With(x => x.DataContextType = dataContextType);

            static void setDataContext(UiViewModel model, TypePath? dataContextPropType, BlazorComponent result)
            {
                if (dataContextPropType is { } prop)
                {
                    result.DataContextProperty = (prop, model.Name!);
                }
            }
        }
        static BlazorComponent createForm(in UiViewModel model, in BlazorComponent result)
        {
            foreach (var prop in model.Properties)
            {
                var bindPropName = InstanceDataContextProperty(prop.Name);
                var position = prop.Position.ToBootstrapPosition();

                if (position.IsDefault())
                {
                    _ = position.SetCol(5);
                }
                switch (prop.ControlType)
                {
                    case ControlType.None:
                        break;

                    case ControlType.RadioButton:
                        break;

                    case ControlType.CheckBox:
                        result.Children.Add(createLabel(prop));
                        result.Children.Add(new BlazorCheckBox($"{prop.Name}CheckBox", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
                        break;

                    case ControlType.TextBox:
                        result.Children.Add(createLabel(prop));
                        result.Children.Add(new BlazorTextBox($"{prop.Name}TextBox", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
                        break;

                    case ControlType.DateTimePicker:
                        result.Children.Add(createLabel(prop));
                        result.Children.Add(new BlazorDatePicker($"{prop.Name}DatePicker", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
                        break;

                    case ControlType.NumericTextBox:
                        result.Children.Add(createLabel(prop));
                        result.Children.Add(new BlazorNumericTextBox($"{prop.Name}NumericTextBox", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
                        break;

                    case ControlType.CurrencyBox:
                        break;

                    case ControlType.DropDown:
                        break;

                    case ControlType.LookupBox:
                        break;

                    case ControlType.ImageUpload:
                        break;

                    case ControlType.FileUpload:
                        break;

                    case ControlType.DataGrid:
                        result.Children.Add(createLabel(prop));
                        result.Children.Add(new BlazorTable { Position = position, IsEnabled = prop.IsEnabled });
                        break;

                    case ControlType.ExternalViewBox:
                        break;

                    case ControlType.Component:
                        break;
                }
            }
            foreach (var elementAction in model.Actions.OfType<FrontElement>())
            {
                IHtmlElement button = elementAction switch
                {
                    CqrsButtonViewModel cqrsButtonViewModel => createCqrsButton(model, cqrsButtonViewModel),
                    CstmButtonViewModel cstmButtonViewModel => createCstmButton(model, cstmButtonViewModel),
                    _ => throw new NotImplementedException()
                };
                result.Children.Add(button);
            }
            return result;

            static IHtmlElement createLabel([DisallowNull] PropertyViewModel prop) =>
                new BlazorLabel($"{prop.ArgumentNotNull().Name}Label", body: prop.Caption.NotNull(New<NotFoundValidationException>))
                {
                    Position = prop.Position.ToBootstrapPosition().SetRow(1).SetCol(2),
                    IsEnabled = prop.IsEnabled
                };

            static BlazorCqrsButton createCqrsButton(UiViewModel model, CqrsButtonViewModel cqrsButtonViewModel)
            {
                var button = new BlazorCqrsButton(name: cqrsButtonViewModel.Name, body: cqrsButtonViewModel.Caption, onClick: cqrsButtonViewModel.EventHandlerName)
                {
                    Position = cqrsButtonViewModel.Position.ToBootstrapPosition()
                };
                if (cqrsButtonViewModel.CqrsSegregate?.Name is not null)
                {
                    button = cqrsButtonViewModel.CqrsSegregate switch
                    {
                        CqrsQueryViewModel query => button.SetAction(
                            query.Name!,
                            new QueryCqrsSegregation(query.Name!, new(model.PageDataContextType, null!), query.ResultDto?.Name.IsNullOrEmpty() ?? true ? null : new(query.ResultDto.Name, null!))),
                        CqrsCommandViewModel command => button.SetAction(
                            command.Name!,
                            new CommandCqrsSegregation(command.Name!, command.ParamsDto is null ? null : new(TypePath.New(command.ParamsDto.Name, command.ParamsDto.NameSpace), null!), command.ResultDto is null ? null : new(TypePath.New(command.ResultDto.Name, command.ResultDto.NameSpace), null!))),
                        _ => throw new NotImplementedException()
                    };
                }

                return button;
            }

            static BlazorCustomButton createCstmButton(UiViewModel model, CstmButtonViewModel customButtonViewModel)
            {
                var button = new BlazorCustomButton(name: customButtonViewModel.Name, body: customButtonViewModel.CodeStatement, onClick: customButtonViewModel.EventHandlerName)
                {
                    Position = customButtonViewModel.Position.ToBootstrapPosition()
                };
                return button.SetAction(model.Name!, customButtonViewModel.CodeStatement);
            }
        }

        static BlazorComponent createGrid(UiViewModel model, BlazorComponent result)
        {
            var id = model.Properties.First(x => x.Name!.EqualsTo("id"));
            var idType = TypePath.New(id.Property.NotNull().Type.ToFullTypeName());
            foreach (var action in model.Actions.OfType<FrontElement>())
            {
                switch (action)
                {
                    case ButtonViewModelBase button:
                        var args = model.IsGrid && button.Placement == Placement.RowButton ? new[] { new MethodArgument(idType, "id") } : null;
                        result.Actions.Add(new ButtonActor(
                            button.Name,
                            button.Placement == Placement.RowButton,
                            button.Caption,
                            arguments: args,
                            eventHandlerName: button.EventHandlerName,
                            body: button.Cast().As<CstmButtonViewModel>()?.CodeStatement));
                        break;
                }
            }
            return result;
        }

        static BlazorComponent processBackendActions(UiViewModel model, BlazorComponent result)
        {
            foreach (var action in model.Actions.OfType<BackElement>())
            {
                switch (action)
                {
                    case CqrsLoadViewModel load when load.CqrsSegregate?.DbObject?.Name != null:
                        var entityName = StringHelper.Pluralize(load.CqrsSegregate.DbObject.Name);
                        result.Actions.Add(new(GetAll_OnCallingMethodName(entityName), false));
                        result.Actions.Add(new(Keyword_AddToOnInitializedAsync(), body: GetAll_CallMethodBody(entityName)));
                        result.Actions.Add(new(GetAll_OnCalledMethodName(entityName), false));
                        break;

                    case CqrsLoadViewModel load:
                        throw new InvalidOperationValidationException("`OnCqrsLoad` method has not required fields.");

                    case CstmLoadViewModel load when load.CodeStatement != null:
                        result.Actions.Add(new(Keyword_AddToOnInitializedAsync(), true, load.CodeStatement));
                        break;

                    case CstmLoadViewModel load:
                        throw new InvalidOperationValidationException("`OnCustomLoad` method has not required fields.");
                }
            }
            return result;
        }

        static BlazorComponent processFrontActions(UiViewModel model, BlazorComponent component) =>
            model.IsGrid ? createGrid(model, component) : createForm(model, component);

        static string GetAll_CallMethodBody(string entityName) =>
            new StringBuilder()
                .AppendLine($"// Setup segregation parameters")
                .AppendLine($"var paramsParams = new Dtos.GetAll{entityName}Params();")
                .AppendLine($"var cqParams = new Dtos.GetAll{entityName}QueryParams(paramsParams);")
                .AppendLine($"")
                //.AppendLine($"// Let the developer know what's going on.")
                //.AppendLine($"cqParams = OnCallingGetAll{entityName}Query(cqParams);")
                .AppendLine($"")
                .AppendLine($"// Invoke the query handler to retrieve all entities")
                .AppendLine($"var cqResult = await this._queryProcessor.ExecuteAsync<Dtos.GetAll{entityName}QueryResult>(cqParams);")
                .AppendLine($"")
                //.AppendLine($"// Let's inform the developer about the result.")
                //.AppendLine($"cqResult = OnCalledGetAll{entityName}Query(cqParams, cqResult);")
                .AppendLine($"")
                .AppendLine($"// Now, set the data context.")
                .AppendLine($"this.DataContext = cqResult.Result;") //x .To{StringHelper.Singularize(entityName)}Dto()
                .ToString();
    }

    public Result<Codes> GenerateCodes(UiPageViewModel viewModel, GenerateCodesParameters? arguments)
    {
        if (!this.Validate(viewModel).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }
        this._logger.Debug($"Generating code is started.");
        var dataContextType = TypePath.New(viewModel.DataContext?.Name, viewModel.DataContext?.NameSpace);
        var page = (viewModel.Route.IsNullOrEmpty()
            ? BlazorPage.NewByModuleName(arguments?.BackendFileName ?? viewModel.Name!, viewModel.Module.Name!)
            : BlazorPage.NewByPageRoute(arguments?.BackendFileName ?? viewModel.Name!, viewModel.Route))
                .With(x => x.NameSpace = viewModel.NameSpace)
                .With(x => x.DataContextType = dataContextType);
        _ = page.Children.AddRange(viewModel.Components.Select(x => toHtmlElement(x, dataContextType, x.PageDataContextProperty is null ? null : (new TypePath(x.PageDataContextProperty.TypeFullName), x.PageDataContextProperty.Name!))));

        var result = page.GenerateCodes(CodeCategory.Page, arguments);
        this._logger.Debug($"Generating code is done.");

        return Result<Codes>.CreateSuccess(result);

        static IHtmlElement toHtmlElement(UiViewModel component, string? dataContextType, (TypePath Type, string Name)? dataContextTypeProperty) =>
            BlazorComponent.New(component.Name!)
                .With(x => x.NameSpace = component.NameSpace)
                .With(x => x.DataContextType = dataContextType)
                .With(x => x.DataContextProperty = dataContextTypeProperty)
                .With(x => x.Position = new(component.Position.Order, component.Position.Row, component.Position.Col, component.Position.ColSpan, component.Position.Offset));
    }

    public bool HasPropertiesPage(ControlType? ct) =>
        ct switch
        {
            ControlType.RadioButton => false,
            ControlType.CheckBox => false,
            ControlType.TextBox => false,
            ControlType.DropDown => false,
            ControlType.DateTimePicker => false,
            ControlType.NumericTextBox => false,
            ControlType.LookupBox => false,
            ControlType.ImageUpload => false,
            ControlType.FileUpload => false,
            ControlType.DataGrid => true,
            ControlType.CurrencyBox => false,
            ControlType.ExternalViewBox => false,
            _ => false,
        };

    public Result<UiPageViewModel> Validate(in UiPageViewModel? model) =>
    model.ArgumentNotNull().Check()
         .NotNull(x => x.Name)
         .NotNull(x => x.NameSpace)
         .NotNull(x => x.ClassName)
         .NotNull(x => x.DataContext)
         .NotNull(x => x.Module)
         //.NotNull(x => x.Route)
         .Build();
}