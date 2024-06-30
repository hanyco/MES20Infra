﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Mapping;
using Library.Validations;

using Services.Helpers;

namespace Services.CodeGen;

internal sealed class EntityViewModelConverter(IMapper mapper, ILogger logger) : IEntityViewModelConverter
{
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;

    ILogger ILoggerContainer.Logger => this._logger;

    public DtoViewModel FillByDbEntity(Dto dto, in IEnumerable<Property>? properties)
    {
        var result = this._mapper.Map<DtoViewModel>(dto).With(x => x.Module = this.ToViewModel(dto.Module)!);
        if (dto?.DbObjectId.IsNullOrEmpty() is false)
        {
            result.DbObject = new(string.Empty, dto.DbObjectId.Cast().ToLong());
        }
        if (properties?.Any() is true)
        {
            foreach (var property in properties)
            {
                result.Properties.Add(this.ToViewModel(property)!);
            }
        }

        return result;
    }

    public IEnumerable<DtoViewModel> FillByDbEntity(IEnumerable<Dto> dtos) =>
        dtos.Select(x => this.FillByDbEntity(x, null));

    public UiBootstrapPositionViewModel? FillByDbEntity(in UiBootstrapPositionViewModel? viewModel, in UiBootstrapPosition? dbEntity) =>
        viewModel is null ? null : this._mapper.Map(dbEntity, viewModel);

    public DtoViewModel FillViewModel(in DtoViewModel viewModel, in Dto dto, in IEnumerable<Property> properties)
    {
        Check.MustBeArgumentNotNull(viewModel);
        Check.MustBeArgumentNotNull(viewModel.Properties);
        Check.MustBeArgumentNotNull(dto);
        Check.MustBeArgumentNotNull(properties);

        _ = this._mapper.Map(dto, viewModel);
        if (viewModel.Module is null && dto.Module is not null)
        {
            viewModel.Module = this.ToViewModel(dto.Module)!;
        }

        _ = viewModel.Properties!.ClearAndAddRange(properties.Select(this.ToViewModel));
        return viewModel;
    }

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

    public SecurityClaim? ToDbEntity(ClaimViewModel? model) =>
        model is null ? null : this._mapper.Map<SecurityClaim>(model);

    public UiComponent? ToDbEntity(UiComponentViewModel? model) =>
        model is null ? null : this._mapper.Map<UiComponent>(model)
            .ForMember(x => x.PageDataContextId = model.PageDataContext?.Id)
            .ForMember(x => x.PageDataContextPropertyId = model.PageDataContextProperty?.Id)
            .ForMember(x => x.IsGrid = model.IsGrid)
            .Fluent()
            .IfTrue(model.Properties.Any() is true, x => x.UiComponentProperties = model.Properties.Select(this.ToDbEntity).Compact().ToList());

    // TODO: Think about how to load component complexity from database
    //?.IfTrue(model.UiActions.Any() is true, x => x.UiComponentActions = model.UiActions.Select(this.ToDbEntity).ToList()!);

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

    public Property? ToDbEntity(PropertyViewModel? viewModel) =>
        viewModel is null ? null : this._mapper.MapExcept<Property>(viewModel, x => new { x.Id })
            .ForMember(x => x.ParentEntityId = viewModel.ParentEntityId)
            .ForMember(x => x.DbObjectId = viewModel.DbObject?.ObjectId.ToString(CultureInfo.CurrentCulture) ?? string.Empty)
            .ForMember(x => x.PropertyType = viewModel.Type.Cast().ToInt())
            .ForMember(x => (x.Id < 0).IfTrue(() => x.Id = 0).Fluent().IfTrue(viewModel.Id > 0, () => x.Id = viewModel.Id!.Value))
            .ForMember(x => x.DtoId = viewModel.Dto?.Id);

    [return: NotNullIfNotNull(nameof(model))]
    public Dto? ToDbEntity(DtoViewModel? model)
    {
        if (model is null)
        {
            return null;
        }

        var result = this._mapper.Map<Dto>(model)
            .ForMember(x => x.Module = this.ToDbEntity(model.Module));
        if (model.Module?.Id is { } moduleId && result.ModuleId != moduleId)
        {
            result.ModuleId = moduleId;
        }
        _ = (result.Properties?.AddRange(model.Properties.Select(this.ToDbEntity)));
        return result;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public CqrsSegregate? ToDbEntity(CqrsQueryViewModel? model) =>
        this.CqrsViewModelToDbEntityInner(model, CqrsSegregateType.Query);

    [return: NotNullIfNotNull(nameof(model))]
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

        result.GetAllQuery = this.ToDbEntity(model.GetAllQueryViewModel.NotNull());
        result.GetByIdQuery = this.ToDbEntity(model.GetByIdQueryViewModel.NotNull());
        result.InsertCommand = this.ToDbEntity(model.InsertCommandViewModel.NotNull());
        result.UpdateCommand = this.ToDbEntity(model.UpdateCommandViewModel.NotNull());
        result.DeleteCommand = this.ToDbEntity(model.DeleteCommandViewModel.NotNull());
        result.SourceDto = this.ToDbEntity(model.SourceDto);
        result.Module = this.ToDbEntity(model.SourceDto.Module);
        result.ModuleId = result.Module?.Id;
        //TODO Fill IDs
        return result;
    }

