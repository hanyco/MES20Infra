using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.ViewModels;

using Library.Mapping;
using Library.Validations;

using Services.Helpers;

namespace Services;

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

    public UiComponentAction? ToDbEntity(UiComponentActionViewModel? model) =>
        model is null ? null : this._mapper.Map<UiComponentAction>(model)
            .ForMember(x => x.TriggerTypeId = model.TriggerType.Cast().ToInt())
            .ForMember(x => x.CqrsSegregateId = model.CqrsSegregate?.Id)
            .ForMember(x => x.Position = this.ToDbEntity(model.Position)!);

    public UiComponentProperty? ToDbEntity(UiComponentPropertyViewModel? model) =>
        model is null ? null : this._mapper.Map<UiComponentProperty>(model)
            .ForMember(x => x.Position = this.ToDbEntity(model.Position)!)
            .ForMember(x => x.PropertyId = model.Property?.Id)
            .ForMember(x => x.ControlTypeId = model.ControlType?.Cast().ToInt() ?? 0);

    public SecurityClaim? ToDbEntity(ClaimViewModel? model) =>
        model is null ? null : this._mapper.Map<SecurityClaim>(model)
            .ForMember(x => x.Id = model.Id)
            .ForMember(x => x.SecurityDescriptorId = model.SecuritytDescriptorId ?? Guid.Empty);

    public UiComponent? ToDbEntity(UiComponentViewModel? model) =>
        model is null ? null : this._mapper.Map<UiComponent>(model)
            .ForMember(x => x.PageDataContextId = model.PageDataContext?.Id)
            .ForMember(x => x.PageDataContextPropertyId = model.PageDataContextProperty?.Id)
            .ForMember(x => x.IsGrid = model.IsGrid)
            .Fluent()
            .IfTrue(model.UiProperties.Any() is true, x => x.UiComponentProperties = model.UiProperties.Select(this.ToDbEntity).ToList()!)
            .IfTrue(model.UiActions.Any() is true, x => x.UiComponentActions = model.UiActions.Select(this.ToDbEntity).ToList()!);

    public Module? ToDbEntity(ModuleViewModel? model) =>
        model is null ? null : this._mapper.Map<Module>(model);

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

    public Dto? ToDbEntity(DtoViewModel? viewModel)
    {
        if (viewModel is null)
        {
            return null;
        }

        var result = this._mapper.Map<Dto>(viewModel)
            .ForMember(x => x.Module = this.ToDbEntity(viewModel.Module))
            .ForMember(x => x.ModuleId = viewModel.Module.Id);
        _ = result.Properties!.AddRange(viewModel.Properties.Select(this.ToDbEntity));
        return result;
    }

    public SecurityDescriptor? ToDbEntity(SecurityDescriptorViewModel? model)
    {
        if (model is null)
        {
            return null;
        }

        var result = this._mapper.Map<SecurityDescriptor>(model)
            .ForMember(x => x.SecurityDescriptorStrategy = model switch
            {
                { IsNoSec: true } => SecurityDescriptorStrategy.None,
                { IsClaimBased: true } => SecurityDescriptorStrategy.Claim,
                _ => SecurityDescriptorStrategy.None
            })
            .ForMember(x => x.Id = model.Id)!;
        _ = model.ClaimSet.ForEach(x =>
        {
            if (this._mapper.Map<SecurityClaim>(x) is { } claim)
            {
                result.SecurityClaims.Add(claim);
            }
        });
        if (!model.EntityId.IsNullOrEmpty() && !result.EntitySecurities.Any(x => x.EntityId == model.EntityId))
        {
            result.EntitySecurities.Add(new() { EntityId = model.EntityId, IsEnabled = true, SecurityDescriptorId = model.Id });
        }

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

        var result = this._mapper.Map<Functionality>(model);
        //! Not required for saving
        //x result.GetAllQuery = this.ToDbEntity(model.GetAllQueryViewModel.NotNull());
        //x result.GetByIdQuery = this.ToDbEntity(model.GetByIdQueryViewModel.NotNull());
        //x result.InsertCommand = this.ToDbEntity(model.InsertCommandViewModel.NotNull());
        //x result.UpdateCommand = this.ToDbEntity(model.UpdateCommandViewModel.NotNull());
        //x result.DeleteCommand = this.ToDbEntity(model.DeleteCommandViewModel.NotNull());

        result.GetAllQueryId = model.GetAllQueryViewModel?.Id ?? 0;
        result.GetByIdQueryId = model.GetByIdQueryViewModel?.Id ?? 0;
        result.InsertCommandId = model.InsertCommandViewModel?.Id ?? 0;
        result.UpdateCommandId = model.UpdateCommandViewModel?.Id ?? 0;
        result.DeleteCommandId = model.DeleteCommandViewModel?.Id ?? 0;
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

    public UiComponentPropertyViewModel? ToUiComponentProperty(in PropertyViewModel? propertyViewModel) =>
        propertyViewModel == null ? null : new()
        {
            Name = propertyViewModel.Name.NotNull(),
            Property = propertyViewModel,
            ControlType = propertyViewModel.Type.ToControlType(propertyViewModel.IsList, propertyViewModel.IsNullable, propertyViewModel.Dto).Control,
            Caption = string.Equals(propertyViewModel.Name, "id")
                ? propertyViewModel.Name
                : propertyViewModel.Name.SeparateCamelCase(),
            IsEnabled = true
        };

    public DtoViewModel? ToViewModel(in Dto? entity, in IEnumerable<SecurityDescriptorViewModel>? securityDescriptors = null)
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
        if (securityDescriptors is not null)
        {
            _ = securityDescriptors.ForEach(result.SecurityDescriptors.Add);
        }
        return result;
    }

    public ModuleViewModel? ToViewModel(Module? entity) =>
        entity is null ? null : this._mapper.Map<ModuleViewModel>(entity);

    public UiBootstrapPositionViewModel? ToViewModel(UiBootstrapPosition? entity) =>
        entity is null ? null : this._mapper.Map<UiBootstrapPositionViewModel>(entity);

    public UiComponentViewModel? ToViewModel(UiComponent? entity) =>
        entity is null ? null : this._mapper.Map<UiComponentViewModel>(entity)
            .ForMember(x => x.IsGrid = entity.IsGrid ?? false)
            .ForMember(x => x.UiProperties!.AddRange(entity.UiComponentProperties.Select(this.ToViewModel)))
            .ForMember(x => x.UiActions!.AddRange(entity.UiComponentActions.Select(this.ToViewModel)))
            .ForMember(x => x.PageDataContext = this.ToViewModel(entity.PageDataContext))
            .ForMember(x => x.PageDataContextProperty = this.ToViewModel(entity.PageDataContextProperty));

    public UiComponentActionViewModel? ToViewModel(UiComponentAction? entity) =>
        entity is null ? null : this._mapper.Map<UiComponentActionViewModel>(entity)
            .ForMember(x => x.Position = this.ToViewModel(entity.Position) ?? new())
            .ForMember(x => x.CqrsSegregate = this.ToViewModel(entity.CqrsSegregate))
            .ForMember(x => x.TriggerType = EnumHelper.ToEnum<TriggerType>(entity.TriggerTypeId));

    public UiComponentPropertyViewModel? ToViewModel(UiComponentProperty? entity) =>
        entity is null ? null : this._mapper.Map<UiComponentPropertyViewModel>(entity)
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

    public SecurityDescriptorViewModel? ToViewModel(SecurityDescriptor? entity) =>
        entity is null ? null : this._mapper.Map<SecurityDescriptorViewModel>(entity)
            .ForMember(x => x.Id = entity.Id)
            .ForMember(x => x.IsNoSec = entity.SecurityDescriptorStrategy == SecurityDescriptorStrategy.None)
            .ForMember(x => x.IsClaimBased = entity.SecurityDescriptorStrategy == SecurityDescriptorStrategy.Claim)
            .ForMember(x => x.ClaimSet.AddRange(entity.SecurityClaims.Select(y => this._mapper.Map<ClaimViewModel>(y)
            .ForMember(z => z.SecuritytDescriptorId = entity.Id))));

    public CqrsViewModelBase? ToViewModel(CqrsSegregate? entity) =>
        entity is null ? null : EnumHelper.ToEnum<CqrsSegregateType>(entity.SegregateType) switch
        {
            CqrsSegregateType.Command => this._mapper.Map<CqrsCommandViewModel>(entity)
                .ForMember(x => x.Module = this._mapper.Map<ModuleViewModel>(entity.Module))
                .ForMember(x => x.ParamsDto = this.ToViewModel(entity.ParamDto))
                .ForMember(x => x.ResultDto = this.ToViewModel(entity.ResultDto)),
            CqrsSegregateType.Query => this._mapper.Map<CqrsQueryViewModel>(entity)
                .ForMember(x => x.Module = this._mapper.Map<ModuleViewModel>(entity.Module))
                .ForMember(x => x.ParamsDto = this.ToViewModel(entity.ParamDto))
                .ForMember(x => x.ResultDto = this.ToViewModel(entity.ResultDto)),
            _ => null,
        };

    public UiComponentViewModel? ToViewModel(UiPageComponent? entity) =>
        entity is null ? null : this._mapper.Map<UiComponentViewModel>(entity)
            .ForMember(x => x.Id = entity.UiComponentId)
            .ForMember(x => x.Name = entity.UiComponent.Name)
            .ForMember(x => x.NameSpace = entity.UiComponent.NameSpace)
            .ForMember(x => x.UiPageComponentId = entity.Id)
            .ForMember(x => x.PageComponentId = entity.Id)
            .ForMember(x => x.Position.FillWith(this.ToViewModel(entity.Position)!))
            .ForMember(x => x.PageDataContext = this.ToViewModel(entity.UiComponent.PageDataContext))
            .ForMember(x => x.PageDataContextProperty = this.ToViewModel(entity.UiComponent.PageDataContextProperty));

    public FunctionalityViewModel? ToViewModel(Functionality? entity) =>
        entity == null ? null : this._mapper.Map<FunctionalityViewModel>(entity)
            .ForMember(x => x.GetAllQueryViewModel = this.ToViewModel(entity.GetAllQuery).Cast().As<CqrsQueryViewModel>())
            .ForMember(x => x.GetByIdQueryViewModel = this.ToViewModel(entity.GetByIdQuery).Cast().As<CqrsQueryViewModel>())
            .ForMember(x => x.InsertCommandViewModel = this.ToViewModel(entity.InsertCommand).Cast().As<CqrsCommandViewModel>())
            .ForMember(x => x.UpdateCommandViewModel = this.ToViewModel(entity.UpdateCommand).Cast().As<CqrsCommandViewModel>())
            .ForMember(x => x.DeleteCommandViewModel = this.ToViewModel(entity.DeleteCommand).Cast().As<CqrsCommandViewModel>());

    private CqrsSegregate? CqrsViewModelToDbEntityInner(CqrsViewModelBase? model, CqrsSegregateType segregateType) =>
        model == null ? null : this._mapper.Map<CqrsSegregate>(model)
            .ForMember(x => x.ModuleId = model.Module.Id.GetValueOrDefault())
            .ForMember(x => x.ParamDtoId = model.ParamsDto.Id.GetValueOrDefault())
            .ForMember(x => x.ResultDtoId = model.ResultDto.Id.GetValueOrDefault())
            .ForMember(x => x.SegregateType = segregateType.Cast().ToInt())
            .ForMember(x => x.CategoryId = model.Category.Cast().ToInt());

    [return: NotNullIfNotNull(nameof(entity))]
    private DtoViewModel? InnerToViewModel(Dto? entity) =>
        entity is null ? null : this._mapper.Map<DtoViewModel>(entity);
}