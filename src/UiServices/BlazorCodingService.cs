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

using Microsoft.EntityFrameworkCore;

using Services.Helpers;

namespace Services;

internal sealed class BlazorCodingService(IDtoService dtoService,
                           IPropertyService propertyService,
                           IEntityViewModelConverter converter,
                           InfraReadDbContext readDbContext) : IBlazorComponentCodingService
{
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly IDtoService _dtoService = dtoService;
    private readonly IPropertyService _propertyService = propertyService;
    private readonly InfraReadDbContext _readDbContext = readDbContext;

    public bool ControlTypeHasPropertiesPage(ControlType controlType)
        => controlType switch
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

    public UiComponentPropertyViewModel CreateBoundPropertyByDto(DtoViewModel viewModel)
    {
        Check.MustBeArgumentNotNull(viewModel?.Name);

        return new UiComponentPropertyViewModel
        {
            Caption = viewModel.Name,
            ControlType = ControlTypeHelper.ByDtoViewModel(viewModel),
        };
    }

    public Task<UiComponentViewModel> CreateNewComponentAsync(CancellationToken cancellationToken = default)
    {
        var result = new UiComponentViewModel();
        return Task.FromResult(result);
    }

    public async Task<UiComponentViewModel> CreateNewComponentByDtoAsync(DtoViewModel dto, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(dto?.Id);
        var entity = (await this._dtoService.GetByIdAsync(dto.Id.Value, cancellationToken)).NotNull(() => new NotFoundValidationException());
        return this.CreateViewModel(dto);
    }

    public UiComponentCustomButtonViewModel CreateUnboundAction() =>
        new() { Caption = "New Action", IsEnabled = true, Placement = Placement.FormButton, Name = "NewAction", };

    public UiComponentPropertyViewModel CreateUnboundProperty() =>
        new() { Caption = "New Property", IsEnabled = true, ControlType = ControlType.None, Name = "UnboundProperty" };

    public UiComponentViewModel CreateViewModel(DtoViewModel dto)
    {
        _ = dto.Check()
            .ArgumentNotNull().ThrowOnFail()
            .NotNull(x => x.Name)
            .NotNull(x => x.NameSpace)
            .ThrowOnFail();

        var name = CommonHelpers.Purify(dto.Name)!;
        var parsedNameSpace = CommonHelpers.Purify(dto.NameSpace);
        var result = new UiComponentViewModel
        {
            Name = name,
            PageDataContext = dto,
            ClassName = name,
            NameSpace = parsedNameSpace,
            Guid = Guid.NewGuid(),
        };
        _ = result.UiProperties.AddRange(dto.Properties.Compact().Select(x => this._converter.ToUiComponentProperty(x)));
        return result;
    }

    public Task<UiComponentPropertyViewModel?> FillUiComponentPropertyViewModelAsync(UiComponentPropertyViewModel? prop, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(prop);
        var id = prop.Id.ArgumentNotNull(nameof(UiComponentPropertyViewModel.Id)).Value;
        return this.GetUiComponentPropertyByIdAsync(id, cancellationToken);
    }

    public Result<Codes> GenerateCodes(in UiComponentViewModel model, GenerateCodesParameters? arguments = null)
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

    public async Task<UiComponentPropertyViewModel?> GetUiComponentPropertyByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var query = from cp in this._readDbContext.UiComponentProperties
                    where cp.Id == id
                    select cp;
        var result = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return this._converter.ToViewModel(result);
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

    private static BlazorComponent CreateComponent(in UiComponentViewModel model)
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

        static (TypePath DataContextType, TypePath? DataContextPropType) createDataContext(UiComponentViewModel model, CancellationToken cancellationToken = default)
        {
            var dataContextType = TypePath.New(model.PageDataContext?.Name, model.PageDataContext?.NameSpace);
            var dataContextPropType = model.PageDataContextProperty is not null and { Name: not null }
                        ? TypePath.New(model.PageDataContextProperty.TypeFullName)
                        : null;
            return (dataContextType, dataContextPropType);
        }

        static BlazorComponent initializeComponent(UiComponentViewModel model, TypePath dataContextType) =>
            BlazorComponent.New(model.Name.NotNull())
                .With(x => x.NameSpace = model.NameSpace)
                .With(x => x.DataContextType = dataContextType)
                .With(x => x.IsGrid = model.IsGrid);

        static void setDataContext(UiComponentViewModel model, TypePath? dataContextPropType, BlazorComponent result, CancellationToken cancellationToken = default)
        {
            if (dataContextPropType is not { } prop)
            {
                return;
            }
            _ = result.With(x => x.DataContextProperty = (prop, model.Name!));
        }

        static void createChildren<TBlazorComponent>(in UiComponentViewModel model, in BlazorComponentBase<TBlazorComponent> engine, CancellationToken cancellationToken = default)
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
            foreach (var elementAction in model.UiActions.OfType<FrontElement>())
            {
                if (elementAction is UiComponentButtonViewModelBase buttonBase)
                {
                    if (buttonBase is UiComponentCqrsButtonViewModel cqrsButtonViewModel)
                    {
                        var button = createCqrsButton(model, cqrsButtonViewModel);
                        engine.Children.Add(button);
                    }
                    else if (buttonBase is UiComponentCustomButtonViewModel customButtonViewModel)
                    {
                        var button = createCustomButton(model, customButtonViewModel);
                        engine.Children.Add(button);
                    }
                }
            }
            static IHtmlElement createLabel([DisallowNull] UiComponentPropertyViewModel prop) =>
                new BlazorLabel($"{prop.ArgumentNotNull().Name}Label", body: prop.Caption.NotNull(New<NotFoundValidationException>))
                {
                    Position = prop.Position.ToBootstrapPosition().SetRow(1).SetCol(2),
                    IsEnabled = prop.IsEnabled
                };

            static BlazorCqrsButton createCqrsButton(UiComponentViewModel model, UiComponentCqrsButtonViewModel cqrsButtonViewModel)
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

            static BlazorCustomButton createCustomButton(UiComponentViewModel model, UiComponentCustomButtonViewModel customButtonViewModel)
            {
                var button = new BlazorCustomButton(name: customButtonViewModel.Name, body: customButtonViewModel.Caption, onClick: customButtonViewModel.EventHandlerName)
                {
                    Position = customButtonViewModel.Position.ToBootstrapPosition()
                };
                _ = button.SetAction(model.Name!, customButtonViewModel.CodeStatement);
                return button;
            }
        }

        static void createGrid(UiComponentViewModel model, BlazorComponent result)
        {
            var id = model.UiProperties.FirstOrDefault(x => x.Name.EqualsTo("id"));
            var idType = TypePath.New(id?.Property.Type.ToFullTypeName());
            foreach (var uiProp in model.UiProperties)
            {
                result.Properties.Add(new PropertyActor(uiProp.Property.TypeFullName, uiProp.Name, uiProp.Caption));
            }
            foreach (var uiAction in model.UiActions.OfType<UiComponentButtonViewModelBase>())
            {
                var args = model.IsGrid && uiAction.Placement == Placement.RowButton ? new[] { new MethodArgument(idType, "id") } : null;
                result.Actions.Add(new(
                    uiAction.Name,
                    uiAction.Placement == Placement.RowButton,
                    uiAction.Caption,
                    Arguments: args,
                    EventHandlerName: uiAction.EventHandlerName,
                    codeStatement: uiAction.Cast().As<UiComponentCustomButtonViewModel>()?.CodeStatement));
            }
        }
    }
}