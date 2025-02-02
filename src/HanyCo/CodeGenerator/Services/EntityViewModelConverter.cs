using HanyCo.Infra.CodeGen.Domain;
using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Data.Markers;
using Library.Mapping;
using Library.Validations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using Newtonsoft.Json;

using Services.Helpers;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Controller = HanyCo.Infra.Internals.Data.DataSources.Controller;

namespace Services;

internal sealed class EntityViewModelConverter(IMapper mapper, ILogger logger) : IEntityViewModelConverter
{
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;

    ILogger ILoggerContainer.Logger => this._logger;


    [return: NotNullIfNotNull(nameof(entity))]
    public TEntity MapId<TEntity>(in TEntity entity, InfraViewModelBase model) where TEntity : IIdenticalEntity =>
        entity.With(x => x.Id = model.Id ?? 0);

    [return: NotNullIfNotNull(nameof(model))]
    public TInfraViewModel MapId<TInfraViewModel>(in TInfraViewModel model, IIdenticalEntity entity) where TInfraViewModel : InfraViewModelBase =>
        model.With(x => x.Id = entity.Id);

    public UiComponentAction? ToDbEntity(UiComponentButtonViewModelBase? model) =>
                model is null ? null : this._mapper.Map<UiComponentAction>(model)
            .ForMember(x => x.TriggerTypeId = model.Placement.Cast().ToInt())
            // TODO: Think about how to save component complexity in database
            //?.ForMember(x => x.CqrsSegregateId = model.CqrsSegregate?.Id)
            .ForMember(x => x.Position = this.ToDbEntity(model.Position)!);

    public UiComponentProperty? ToDbEntity(UiPropertyViewModel? model) =>
        model is null ? null : this._mapper.Map<UiComponentProperty>(model)
            .ForMember(x => x.Position = this.ToDbEntity(model.Position)!)
            .ForMember(x => x.PropertyId = model.Property?.Id)
            .ForMember(x => x.ControlTypeId = model.ControlType?.Cast().ToInt() ?? 0);

    public UiComponent? ToDbEntity(UiComponentViewModel? model) =>
        model is null ? null : this._mapper.Map<UiComponent>(model)
            .ForMember(x => x.PageDataContextId = model.PageDataContext?.Id)
            .ForMember(x => x.PageDataContextPropertyId = model.PageDataContextProperty?.Id)
            .ForMember(x => x.IsGrid = model.IsGrid)
            .Fluent()
            .IfTrue(model.Properties.Any() is true, x => x.UiComponentProperties = [.. model.Properties.Select(this.ToDbEntity).Compact()]);

    public HanyCo.Infra.Internals.Data.DataSources.Module? ToDbEntity(ModuleViewModel? model) =>
        model is null ? null : this._mapper.Map<HanyCo.Infra.Internals.Data.DataSources.Module>(model);

    public UiBootstrapPosition? ToDbEntity(UiBootstrapPositionViewModel? model) =>
        model is null ? null : this._mapper.Map<UiBootstrapPosition>(model);

    public UiPage? ToDbEntity(UiPageViewModel? model) =>
        model is null ? null : this._mapper.Map<UiPage>(model)
            .ForMember(x => x.DtoId = model.DataContext?.Id)
            .ForMember(x => x.Dto = null)
            .ForMember(x => x.ModuleId = model.Module.Id ?? default)
            .ForMember(x => x.Module = null!)
            .ForMember(x => x.UiPageComponents.AddRange(model.Components.Select(cmp =>
            {
                var uiPageComponent = new UiPageComponent
                {
                    PageId = model.Id ?? default,
                    Position = this.ToDbEntity(cmp.Position),
                    PositionId = cmp.Position.Id,
                    UiComponentId = cmp.Id ?? default,
                    UiComponent = null!,
                    //Id = cmp.PageComponentId ?? long.MinValue
                };
                if (cmp.PageComponentId is { } upcid)
                {
                    uiPageComponent.Id = upcid;
                }

                return uiPageComponent;
            })));

