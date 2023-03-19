using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.ViewModels;

using Library.Mapping;
using Library.Validations;
using Library.Wpf.Bases;

using Services.Helpers;

namespace HanyCo.Infra.UI.Services.Imp;

internal sealed class EntityViewModelConverter : IEntityViewModelConverter
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public EntityViewModelConverter(IMapper mapper, ILogger logger)
        => (this._mapper, this._logger) = (mapper, logger);

    ILogger ILoggerContainer.Logger => this._logger;

    public DtoViewModel FillByDbEntity(in Dto dto, in IEnumerable<Property>? properties)
    {
        var result = this._mapper.Map<DtoViewModel>(dto);
        if (dto?.Module is not null)
        {
            result.Module = this.ToViewModel(dto.Module)!;
        }
        if (dto?.DbObjectId.IsNullOrEmpty() is false)
        {
            result.DbObject = new(string.Empty, dto.DbObjectId.ToLong());
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

    public IEnumerable<DtoViewModel> FillByDbEntity(IEnumerable<Dto> dtos)
        => dtos.Select(x => this.FillByDbEntity(x, null));

    public DtoViewModel FillViewModel(in DtoViewModel viewModel, in Dto dto, in IEnumerable<Property> properties)
    {
        Check.IfArgumentNotNull(dto);
        Check.IfArgumentNotNull(properties);

        _ = this._mapper.Map(dto, viewModel);
        if (viewModel.Module is null && dto.Module is not null)
        {
            viewModel.Module = this.ToViewModel(dto.Module)!;
        }

        _ = viewModel.Properties!.ClearAndAddRange(properties.Select(this.ToViewModel));
        return viewModel;
    }

    public void FillViewModelByDbEntity(in UiBootstrapPositionViewModel viewModel, in UiBootstrapPosition dbEnitity)
        => this._mapper.Map(dbEnitity, viewModel);

    public UiComponentAction? ToDbEntity(UiComponentActionViewModel? model)
        => model is null ? null : this._mapper.Map<UiComponentAction>(model)
            .ForMember(x => x.TriggerTypeId = model.TriggerType.ToInt())
            .ForMember(x => x.CqrsSegregateId = model.CqrsSegregate?.Id)
            .ForMember(x => x.Position = this.ToDbEntity(model.Position)!);

    public UiComponentProperty? ToDbEntity(UiComponentPropertyViewModel? model)
        => model is null ? null : this._mapper.Map<UiComponentProperty>(model)
            .ForMember(x => x.Position = this.ToDbEntity(model.Position)!)
            .ForMember(x => x.PropertyId = model.Property?.Id)
            .ForMember(x => x.ControlTypeId = model.ControlType?.ToInt() ?? 0);

    public SecurityClaim? ToDbEntity(ClaimViewModel? model)
        => model is null ? null : this._mapper.Map<SecurityClaim>(model)
            .ForMember(x => x.Id = model.Id)
            .ForMember(x => x.SecurityDescriptorId = model.SecuritytDescriptorId ?? Guid.Empty);

    public UiComponent? ToDbEntity(UiComponentViewModel? model)
        => model is null ? null : this._mapper.Map<UiComponent>(model)
            .ForMember(x => x.PageDataContextId = model.PageDataContext?.Id)
            .ForMember(x => x.PageDataContextPropertyId = model.PageDataContextProperty?.Id)
            .Fluent()
            .IfTrue(model.UiProperties.Any() is true, x => x.UiComponentProperties = model.UiProperties.Select(this.ToDbEntity).ToList()!)
            .IfTrue(model.UiActions.Any() is true, x => x.UiComponentActions = model.UiActions.Select(this.ToDbEntity).ToList()!);

    public IEnumerable<UiComponentAction?> ToDbEntity(IEnumerable<UiComponentActionViewModel?> models)
        => models.Select(this.ToDbEntity);

    public IEnumerable<UiComponentProperty?> ToDbEntity(IEnumerable<UiComponentPropertyViewModel?> models)
        => models.Select(this.ToDbEntity);

    public IEnumerable<UiComponent?> ToDbEntity(IEnumerable<UiComponentViewModel?> models)
        => models.Select(this.ToDbEntity);

    public IEnumerable<UiBootstrapPosition?> ToDbEntity(IEnumerable<UiBootstrapPositionViewModel?> models)
        => throw new NotImplementedException();

    public IEnumerable<Dto?> ToDbEntity(IEnumerable<DtoViewModel?> models)
        => models.Select(this.ToDbEntity);

    public IEnumerable<Module?> ToDbEntity(IEnumerable<ModuleViewModel?> models)
        => models.Select(this.ToDbEntity);

    public Module? ToDbEntity(ModuleViewModel? model)
        => model is null ? null : this._mapper.Map<Module>(model);

    public UiBootstrapPosition? ToDbEntity(UiBootstrapPositionViewModel? model)
        => model is null ? null : this._mapper.Map<UiBootstrapPosition>(model);

    public IEnumerable<UiPage?> ToDbEntity(IEnumerable<UiPageViewModel?> models)
        => models.Select(this.ToDbEntity);

    public UiPage? ToDbEntity(UiPageViewModel? model)
        => model is null
        ? null
        : this._mapper.Map<UiPage>(model)
                      .ForMember(x => x.DtoId = model.Dto?.Id)
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

        var result = this._mapper.MapExcept<PropertyViewModel, Property>(viewModel, x => new { x.Id })
                                 .ForMember(x => x.ParentEntityId = viewModel.ParentEntityId)
                                 .ForMember(x => x.DbObjectId = viewModel.DbObject?.ObjectId.ToString(CultureInfo.CurrentCulture) ?? string.Empty)
                                 .ForMember(x => x.PropertyType = viewModel.Type.ToInt())
                                 .ForMember(x => Functional.IfTrue(x.Id < 0, () => x.Id = 0).Fluent().IfTrue(viewModel.Id > 0, () => x.Id = viewModel.Id!.Value))
                                 .ForMember(x => x.DtoId = viewModel.Dto?.Id);
        return result;
    }

    public Property? ToDbEntity(PropertyViewModel? model, long parentId)
        => this.ToDbEntity(model?.ForMember(x => x.ParentEntityId = parentId));

    public Dto? ToDbEntity(DtoViewModel? viewModel)
    {
        if (viewModel is null)
        {
            return null;
        }

        var result = this._mapper.Map<Dto>(viewModel)
                                 .ForMember(x => x.Module = this.ToDbEntity(viewModel.Module))
                                 .ForMember(x => x.ModuleId = viewModel.Module.Id);
        _ = result.Properties!.AddRange(viewModel.Properties.Select(x => this.ToDbEntity(x)));
        return result;
    }

    public IEnumerable<SecurityDescriptor?> ToDbEntity(IEnumerable<SecurityDescriptorViewModel?> models)
        => models.Select(this.ToDbEntity);

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
        _ = model.ClaimSet.ForEachEager(x =>
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

    public IEnumerable<SecurityClaim?> ToDbEntity(IEnumerable<ClaimViewModel?> models)
        => models.Select(this.ToDbEntity);

    public IEnumerable<Property?> ToDbEntity(IEnumerable<PropertyViewModel?> models)
        => models.Select(x => this.ToDbEntity(x));

    public CqrsSegregate? ToDbEntity(CqrsQueryViewModel? model)
        => this.CqrsViewModelToDbEntityInner(model, CqrsSegregateType.Query);

    public IEnumerable<CqrsSegregate?> ToDbEntity(IEnumerable<CqrsQueryViewModel?> models)
        => models.Select(this.ToDbEntity);

    public CqrsSegregate? ToDbEntity(CqrsCommandViewModel? model)
        => this.CqrsViewModelToDbEntityInner(model, CqrsSegregateType.Command);

    public IEnumerable<CqrsSegregate?> ToDbEntity(IEnumerable<CqrsCommandViewModel?> models)
        => models.Select(this.ToDbEntity);

    public IEnumerable<Functionality?> ToDbEntity(IEnumerable<FunctionalityViewModel?> models) => throw new NotImplementedException();

    public Functionality? ToDbEntity(FunctionalityViewModel? model) => throw new NotImplementedException();

    public PropertyViewModel? ToPropertyViewModel(DbColumnViewModel? columnViewModel)
        => columnViewModel == null ? null : new()
        {
            DbObject = columnViewModel,
            Name = columnViewModel.Name,
            Type = PropertyTypeHelper.FromDbType(columnViewModel.DbType),
            IsNullable = columnViewModel.IsNullable,
            Id = columnViewModel.ObjectId * -1
        };

    public DtoViewModel? ToViewModel(in Dto? entity, in IEnumerable<SecurityDescriptorViewModel>? securityDescriptors = null)
    {
        if (entity is null)
        {
            return null;
        }

        var result = InnerToViewModel(entity);

        if (entity.Properties is not null and { Count: > 0})
        {
            _ = result.Properties!.AddRange(entity.Properties.Select(this.ToViewModel));
        }
        if (entity.Module is not null)
        {
            result.Module = this.ToViewModel(entity.Module)!;
        }
        if (securityDescriptors is not null)
        {
            _ = securityDescriptors.ForEachEager(result.SecurityDescriptors.Add);
        }
        return result;
    }

    public IEnumerable<ModuleViewModel?> ToViewModel(IEnumerable<Module?> entities)
        => entities.Select(this.ToViewModel);

    public IEnumerable<UiBootstrapPositionViewModel?> ToViewModel(IEnumerable<UiBootstrapPosition?> entities)
        => throw new NotImplementedException();

    public IEnumerable<UiComponentViewModel?> ToViewModel(IEnumerable<UiComponent?> entities)
        => entities.Select(this.ToViewModel);

    public IEnumerable<UiComponentActionViewModel?> ToViewModel(IEnumerable<UiComponentAction?> entities)
        => entities.Select(this.ToViewModel);

    public IEnumerable<UiComponentPropertyViewModel?> ToViewModel(IEnumerable<UiComponentProperty?> entities)
        => entities.Select(this.ToViewModel);

    public ModuleViewModel? ToViewModel(Module? entity)
        => entity is null ? null : this._mapper.Map<ModuleViewModel>(entity);

    public UiBootstrapPositionViewModel? ToViewModel(UiBootstrapPosition? entity)
        => entity is null ? null : this._mapper.Map<UiBootstrapPositionViewModel>(entity);

    public UiComponentViewModel? ToViewModel(UiComponent? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var result = this._mapper.Map<UiComponentViewModel>(entity)
            .ForMember(x => x.UiProperties!.AddRange(this.ToViewModel(entity.UiComponentProperties)))
            .ForMember(x => x.UiActions!.AddRange(this.ToViewModel(entity.UiComponentActions)))
            .ForMember(x => x.PageDataContext = this.ToViewModel(entity.PageDataContext))
            .ForMember(x => x.PageDataContextProperty = this.ToViewModel(entity.PageDataContextProperty));

        return result;
    }

    public UiComponentActionViewModel? ToViewModel(UiComponentAction? entity)
        => entity is null ? null : this._mapper.Map<UiComponentActionViewModel>(entity)
            .ForMember(x => x.Position = this.ToViewModel(entity.Position) ?? new())
            .ForMember(x => x.CqrsSegregate = this.ToViewModel(entity.CqrsSegregate))
            .ForMember(x => x.TriggerType = EnumHelper.ToEnum<TriggerType>(entity.TriggerTypeId));

    public UiComponentPropertyViewModel? ToViewModel(UiComponentProperty? entity)
        => entity is null ? null : this._mapper.Map<UiComponentPropertyViewModel>(entity)
            .ForMember(x => x.Property = this.ToViewModel(entity.Property))
            .ForMember(x => x.ControlType = ControlTypeHelper.FromControlTypeId(entity.ControlTypeId));

    public IEnumerable<UiPageViewModel?> ToViewModel(IEnumerable<UiPage?> entities)
        => entities.Select(this.ToViewModel);

    public UiPageViewModel? ToViewModel(UiPage? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var components = entity.UiPageComponents.Select(this.ToViewModel).Compact();

        var result = this._mapper.Map<UiPageViewModel>(entity)
            .ForMember(x => x.Components.AddRange(components.OrderBy(x => x.Position)))
            .ForMember(x => x.Dto = this.ToViewModel(entity.Dto))
            .ForMember(x => x.Module = this.ToViewModel(entity.Module));
        return result;
    }

    public IEnumerable<PropertyViewModel?> ToViewModel(IEnumerable<Property?> entities)
        => entities.Select(this.ToViewModel);

    public PropertyViewModel? ToViewModel(Property? entity)
        => entity is null ? null : this._mapper.Map<PropertyViewModel>(entity)
            .ForMember(x => x.TypeFullName = entity.TypeFullName!)
            .ForMember(x => x.Type = PropertyTypeHelper.FromPropertyTypeId(entity.PropertyType))
            .ForMember(x => x.Dto = InnerToViewModel(entity.Dto));
    
    [return: NotNullIfNotNull(nameof(entity))]
    private DtoViewModel? InnerToViewModel(Dto? entity) 
        => entity is null ? null : this._mapper.Map<DtoViewModel>(entity);
    public IEnumerable<SecurityDescriptorViewModel?> ToViewModel(IEnumerable<SecurityDescriptor?> entities)
        => entities.Select(this.ToViewModel);

    public SecurityDescriptorViewModel? ToViewModel(SecurityDescriptor? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var result = this._mapper.Map<SecurityDescriptorViewModel>(entity)
            .ForMember(x => x.Id = entity.Id)
            .ForMember(x => x.IsNoSec = entity.SecurityDescriptorStrategy == SecurityDescriptorStrategy.None)
            .ForMember(x => x.IsClaimBased = entity.SecurityDescriptorStrategy == SecurityDescriptorStrategy.Claim)
            .ForMember(x => x.ClaimSet.AddRange(entity.SecurityClaims.Select(y => this._mapper.Map<ClaimViewModel>(y)
                .ForMember(z => z.SecuritytDescriptorId = entity.Id))));
        return result;
    }

    public CqrsViewModelBase? ToViewModel(CqrsSegregate? entity)
    {
        if (entity is null)
        {
            return null;
        }
        var cqrsType = EnumHelper.ToEnum<CqrsSegregateType>(entity.SegregateType);
        CqrsViewModelBase? result = cqrsType switch
        {
            CqrsSegregateType.Command => this._mapper.Map<CqrsCommandViewModel>(entity)
                .ForMember(x => x.Module = this._mapper.Map<ModuleViewModel>(entity.Module))
                .ForMember(x => x.ParamDto = this.ToViewModel(entity.ParamDto))
                .ForMember(x => x.ResultDto = this.ToViewModel(entity.ResultDto)),
            CqrsSegregateType.Query => this._mapper.Map<CqrsQueryViewModel>(entity)
                .ForMember(x => x.Module = this._mapper.Map<ModuleViewModel>(entity.Module))
                .ForMember(x => x.ParamDto = this.ToViewModel(entity.ParamDto))
                .ForMember(x => x.ResultDto = this.ToViewModel(entity.ResultDto)),
            _ => null,
        };
        return result;
    }

    public IEnumerable<CqrsViewModelBase?> ToViewModel(IEnumerable<CqrsSegregate?> entities)
        => entities.Select(this.ToViewModel);

    public IEnumerable<UiComponentViewModel?> ToViewModel(IEnumerable<UiPageComponent?> entities)
        => entities.Select(this.ToViewModel);

    public UiComponentViewModel? ToViewModel(UiPageComponent? entity)
    {
        if (entity is null)
        {
            return null;
        }
        var result = this._mapper.Map<UiComponentViewModel>(entity)
                                 .ForMember(x =>
                                 {
                                     x.Name = entity.UiComponent.Name;
                                     x.NameSpace = entity.UiComponent.NameSpace;
                                     x.UiPageComponentId = entity.Id;
                                     x.Id = entity.UiComponentId;
                                     x.PageComponentId = entity.Id;
                                 })
                                 .ForMember(x => x.Position.FillWith(this.ToViewModel(entity.Position)!))
                                 .ForMember(x => x.PageDataContext = this.ToViewModel(entity.UiComponent.PageDataContext))
                                 .ForMember(x => x.PageDataContextProperty = this.ToViewModel(entity.UiComponent.PageDataContextProperty));
        return result;
    }

    public IEnumerable<FunctionalityViewModel?> ToViewModel(IEnumerable<Functionality?> entities)
        => entities.Select(ToViewModel);

    public FunctionalityViewModel? ToViewModel(Functionality? entity)
        => entity is null ? null : _mapper.Map<FunctionalityViewModel>(entity);

    private CqrsSegregate? CqrsViewModelToDbEntityInner(CqrsViewModelBase? model, CqrsSegregateType segregateType)
        => model is null
                ? null
                : this._mapper.Map<CqrsSegregate>(model)
                              .ForMember(x => x.ModuleId = model.Module.Id.GetValueOrDefault())
                              .ForMember(x => x.ParamDtoId = model.ParamDto.Id!.Value)
                              .ForMember(x => x.ResultDtoId = model.ResultDto.Id!.Value)
                              .ForMember(x => x.SegregateType = segregateType.ToInt())
                              .ForMember(x => x.CategoryId = model.Category.ToInt());
}