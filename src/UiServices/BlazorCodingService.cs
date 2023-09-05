using System.Diagnostics.CodeAnalysis;
using System.IO;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Collections;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

using Services.Helpers;

namespace Services;

internal sealed class BlazorCodingService : IBlazorComponentCodingService
{
    private readonly IEntityViewModelConverter _converter;
    private readonly IDtoService _dtoService;
    private readonly IPropertyService _propertyService;
    private readonly InfraReadDbContext _readDbContext;

    public BlazorCodingService(IDtoService dtoService,
                               IPropertyService propertyService,
                               IEntityViewModelConverter converter,
                               InfraReadDbContext readDbContext)
    {
        this._dtoService = dtoService;
        this._propertyService = propertyService;
        this._converter = converter;
        this._readDbContext = readDbContext;
    }

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

    public UiComponentViewModel CreateNewComponentByDto(DtoViewModel dto)
    {
        Check.MustBeArgumentNotNull(dto);
        var parsedName = dto.Name?.Replace("Dto", "Component").Replace("ViewModel", "Component") ?? string.Empty;
        var parsedNameSpace = dto.NameSpace.Replace("Dto", "Component").Replace("ViewModel", "Component");
        var result = new UiComponentViewModel
        {
            Name = parsedName,
            PageDataContext = dto,
            ClassName = parsedName,
            NameSpace = parsedNameSpace,
            Guid = Guid.NewGuid()
        };
        _ = result.UiProperties.AddRange(dto.Properties.Compact().Select(x => this._converter.ToUiComponentProperty(x)));
        return result;
    }

    public async Task<UiComponentViewModel> CreateNewComponentByDtoAsync(DtoViewModel dto, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(dto?.Id);
        var entity = (await this._dtoService.GetByIdAsync(dto.Id.Value, cancellationToken)).NotNull(() => new NotFoundValidationException());
        return this.CreateNewComponentByDto(dto);
    }

    public UiComponentActionViewModel CreateUnboundAction() =>
        new() { Caption = "New Action", IsEnabled = true, TriggerType = TriggerType.Button, Name = "NewAction", };

    public UiComponentPropertyViewModel CreateUnboundProperty() =>
        new() { Caption = "New Property", IsEnabled = true, ControlType = ControlType.None, Name = "UnboundProperty" };

    public Task<UiComponentPropertyViewModel?> FillUiComponentPropertyViewModelAsync(UiComponentPropertyViewModel? prop, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(prop);
        var id = prop.Id.ArgumentNotNull(nameof(UiComponentPropertyViewModel.Id)).Value;
        return this.GetUiComponentPropertyByIdAsync(id, cancellationToken);
    }

    public GenerateCodeResult GenerateBlazorCodeBehinds(in UiComponentViewModel model, GenerateCodesParameters? arguments = null)
    {
        arguments ??= new GenerateCodesParameters();
        var engine = CreateComponent(model);
        var result = engine.GenerateBehindCode(arguments);
        return result;
    }

    public Code? GenerateBlazorHtmlCode(in UiComponentViewModel model)
        => CreateComponent(model).GenerateUiCode();

    public Result<Codes> GenerateCodes(in UiComponentViewModel model, GenerateCodesParameters? arguments = null)
        => new(CreateComponent(model).GenerateCodes(arguments ?? new GenerateCodesParameters()));

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

    public async Task SaveToPathAsync(UiComponentViewModel viewModel, string path, GenerateCodesParameters? arguments = null, CancellationToken cancellationToken = default)
    {
        Check.MutBeNotNull(viewModel);
        Check.MutBeNotNull(viewModel.ClassName);
        Check.MutBeNotNull(path);

        var codes = this.GenerateCodes(viewModel, arguments).Value;
        foreach (var code in codes.Compact())
        {
            await File.WriteAllTextAsync(Path.Combine(path, code.FileName), code.Statement, cancellationToken: cancellationToken);
        }
    }

    public Task SaveToPathAsync(UiPageViewModel viewModel, string path, GenerateCodesParameters? arguments = null, CancellationToken cancellationToken = default)
    {
        var page = CreatePage(viewModel, cancellationToken);
        var codes = page.GenerateCodes(arguments);
        var writeTasks = TaskList.New();
        foreach (var code in codes.Compact())
        {
            _ = writeTasks.Add(File.WriteAllTextAsync(Path.Combine(path, code.Name), code.Statement, cancellationToken: cancellationToken));
        }
        return writeTasks.WhenAllAsync();
    }

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

        static BlazorComponent initializeComponent(UiComponentViewModel model, TypePath dataContextType)
            => BlazorComponent.New(model.Name!)
                              .SetNameSpace(model.NameSpace)
                              .SetDataContext(dataContextType)
                              .SetIsGrid(model.IsGrid);