    public Property? ToDbEntity(PropertyViewModel? viewModel)
    {
        if (viewModel is null)
        {
            return null;
        }
        var result = this._mapper.MapExcept<Property>(viewModel, x => x.Id)
            .ForMember(x => x.ParentEntityId = viewModel.ParentEntityId)
            .ForMember(x => x.DbObjectId = viewModel.DbObject?.ToDbFormat())
            .ForMember(x => x.PropertyType = viewModel.Type.Cast().ToInt())
            .ForMember(x => x.DtoId = viewModel.Dto?.Id);
        if (viewModel.Id > 0)
        {
            result.Id = viewModel.Id.Value;
        }
        return result;
    }

    public Dto? ToDbEntity(DtoViewModel? model)
    {
        if (model is null)
        {
            return null;
        }

        var result = this._mapper.MapExcept<Dto>(model, x => x.Id)
            .ForMember(x => x.Module = null)
            .ForMember(x => x.DbObjectId = model.DbObject?.ToDbFormat());
        if (model.Id > 0)
        {
            model.Id = model.Id!.Value;
        }

        if (model.Module?.Id is { } moduleId)
        {
            result.ModuleId = moduleId;
        }
        _ = (result.Properties?.AddRange(model.Properties.Select(this.ToDbEntity).Compact()));
        return result;
    }


    public CqrsSegregate? ToDbEntity(CqrsQueryViewModel? model) =>
        this.CqrsViewModelToDbEntityInner(model, CqrsSegregateType.Query);


    public CqrsSegregate? ToDbEntity(CqrsCommandViewModel? model) =>
        this.CqrsViewModelToDbEntityInner(model, CqrsSegregateType.Command);

    public Functionality? ToDbEntity(FunctionalityViewModel? model)
    {
        if (model == null)
        {
            return null;
        }

        var result = this._mapper.MapExcept<Functionality>(model, x => x.Id);
        if (model.Id is not null and not 0)
        {
            result.Id = model.Id.Value;
        }

        result.GetAllQuery = this.ToDbEntity(model.GetAllQuery);
        result.GetByIdQuery = this.ToDbEntity(model.GetByIdQuery);
        result.InsertCommand = this.ToDbEntity(model.InsertCommand);
        result.UpdateCommand = this.ToDbEntity(model.UpdateCommand);
        result.DeleteCommand = this.ToDbEntity(model.DeleteCommand);
        result.SourceDto = this.ToDbEntity(model.SourceDto);
        result.Controller = this.ToDbEntity(model.Controller);
        if (model.Module?.Id is > 0)
        {
            result.Module = null;
            result.ModuleId = model.Module.Id.Value;
        }
        else
        {
            result.Module = this.ToDbEntity(model.Module);
        }

        return result;
    }


    public CqrsSegregate? ToDbEntity(CqrsViewModelBase? model)
    {
        if (model == null)
        {
            return null;
        }

        var result = this._mapper
            .MapExcept<CqrsSegregate>(model, x => x.Id)
            .ForMember(x => x.Module = null!)
            .ForMember(x => x.ParamDto = this.ToDbEntity(model.ParamsDto))
            .ForMember(x => x.ResultDto = this.ToDbEntity(model.ResultDto));
        if (model.Id is not null and not 0)
        {
            result.Id = model.Id.Value;
        }
        if (model.Module?.Id is { } moduleId)
        {
            result.ModuleId = moduleId;
        }
        if (model.ParamsDto?.Id is { } paramsDtoId)
        {
            result.ParamDtoId = paramsDtoId;
        }
        if (model.ResultDto?.Id is { } resultDtoId)
        {
            result.ResultDtoId = resultDtoId;
        }

        return result;
    }


