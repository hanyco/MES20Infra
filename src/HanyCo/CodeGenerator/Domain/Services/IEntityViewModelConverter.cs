using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Data.Markers;
using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IEntityViewModelConverter :
    IService,
    ILoggerContainer,

    IDbEntityViewModelConverter<UiComponentViewModel, UiComponent>,
    IDbEntityViewModelConverter<UiPropertyViewModel, UiComponentProperty>,
    IDbEntityViewModelConverter<UiComponentButtonViewModelBase, UiComponentAction>,
    IDbEntityViewModelConverter<ModuleViewModel, Module>,
    IDbEntityViewModelConverter<PropertyViewModel, Property>,
    IDbEntityViewModelConverter<UiBootstrapPositionViewModel, UiBootstrapPosition>,
    IDbEntityViewModelConverter<UiPageViewModel, UiPage>,
    IDbEntityToViewModelConverter<UiComponentViewModel, UiPageComponent>,
    IDbEntityViewModelConverter<FunctionalityViewModel, Functionality>,
    IDbEntityViewModelConverter<DtoViewModel, Dto>,
    IDbEntityViewModelConverter<ClaimViewModel, SecurityClaim>,
    IViewModelToDbEntityConverter<CqrsCommandViewModel, CqrsSegregate>,
    IDbEntityViewModelConverter<CqrsViewModelBase, CqrsSegregate>,
    IDbEntityViewModelConverter<ControllerViewModel, Controller>,
    IDbEntityViewModelConverter<ControllerMethodViewModel, ControllerMethod>
{
    [return: NotNullIfNotNull(nameof(entity))]
    DtoViewModel FillByDbEntity(Dto entity, in IEnumerable<Property>? properties);

    IEnumerable<DtoViewModel> FillByDbEntity(IEnumerable<Dto> entities);

    [return: NotNullIfNotNull(nameof(viewModel))]
    DtoViewModel FillViewModel(in DtoViewModel viewModel, in Dto dto, in IEnumerable<Property> properties);

    [return: NotNullIfNotNull(nameof(viewModel))]
    UiBootstrapPositionViewModel? FillByDbEntity(in UiBootstrapPositionViewModel? viewModel, in UiBootstrapPosition? dbEntity);

    [return: NotNullIfNotNull(nameof(viewModel))]
    PropertyViewModel? ToPropertyViewModel(DbColumnViewModel? viewModel);

    [return: NotNullIfNotNull(nameof(propertyViewModel))]
    UiPropertyViewModel? ToUiComponentProperty(in PropertyViewModel? propertyViewModel);
}