    public PropertyViewModel? ToPropertyViewModel(DbColumnViewModel? columnViewModel) =>
        columnViewModel == null
            ? null
            : new()
            {
                DbObject = columnViewModel,
                Name = columnViewModel.Name,
                Type = PropertyTypeHelper.FromDbType(columnViewModel.DbType),
                IsNullable = columnViewModel.IsNullable,
                Id = columnViewModel.ObjectId * -1
            };

    public UiPropertyViewModel? ToUiComponentProperty(in PropertyViewModel? propertyViewModel) =>
        propertyViewModel == null ? null : new()
        {
            Name = propertyViewModel.Name.NotNull(),
            Property = propertyViewModel,
            ControlType = propertyViewModel.Type.ToControlType(propertyViewModel.IsList, propertyViewModel.IsNullable, propertyViewModel.Dto).Control,
            Caption = string.Equals(propertyViewModel.Name, "id", global::System.StringComparison.Ordinal)
                ? propertyViewModel.Name
                : propertyViewModel.Name.SeparateCamelCase(),
            IsEnabled = !propertyViewModel.Name.EqualsTo("id")
        };

    [return: NotNullIfNotNull(nameof(entity))]
    public DtoViewModel? ToViewModel(Dto? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var result = this.InnerToViewModel(entity);

        if (entity.Properties?.Count > 0)
        {
            _ = result.Properties!.AddRange(entity.Properties.Select(this.ToViewModel));
        }
        if (entity.Module is not null)
        {
            result.Module = this.ToViewModel(entity.Module)!;
        }
        return result;
    }

    public ModuleViewModel? ToViewModel(HanyCo.Infra.Internals.Data.DataSources.Module? entity) =>
        entity is null ? null : this._mapper.Map<ModuleViewModel>(entity);

    public UiBootstrapPositionViewModel? ToViewModel(UiBootstrapPosition? entity) =>
        entity is null ? null : this._mapper.Map<UiBootstrapPositionViewModel>(entity);

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
        };

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
            .ForMember(x => x.Module = this.ToViewModel(entity.Module));

    public PropertyViewModel? ToViewModel(Property? entity) =>
        entity is null ? null : this._mapper.Map<PropertyViewModel>(entity)
            .ForMember(x => x.TypeFullName = entity.TypeFullName!)
            .ForMember(x => x.Type = PropertyTypeHelper.FromPropertyTypeId(entity.PropertyType))
            .ForMember(x => x.Dto = this.InnerToViewModel(entity.Dto));

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
            .ForMember(x => x.GetAllQueryViewModel = this.ToQueryViewModel(entity.GetAllQuery))
            .ForMember(x => x.GetByIdQueryViewModel = this.ToQueryViewModel(entity.GetByIdQuery))
            .ForMember(x => x.InsertCommandViewModel = this.ToCommandViewModel(entity.InsertCommand))
            .ForMember(x => x.UpdateCommandViewModel = this.ToCommandViewModel(entity.UpdateCommand))
            .ForMember(x => x.DeleteCommandViewModel = this.ToCommandViewModel(entity.DeleteCommand));

    [return: NotNullIfNotNull(nameof(entity))]
    public ClaimViewModel? ToViewModel(SecurityClaim? entity)
        => entity is null ? null : this._mapper.Map<ClaimViewModel>(entity);

    private CqrsSegregate? CqrsViewModelToDbEntityInner(CqrsViewModelBase? model, CqrsSegregateType segregateType)
    {
        if (model == null)
        {
            return null;
        }

        var result = this._mapper.Map<CqrsSegregate>(model)
            .ForMember(x => x.SegregateType = segregateType.Cast().ToInt())
            .ForMember(x => x.CategoryId = model.Category.Cast().ToInt());
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
    private DtoViewModel? InnerToViewModel(Dto? entity) =>
        entity is null ? null : this._mapper.Map<DtoViewModel>(entity);

    [return: NotNullIfNotNull(nameof(entity))]
    private CqrsCommandViewModel? ToCommandViewModel(CqrsSegregate? entity)
        => this.ToViewModel(entity) as CqrsCommandViewModel;

    [return: NotNullIfNotNull(nameof(entity))]
    private CqrsQueryViewModel? ToQueryViewModel(CqrsSegregate? entity)
        => this.ToViewModel(entity) as CqrsQueryViewModel;
}