    public Controller? ToDbEntity(ControllerViewModel? model)
    {
        if (model == null)
        {
            return null;
        }

        var result = this._mapper
            .MapExcept<Controller>(model, x => x.Id)
            .ForMember(x => x.AdditionalUsings = model.AdditionalUsings.Merge(";"))
            .ForMember(x => x.ControllerMethods!.AddRange(model.Apis.Select(this.ToDbEntity)))
            .ForMember(x => x.ControllerName = model.Name!)
            .ForMember(x => x.ControllerRoute = model.Route)
            .ForMember(x => x.Module = null!);
        if (model.Id is not null and not 0)
        {
            result.Id = model.Id.Value;
        }
        if (model.Module?.Id is { } moduleId)
        {
            result.ModuleId = moduleId;
        }

        return result;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public ControllerMethod? ToDbEntity(ControllerMethodViewModel? model)
    {
        if (model == null)
        {
            return null;
        }
        var result = this._mapper.MapExcept<ControllerMethod>(model, x => x.Id);
        if (model.Id is not null and not 0)
        {
            result.Id = model.Id.Value;
        }
        //result.

        return result;
    }

    public PropertyViewModel? ToPropertyViewModel(DbColumnViewModel? columnViewModel) =>
        columnViewModel == null
            ? null
            : new(columnViewModel);

    public UiPropertyViewModel? ToUiComponentProperty(in PropertyViewModel? propertyViewModel) =>
        propertyViewModel == null ? null : new()
        {
            Name = propertyViewModel.Name,
            Property = propertyViewModel,
            ControlType = propertyViewModel.Type.ToControlType(propertyViewModel.IsList, propertyViewModel.IsNullable, propertyViewModel.Dto).Control,
            Caption = string.Equals(propertyViewModel.Name, "id", global::System.StringComparison.Ordinal)
                ? propertyViewModel.Name
                : propertyViewModel.Name.SeparateCamelCase(),
            IsEnabled = !propertyViewModel.Name?.EqualsTo("id") ?? false
        };

    [return: NotNullIfNotNull(nameof(entity))]
    public DtoViewModel? ToViewModel(Dto? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var result = this._mapper.Map<DtoViewModel>(entity)
            .ForMember(x => x.DbObject = DbObjectViewModel.FromDbFormat(entity.DbObjectId));

        if (entity.Properties?.Count > 0)
        {
            _ = result.Properties!.AddRange(entity.Properties.Select(this.ToViewModel));
        }
        if (entity.Module is not null)
        {
            result.Module = this.ToViewModel(entity.Module);
        }
        if (!entity.BaseType.IsNullOrEmpty())
        {
            result.BaseType = TypePath.New(entity.BaseType);
        }
        return result;
    }

    [return: NotNullIfNotNull(nameof(entity))]
    public ModuleViewModel? ToViewModel(HanyCo.Infra.Internals.Data.DataSources.Module? entity) =>
        entity is null ? null : this._mapper.Map<ModuleViewModel>(entity);

    [return: NotNullIfNotNull(nameof(entity))]
    public UiBootstrapPositionViewModel? ToViewModel(UiBootstrapPosition? entity) =>
        entity is null ? null : this._mapper.Map<UiBootstrapPositionViewModel>(entity);

    [return: NotNullIfNotNull(nameof(entity))]
    public UiComponentViewModel? ToViewModel(UiComponent? entity) =>
        entity is null ? null : this._mapper.Map<UiComponentViewModel>(entity)
            .ForMember(x => x.IsGrid = entity.IsGrid ?? false)
            .ForMember(x => x.Properties!.AddRange(entity.UiComponentProperties.Select(this.ToViewModel)))
            .ForMember(x => x.Actions!.AddRange(entity.UiComponentActions.Select(this.ToViewModel).Compact()))
            .ForMember(x => x.PageDataContext = this.ToViewModel(entity.PageDataContext))
            .ForMember(x => x.PageDataContextProperty = this.ToViewModel(entity.PageDataContextProperty));

    public UiComponentButtonViewModelBase? ToViewModel(UiComponentAction? entity)
    {
        if (entity is null)
        {
            return null;
        }

        if (entity.CqrsSegregate is not null)
        {
            return this._mapper.Map<UiComponentCqrsButtonViewModel>(entity)
                .ForMember(x => x.Position = this.ToViewModel(entity.Position) ?? new())
                .ForMember(x => x.CqrsSegregate = this.ToViewModel(entity.CqrsSegregate))
                .ForMember(x => x.Placement = EnumHelper.ToEnum<Placement>(entity.TriggerTypeId));
        }
        ;

        return this._mapper.Map<UiComponentCustomButton>(entity)
            .ForMember(x => x.Position = this.ToViewModel(entity.Position) ?? new())
            .ForMember(x => x.Placement = EnumHelper.ToEnum<Placement>(entity.TriggerTypeId));
    }

    public UiPropertyViewModel? ToViewModel(UiComponentProperty? entity) =>
        entity is null ? null : this._mapper.Map<UiPropertyViewModel>(entity)
             .ForMember(x => x.Property = this.ToViewModel(entity.Property))
             .ForMember(x => x.ControlType = ControlTypeHelper.FromControlTypeId(entity.ControlTypeId));

    public UiPageViewModel? ToViewModel(UiPage? entity) =>
        entity is null ? null : this._mapper.Map<UiPageViewModel>(entity)
            .ForMember(x => x.Components.AddRange(entity.UiPageComponents.Select(this.ToViewModel).Compact().OrderBy(x => x.Position)))
            .ForMember(x => x.DataContext = this.ToViewModel(entity.Dto))
            .ForMember(x => x.Module = null);

    public PropertyViewModel? ToViewModel(Property? entity) =>
        entity is null ? null : this._mapper.Map<PropertyViewModel>(entity)
            .ForMember(x => x.TypeFullName = entity.TypeFullName!)
            .ForMember(x => x.Type = PropertyTypeHelper.FromPropertyTypeId(entity.PropertyType))
            .ForMember(x => x.Dto = this.ToViewModel(entity.Dto))
            .ForMember(x => x.DbObject = DbColumnViewModel.FromDbObjectViewModel(DbObjectViewModel.FromDbFormat(entity.DbObjectId)));

    [return: NotNullIfNotNull(nameof(CqrsSegregate))]
    public CqrsViewModelBase? ToViewModel(CqrsSegregate? entity) =>
        entity is null ? null : EnumHelper.ToEnum<CqrsSegregateType>(entity.SegregateType) switch
        {
            CqrsSegregateType.Command => this._mapper.Map<CqrsCommandViewModel>(entity)
                .ForMember(x => x.Module = this.ToViewModel(entity.Module))
                .ForMember(x => x.ParamsDto = this.ToViewModel(entity.ParamDto))
                .ForMember(x => x.ResultDto = this.ToViewModel(entity.ResultDto)),
            CqrsSegregateType.Query => this._mapper.Map<CqrsQueryViewModel>(entity)
                .ForMember(x => x.Module = this.ToViewModel(entity.Module))
                .ForMember(x => x.ParamsDto = this.ToViewModel(entity.ParamDto))
                .ForMember(x => x.ResultDto = this.ToViewModel(entity.ResultDto)),
            _ => null,
        };

    public UiComponentViewModel? ToViewModel(UiPageComponent? entity)
        => entity is null ? null : this._mapper.Map<UiComponentViewModel>(entity)
            .ForMember(x => x.Id = entity.UiComponentId)
            .ForMember(x => x.Name = entity.UiComponent.Name)
            .ForMember(x => x.NameSpace = entity.UiComponent.NameSpace)
            .ForMember(x => x.UiPageComponentId = entity.Id)
            .ForMember(x => x.PageComponentId = entity.Id)
            .ForMember(x => x.Position.FillWith(this.ToViewModel(entity.Position)!))
            .ForMember(x => x.PageDataContext = this.ToViewModel(entity.UiComponent.PageDataContext))
            .ForMember(x => x.PageDataContextProperty = this.ToViewModel(entity.UiComponent.PageDataContextProperty));

    public FunctionalityViewModel? ToViewModel(Functionality? entity)
        => entity == null ? null : this._mapper.Map<FunctionalityViewModel>(entity)
            .ForMember(x => x.SourceDto = this.ToViewModel(entity.SourceDto))
            .ForMember(x => x.GetAllQuery = this.ToQueryViewModel(entity.GetAllQuery))
            .ForMember(x => x.GetByIdQuery = this.ToQueryViewModel(entity.GetByIdQuery))
            .ForMember(x => x.InsertCommand = this.ToCommandViewModel(entity.InsertCommand))
            .ForMember(x => x.UpdateCommand = this.ToCommandViewModel(entity.UpdateCommand))
            .ForMember(x => x.DeleteCommand = this.ToCommandViewModel(entity.DeleteCommand))
            .ForMember(x => x.Module = this.ToViewModel(entity.Module))
            .ForMember(x => x.Controller = this.ToViewModel(entity.Controller))
            ;

    [return: NotNullIfNotNull(nameof(entity))]
    public ControllerViewModel? ToViewModel(Controller? entity)
    {
        if (entity is null)
        {
            return null;
        }
        var result = this._mapper.Map<ControllerViewModel>(entity)
            .ForMember(x => x.AdditionalUsings.ClearAndAddRange((entity.AdditionalUsings??string.Empty).Split(';')))
            .ForMember(x => x.Apis.AddRange(entity.ControllerMethods.Select(this.ToViewModel)))
            .ForMember(x => x.Name = entity.ControllerName)
            .ForMember(x => x.Route = entity.ControllerRoute)
            .ForMember(x => x.Module = this.ToViewModel(entity.Module));
        return result;
    }

    [return: NotNullIfNotNull(nameof(entity))]
    public ControllerMethodViewModel? ToViewModel(ControllerMethod? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var viewModel = new ControllerMethodViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Body = entity.Body,
            IsAsync = entity.IsAsync ?? false,
            ReturnType = entity.ReturnType != null ? new TypePath(entity.ReturnType) : null,
        };

        if (!string.IsNullOrEmpty(entity.Arguments))
        {
            var arguments = JsonConvert.DeserializeObject<List<MethodArgument>>(entity.Arguments);
            if (arguments != null)
            {
                foreach (var argument in arguments)
                {
                    _ = viewModel.Arguments.Add(argument);
                }
            }
        }

        if (!string.IsNullOrEmpty(entity.HttpMethods))
        {
            _ = entity.HttpMethods.Split(',');
            //foreach (var method in httpMethods)
            //{
            //    viewModel.HttpMethods.Add(getHttpMethodByName);
            //}
        }

        return viewModel;
    }

