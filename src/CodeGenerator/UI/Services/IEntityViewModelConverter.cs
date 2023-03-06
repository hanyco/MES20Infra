using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace HanyCo.Infra.UI.Services;

public interface IEntityViewModelConverter :
    IService,
    ILoggerContainer,

    IDbEntityToViewModelConverter<UiComponentViewModel, UiComponent>,
    IViewModelToDbEntityConverter<UiComponentViewModel, UiComponent>,

    IDbEntityToViewModelConverter<UiComponentPropertyViewModel, UiComponentProperty>,
    IViewModelToDbEntityConverter<UiComponentPropertyViewModel, UiComponentProperty>,

    IDbEntityToViewModelConverter<UiComponentActionViewModel, UiComponentAction>,
    IViewModelToDbEntityConverter<UiComponentActionViewModel, UiComponentAction>,

    IDbEntityToViewModelConverter<ModuleViewModel, Module>,
    IViewModelToDbEntityConverter<ModuleViewModel, Module>,

    IDbEntityToViewModelConverter<PropertyViewModel, Property>,
    IViewModelToDbEntityConverter<PropertyViewModel, Property>,

    IDbEntityToViewModelConverter<UiBootstrapPositionViewModel, UiBootstrapPosition>,
    IViewModelToDbEntityConverter<UiBootstrapPositionViewModel, UiBootstrapPosition>,

    IDbEntityToViewModelConverter<UiPageViewModel, UiPage>,
    IViewModelToDbEntityConverter<UiPageViewModel, UiPage>,

    IDbEntityToViewModelConverter<UiComponentViewModel, UiPageComponent>,

    IDbEntityToViewModelConverter<SecurityDescriptorViewModel, SecurityDescriptor>,
    IViewModelToDbEntityConverter<SecurityDescriptorViewModel, SecurityDescriptor>,

    IDbEntityToViewModelConverter<FunctionalityViewModel, Functionality>,
    IViewModelToDbEntityConverter<FunctionalityViewModel, Functionality>,

    IViewModelToDbEntityConverter<DtoViewModel, Dto>,

    IViewModelToDbEntityConverter<ClaimViewModel, SecurityClaim>,

    IViewModelToDbEntityConverter<CqrsQueryViewModel, CqrsSegregate>,
    IViewModelToDbEntityConverter<CqrsCommandViewModel, CqrsSegregate>,
    IDbEntityToViewModelConverter<CqrsViewModelBase, CqrsSegregate>
{
    DtoViewModel FillByDbEntity(in Dto dto, in IEnumerable<Property>? properties);

    IEnumerable<DtoViewModel> FillByDbEntity(IEnumerable<Dto> dtos);

    DtoViewModel FillViewModel(in DtoViewModel viewModel, in Dto dto, in IEnumerable<Property> properties);

    Property? ToDbEntity(PropertyViewModel? model, long parentId);

    [return: NotNullIfNotNull(nameof(columnViewModel))]
    PropertyViewModel? ToPropertyViewModel(DbColumnViewModel? columnViewModel);

    DtoViewModel? ToViewModel(in Dto? entity, in IEnumerable<SecurityDescriptorViewModel>? securityDescriptors = null);
}