        static void setDataContext(UiComponentViewModel model, TypePath? dataContextPropType, BlazorComponent result, CancellationToken cancellationToken = default)
        {
            if (dataContextPropType is not { } prop)
            {
                return;
            }
            _ = result.SetDataContextProperty(prop, model?.Name);
        }

        static void createChildren<TBlazorComponent>(in UiComponentViewModel model, in BlazorComponentBase<TBlazorComponent> engine, CancellationToken cancellationToken = default)
            where TBlazorComponent : BlazorComponentBase<TBlazorComponent>
        {
            foreach (var prop in model.UiProperties)
            {
                var bindPropName = $"this.DataContext.{prop.Name}";
                switch (prop.ControlType)
                {
                    case ControlType.None:
                        break;

                    case ControlType.RadioButton:
                        break;

                    case ControlType.CheckBox:
                        engine.Children.Add(createLabel(prop));
                        engine.Children.Add(new BlazorCheckBox($"{prop.Name}CheckBox", bind: bindPropName) { Position = prop.Position.ToBootstrapPosition() });
                        break;

                    case ControlType.TextBox:
                        engine.Children.Add(createLabel(prop));
                        engine.Children.Add(new BlazorTextBox($"{prop.Name}TextBox", bind: bindPropName) { Position = prop.Position.ToBootstrapPosition() });
                        break;

                    case ControlType.DateTimePicker:
                        engine.Children.Add(createLabel(prop));
                        engine.Children.Add(new BlazorDatePicker($"{prop.Name}DatePicker", bind: bindPropName) { Position = prop.Position.ToBootstrapPosition() });
                        break;

                    case ControlType.NumericTextBox:
                        engine.Children.Add(createLabel(prop));
                        engine.Children.Add(new BlazorNumericTextBox($"{prop.Name}NumericTextBox", bind: bindPropName) { Position = prop.Position.ToBootstrapPosition() });
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
                        engine.Children.Add(new BlazorTable($"{bindPropName}Grid", bind: bindPropName) { Position = prop.Position.ToBootstrapPosition() });
                        break;

                    case ControlType.ExternalViewBox:
                        break;

                    case ControlType.Component:
                        break;
                }
            }
            foreach (var action in model.UiActions)
            {
                if (action.TriggerType.GetKind() == TriggerKind.Button)
                {
                    HtmlButton button = new BlazorButton(name: action.Name, body: action.Caption, type: action.TriggerType.ToButtonType())
                    {
                        Position = action.Position.ToBootstrapPosition()
                    };
                    if (action.CqrsSegregate?.Name is not null)
                    {
                        button = action.CqrsSegregate switch
                        {
                            CqrsQueryViewModel query => button.SetAction(
                                query.Name!,
                                new QueryCqrsSergregation(query.Name!, new(model.PageDataContextType, null!), query.ResultDto?.Name.IsNullOrEmpty() ?? true ? null : new(query.ResultDto.Name, null!))),
                            CqrsCommandViewModel command => button.SetAction(
                                command.Name!,
                                new CommandCqrsSergregation(command.Name!, command.ParamsDto is null ? null : new(new(command.ParamsDto.Name, command.ParamsDto.NameSpace), null!), command.ResultDto is null ? null : new(new(command.ResultDto.Name, command.ResultDto.NameSpace), null!))),
                            _ => throw new NotImplementedException()
                        };
                    }
                    engine.Children.Add(button);
                }
            }
            static IHtmlElement createLabel([DisallowNull] UiComponentPropertyViewModel prop)
                => new BlazorLabel($"{prop.ArgumentNotNull().Name}Label", body: prop.Caption.NotNull(New<NotFoundValidationException>))
                {
                    Position = prop.Position.ToBootstrapPosition().SetColSpan(1)
                };
        }

        static void createGrid(UiComponentViewModel model, BlazorComponent result)
        {
            var id = model.UiProperties.FirstOrDefault(x => x.Name.EqualsTo("id"));
            var idType = TypePath.New(id?.Property.Type.ToFullTypeName());
            //foreach (var uiProp in model.UiProperties)
            //{
            //    result.Properties.Add(new PropertyActor(null, uiProp.Name, uiProp.Caption));
            //}
            foreach (var uiAction in model.UiActions)
            {
                var args = model.IsGrid ? new[] { new MethodArgument(idType, "id") } : null;
                result.Actions.Add(new(uiAction.Name, uiAction.Caption, Arguments: args, EventHandlerName: uiAction.EventHandlerName));
            }
        }
    }

    private static BlazorPage CreatePage(in UiPageViewModel model, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(model);
        Check.MustBeArgumentNotNull(model.Name, nameof(model.Name));

        var dataContextType = TypePath.New(model.Dto?.Name, model.Dto?.NameSpace);
        var result = BlazorPage.New(model.Name).SetPageRoute(model.Route).SetNameSpace(model.NameSpace).SetDataContext(dataContextType);
        return result;
    }
}