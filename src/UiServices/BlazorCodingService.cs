using System.Diagnostics.CodeAnalysis;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Validations;

using ButtonViewModel = Contracts.ViewModels.UiComponentButtonViewModelBase;
using CqrsButtonViewModel = Contracts.ViewModels.UiComponentCqrsButtonViewModel;
using CustomButtonViewModel = Contracts.ViewModels.UiComponentCustomButtonViewModel;
using UiPropertyViewModel = Contracts.ViewModels.UiPropertyViewModel;
using UiViewModel = Contracts.ViewModels.UiViewModel;

namespace Services;

internal sealed class BlazorCodingService(
    IDtoService dtoService,
    IEntityViewModelConverter converter, ILogger logger,
    InfraReadDbContext readDbContext) : IBlazorComponentCodingService, IBlazorPageCodingService
{
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly IDtoService _dtoService = dtoService;
    private readonly ILogger _logger = logger;
    private readonly InfraReadDbContext _readDbContext = readDbContext;

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

    public Result<Codes> GenerateCodes(UiViewModel model, GenerateCodesArgs? arguments = null)
    {
        Result<Codes> result;
        try
        {
            var component = CreateComponent(model);
            var codes = component.GenerateCodes(CodeCategory.Component, arguments ?? new(true, true, true));
            result = Result<Codes>.CreateSuccess(codes);
        }
        catch (Exception ex)
        {
            result = Result<Codes>.CreateFailure(ex, Codes.Empty);
        }
        return result;
    }

    public bool HasPropertiesPage(ControlType? ct)
        => ct switch
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

    private static BlazorComponent CreateComponent(in UiViewModel model)
    {
        Check.MustBeArgumentNotNull(model?.Name);

        var (dataContextType, dataContextPropType) = createDataContext(model);
        var result = initializeComponent(model, dataContextType);
        setDataContext(model, dataContextPropType, result);
        if (!model.IsGrid)
        {
            createChildren(model, result);
        }
        else
        {
            createGrid(model, result);
        }
        return result;

        static (TypePath DataContextType, TypePath? DataContextPropType) createDataContext(UiViewModel model, CancellationToken cancellationToken = default)
        {
            var dataContextType = TypePath.New(model.PageDataContext?.Name, model.PageDataContext?.NameSpace);
            var dataContextPropType = model.PageDataContextProperty is not null and { Name: not null }
                        ? TypePath.New(model.PageDataContextProperty.TypeFullName)
                        : null;
            return (dataContextType, dataContextPropType);
        }

        static BlazorComponent initializeComponent(UiViewModel model, TypePath dataContextType) =>
            BlazorComponent.New(model.Name.NotNull())
                .With(x => x.NameSpace = model.NameSpace)
                .With(x => x.DataContextType = dataContextType)
                .With(x => x.IsGrid = model.IsGrid);

        static void setDataContext(UiViewModel model, TypePath? dataContextPropType, BlazorComponent result, CancellationToken cancellationToken = default)
        {
            if (dataContextPropType is not { } prop)
            {
                return;
            }
            _ = result.With(x => x.DataContextProperty = (prop, model.Name!));
        }

        static void createChildren<TBlazorComponent>(in UiViewModel model, in BlazorComponentBase<TBlazorComponent> engine, CancellationToken cancellationToken = default)
            where TBlazorComponent : BlazorComponentBase<TBlazorComponent>
        {
            for (var propertyIndex = 0; propertyIndex < model.UiProperties.Count; propertyIndex++)
            {
                var prop = model.UiProperties[propertyIndex];
                var bindPropName = $"this.DataContext.{prop.Name}";
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
                        engine.Children.Add(createLabel(prop));
                        engine.Children.Add(new BlazorCheckBox($"{prop.Name}CheckBox", bind: bindPropName) { Position = position });
                        break;

                    case ControlType.TextBox:
                        engine.Children.Add(createLabel(prop));
                        engine.Children.Add(new BlazorTextBox($"{prop.Name}TextBox", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
                        break;

                    case ControlType.DateTimePicker:
                        engine.Children.Add(createLabel(prop));
                        engine.Children.Add(new BlazorDatePicker($"{prop.Name}DatePicker", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
                        break;

                    case ControlType.NumericTextBox:
                        engine.Children.Add(createLabel(prop));
                        engine.Children.Add(new BlazorNumericTextBox($"{prop.Name}NumericTextBox", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
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
                        engine.Children.Add(createLabel(prop));
                        engine.Children.Add(new BlazorTable { Position = position, IsEnabled = prop.IsEnabled });
                        break;

                    case ControlType.ExternalViewBox:
                        break;

                    case ControlType.Component:
                        break;
                }
            }
            foreach (var backAction in model.UiActions.OfType<BackElement>())
            {
            }
            foreach (var elementAction in model.UiActions.OfType<FrontElement>())
            {
                IHtmlElement button = elementAction switch
                {
                    CqrsButtonViewModel cqrsButtonViewModel => createCqrsButton(model, cqrsButtonViewModel),
                    CustomButtonViewModel customButtonViewModel => createCustomButton(model, customButtonViewModel),
                    _ => throw new NotImplementedException()
                };
                engine.Children.Add(button);
            }
            static IHtmlElement createLabel([DisallowNull] UiPropertyViewModel prop) =>
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
                            new CommandCqrsSegregation(command.Name!, command.ParamsDto is null ? null : new(new(command.ParamsDto.Name, command.ParamsDto.NameSpace), null!), command.ResultDto is null ? null : new(new(command.ResultDto.Name, command.ResultDto.NameSpace), null!))),
                        _ => throw new NotImplementedException()
                    };
                }

                return button;
            }

            static BlazorCustomButton createCustomButton(UiViewModel model, CustomButtonViewModel customButtonViewModel)
            {
                var button = new BlazorCustomButton(name: customButtonViewModel.Name, body: customButtonViewModel.Caption, onClick: customButtonViewModel.EventHandlerName)
                {
                    Position = customButtonViewModel.Position.ToBootstrapPosition()
                };
                return button.SetAction(model.Name!, customButtonViewModel.CodeStatement);
            }
        }

        static void createGrid(UiViewModel model, BlazorComponent result)
        {
            var id = model.UiProperties.First(x => x.Name!.EqualsTo("id"));
            var idType = TypePath.New(id.Property.NotNull().Type.ToFullTypeName());
            foreach (var uiProp in model.UiProperties)
            {
                result.Properties.Add(new PropertyActor(uiProp.Property.NotNull().TypeFullName, uiProp.Name.NotNull(), uiProp.Caption));
            }
            foreach (var uiAction in model.UiActions.OfType<ButtonViewModel>())
            {
                var args = model.IsGrid && uiAction.Placement == Placement.RowButton ? new[] { new MethodArgument(idType, "id") } : null;
                result.Actions.Add(new(
                    uiAction.Name,
                    uiAction.Placement == Placement.RowButton,
                    uiAction.Caption,
                    Arguments: args,
                    EventHandlerName: uiAction.EventHandlerName,
                    Body: uiAction.Cast().As<CustomButtonViewModel>()?.CodeStatement?.ToString()));
            }
        }
    }

    #region Page Coding

    public Result<Codes> GenerateCodes(UiPageViewModel viewModel, GenerateCodesArgs? arguments)
    {
        if (!this.Validate(viewModel).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }
        this._logger.Debug($"Generating code is started.");
        var dataContextType = TypePath.New(viewModel.DataContext?.Name, viewModel.DataContext?.NameSpace);
        var page = (viewModel.Route.IsNullOrEmpty()
            ? BlazorPage.NewByModuleName(arguments?.FileName ?? viewModel.Name!, viewModel.Module.Name!)
            : BlazorPage.NewByPageRoute(arguments?.FileName ?? viewModel.Name!, viewModel.Route))
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

    public Result<UiPageViewModel> Validate(in UiPageViewModel? model) =>
    model.ArgumentNotNull().Check()
         .NotNull(x => x.Name)
         .NotNull(x => x.NameSpace)
         .NotNull(x => x.ClassName)
         .NotNull(x => x.DataContext)
         .NotNull(x => x.Module)
         //.NotNull(x => x.Route)
         .Build();

    #endregion Page Coding
}