    private static HttpMethodAttribute getHttpMethodByName(string name, string? route) =>
        name?.ToUpper() switch
        {
            "GET" => route is { } ? new HttpGetAttribute(route) : new HttpGetAttribute(),
            "POST" => route is { } ? new HttpPostAttribute(route) : new HttpPostAttribute(),
            "PUT" => route is { } ? new HttpPutAttribute(route) : new HttpPutAttribute(),
            "DELETE" => route is { } ? new HttpDeleteAttribute(route) : new HttpDeleteAttribute(),
            _ => throw new NotImplementedException(),
        };

    private CqrsSegregate? CqrsViewModelToDbEntityInner(CqrsViewModelBase? model, CqrsSegregateType segregateType)
    {
        if (model == null)
        {
            return null;
        }

        var result = this._mapper.Map<CqrsSegregate>(model)
            .ForMember(x => x.SegregateType = segregateType.Cast().ToInt())
            .ForMember(x => x.CategoryId = model.Category.Cast().ToInt())
            .ForMember(x => x.Module = null!);
        if (model.Module?.Id is { } moduleId)
        {
            result.ModuleId = moduleId;
        }

        if (model.ParamsDto?.Id is { } paramDtoId)
        {
            result.ParamDtoId = paramDtoId;
        }

        if (model.ResultDto?.Id is { } resultDtoId)
        {
            result.ResultDtoId = resultDtoId;
        }

        return result;
    }

    [return: NotNullIfNotNull(nameof(entity))]
    private CqrsCommandViewModel? ToCommandViewModel(CqrsSegregate? entity)
        => this.ToViewModel(entity) as CqrsCommandViewModel;

    [return: NotNullIfNotNull(nameof(entity))]
    private CqrsQueryViewModel? ToQueryViewModel(CqrsSegregate? entity)
        => this.ToViewModel(entity) as CqrsQueryViewModel;
}