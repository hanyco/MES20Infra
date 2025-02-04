﻿using System.Diagnostics.CodeAnalysis;
using System.Text;

using HanyCo.Infra.CodeGen.Domain;
using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Validations;

using Services.Helpers;

using static HanyCo.Infra.CodeGeneration.Definitions.CodeConstants;

using ButtonViewModelBase = HanyCo.Infra.CodeGen.Domain.ViewModels.UiComponentButtonViewModelBase;
using CqrsButtonViewModel = HanyCo.Infra.CodeGen.Domain.ViewModels.UiComponentCqrsButtonViewModel;
using CqrsLoadViewModel = HanyCo.Infra.CodeGen.Domain.ViewModels.UiComponentCqrsLoadViewModel;
using CstmButtonViewModel = HanyCo.Infra.CodeGen.Domain.ViewModels.UiComponentCustomButton;
using CstmLoadViewModel = HanyCo.Infra.CodeGen.Domain.ViewModels.UiComponentCustomLoad;
using PropertyViewModel = HanyCo.Infra.CodeGen.Domain.ViewModels.UiPropertyViewModel;
using UiViewModel = HanyCo.Infra.CodeGen.Domain.ViewModels.UiComponentViewModel;

namespace Services;

internal sealed class BlazorCodingService(ILogger logger, IMapperSourceGenerator mapperSourceGenerator, ICodeGeneratorEngine codeGenerator) 
    : IBlazorComponentCodeService, IBlazorPageCodeService, IValidator<UiPageViewModel>
{
    private readonly Queue<CqrsViewModelBase> _conversionSubjects = [];
    private readonly ILogger _logger = logger;
    private readonly ICodeGeneratorEngine _codeGenerator = codeGenerator;

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
            var component = createComponent(model, _codeGenerator);
            processBackendActions(model, component);
            processFrontActions(model, component);
            addParameters(model, component);            
            var codes = component.GenerateCodes(CodeCategory.Component, arguments)
                //.AddRange(this.GenerateModelConverterCode())
                ;
            return Result.Success<Codes>(codes);
        }
        catch (Exception ex)
        {
            return Result.Fail<Codes>(ex, Codes.Empty);
        }

        static BlazorComponent createComponent(in UiViewModel model, in ICodeGeneratorEngine codeGenerator)
        {
            Check.MustBeArgumentNotNull(model.Name);

            var (dataContextType, dataContextPropType) = createDataContext(model);
            var result = getNewComponent(model, dataContextType, codeGenerator);
            setDataContext(model, dataContextPropType, result);
            return result;

            static (TypePath DataContextType, TypePath? DataContextPropType) createDataContext(UiViewModel model)
            {
                var dataContextType = TypePath.New(model.PageDataContext!.Name, model.PageDataContext!.NameSpace);
                var dataContextPropType = model.PageDataContextProperty is not null and { Name: not null }
                    ? TypePath.New(model.PageDataContextProperty.TypeFullName)
                    : null;
                return (dataContextType, dataContextPropType);
            }

            static BlazorComponent getNewComponent(UiViewModel model, TypePath dataContextType, ICodeGeneratorEngine codeGenerator) =>
                BlazorComponent.New(model.Name!, codeGenerator)
                    .With(x => x.NameSpace = model.NameSpace)
                    .With(x => x.IsGrid = model.IsGrid)
                    .With(x => x.DataContextType = dataContextType)
                    .With(x => x.AdditionalUsings.AddRange(model.AdditionalUsings))
                    .With(x => x.AdditionalInjects.AddRange(model.AdditionalInjects));

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
                var cssClass = "error-message";
                switch (prop.ControlType)
                {
                    case ControlType.None:
                        break;

                    case ControlType.RadioButton:
                        break;

                    case ControlType.CheckBox:
                        result.Children.Add(createLabel(prop));
                        result.Children.Add(new BlazorCheckBox($"{prop.Name}CheckBox", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
                        result.Children.Add(new ValidationMessage(bindPropName, cssClass));
                        break;

                    case ControlType.TextBox:
                        result.Children.Add(createLabel(prop));
                        result.Children.Add(new BlazorTextBox($"{prop.Name}TextBox", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
                        result.Children.Add(new ValidationMessage(bindPropName, cssClass));
                        break;

                    case ControlType.DateTimePicker:
                        result.Children.Add(createLabel(prop));
                        result.Children.Add(new BlazorDatePicker($"{prop.Name}DatePicker", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
                        result.Children.Add(new ValidationMessage(bindPropName, cssClass));
                        break;

                    case ControlType.NumericTextBox:
                        result.Children.Add(createLabel(prop));
                        result.Children.Add(new BlazorNumericTextBox($"{prop.Name}NumericTextBox", bind: bindPropName) { Position = position, IsEnabled = prop.IsEnabled });
                        result.Children.Add(new ValidationMessage(bindPropName, cssClass));
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
                var button = new BlazorCqrsButton(name: cqrsButtonViewModel.Name, body: cqrsButtonViewModel.Caption, onClick: cqrsButtonViewModel.EventHandlerName, onClickReturnType: cqrsButtonViewModel.ReturnType)
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

            static BlazorCustomButton createCstmButton(UiViewModel model, CstmButtonViewModel button)
            {
                var btn = new BlazorCustomButton(name: button.Name, body: button.Caption, onClick: button.EventHandlerName, type: button.ButtonType)
                {
                    Position = button.Position.ToBootstrapPosition(),
                    OnClickReturnType = button.ReturnType,
                };
                return btn.SetAction(model.Name!, button.CodeStatement);
            }
            static string InstanceDataContextProperty(string? name) =>
                $"this.DataContext.{name}";
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
                        var args = button.Placement == Placement.RowButton ? new[] { new MethodArgument(idType, "id") } : null;
                        result.Actions.Add(new ButtonActor(
                            button.Name,
                            button.Placement == Placement.RowButton,
                            button.Caption,
                            arguments: args,
                            eventHandlerName: button.EventHandlerName,
                            body: button.Cast().As<CstmButtonViewModel>()?.CodeStatement, returnType: button.ReturnType));
                        break;
                }
            }

            result.Properties.Clear();
            foreach (var property in model.Properties)
            {
                result.Properties.Add(new PropertyActor(null!, property.Name!, property.Caption, BindingName: property.Property?.DbObject?.Name));
            }
            return result;
        }

        void processBackendActions(in UiViewModel model, in BlazorComponent result)
        {
            result.AddUsings(model.AdditionalUsings);
            foreach (var action in model.Actions.OfType<BackElement>())
            {
                switch (action)
                {
                    case CqrsLoadViewModel load when load.CqrsSegregate is not null:
                        this._conversionSubjects.Enqueue(load.CqrsSegregate);
                        result.Actions.Add(new(Keyword_AddToOnInitializedAsync, true, body:CodeSnippets.ExecuteCqrs_MethodBody(load.CqrsSegregate)));
                        _ = result.AdditionalUsings.Add(load.CqrsSegregate.CqrsNameSpace);
                        _ = result.AdditionalUsings.Add(load.CqrsSegregate.DtoNameSpace);
                        //_ = result.AdditionalUsings.Add(load.CqrsSegregate.MapperNameSpace);
                        break;

                    case CqrsLoadViewModel load:
                        throw new InvalidOperationValidationException("`OnCqrsLoad` method has not required fields.");

                    case CstmLoadViewModel load when load.CodeStatement != null:
                        result.Actions.Add(new(Keyword_AddToOnInitializedAsync, false, load.CodeStatement));
                        break;

                    case CstmLoadViewModel load:
                        throw new InvalidOperationValidationException("`OnCustomLoad` method has not required fields.");
                }
            }
            if (model.EditFormInfo.IsEditForm == true || (!model.IsGrid && model.EditFormInfo.IsEditForm != false))
            {
                foreach (var evt in model.EditFormInfo.Events)
                {
                    result.Actions.Add(new FormActor(evt.Handler.Name, evt.IsPartial, evt.Handler.Body, evt.Handler.ReturnType?.FullPath ?? "void",
                        arguments: evt.Handler.Arguments.Select(x => new MethodArgument(x.Type, x.Name)).ToArray()));
                }
            }
        }

        static void processFrontActions(UiViewModel model, BlazorComponent component)
        {
            if (model.IsGrid)
            {
                _ = createGrid(model, component);
            }
            else
            {
                _ = createForm(model, component);
            }
        }

        static void addParameters(UiViewModel model, BlazorComponent component)
        {
            foreach (var parameter in model.Parameters)
            {
                component.Parameters.Add(new(parameter.Type, parameter.Name));
            }
        }
    }

    public Result<Codes> GenerateCodes(UiPageViewModel viewModel, GenerateCodesParameters? arguments)
    {
        if (!this.Validate(viewModel).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }
        this._logger.Debug($"Generating code is started.");
        var dataContextType = TypePath.New(viewModel.DataContext.Name, viewModel.DataContext.NameSpace);
        var page = (!viewModel.Routes.Any()
            ? BlazorPage.NewByModuleName(arguments?.BackendFileName ?? viewModel.Name!, viewModel.Module.Name!, _codeGenerator)
            : BlazorPage.NewByPageRoute(arguments?.BackendFileName ?? viewModel.Name!, viewModel.Routes, _codeGenerator))
                .With(x => x.NameSpace = viewModel.NameSpace)
                .With(x => x.DataContextType = dataContextType);

        foreach (var parameter in viewModel.Parameters)
        {
            page.Parameters.Add(new(parameter.Type, parameter.Name));
        }
        _ = page.Children.AddRange(viewModel.Components.Select(x => toHtmlElement(x, dataContextType, x.PageDataContextProperty is null ? null : (new TypePath(x.PageDataContextProperty.TypeFullName), x.PageDataContextProperty.Name!), _codeGenerator)));
        page.AddUsings(viewModel.AdditionalUsings);
        var result = page.GenerateCodes(CodeCategory.Page, arguments);
        this._logger.Debug($"Generating code is done.");

        return Result.Success<Codes>(result);

        static IHtmlElement toHtmlElement(UiViewModel component, string? dataContextType, (TypePath Type, string Name)? dataContextTypeProperty, ICodeGeneratorEngine codeGenerator) =>
            BlazorComponent.New(component.Name!, codeGenerator)
                .With(x => x.NameSpace = component.NameSpace)
                .With(x => x.DataContextType = dataContextType)
                .With(x => x.DataContextProperty = dataContextTypeProperty)
                .With(x => x.Attributes.AddRange(component.Attributes.Select(x => new KeyValuePair<string, string?>(x.Key, x.Value))))
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

    [Obsolete("Mappers are no longer used in this project", true)]
    private IEnumerable<Code> GenerateModelConverterCode()
    {
        while (this._conversionSubjects.TryDequeue(out var conversionSubject))
        {
            var args = new MapperSourceGeneratorArguments(conversionSubject.ResultDto, conversionSubject.ResultDto, conversionSubject.MapperNameSpace);
            var codes = mapperSourceGenerator.GenerateCodes(args);
            foreach (var code in codes.Value)
            {
                yield return code!;
            }
        }
    }
}