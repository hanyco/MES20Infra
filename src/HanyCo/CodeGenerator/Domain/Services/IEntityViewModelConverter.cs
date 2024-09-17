﻿using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Data.Markers;
using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IEntityViewModelConverter :
    IService,
    ILoggerContainer,

    IDbEntityToViewModelConverter<UiComponentViewModel, UiComponent>,
    IViewModelToDbEntityConverter<UiComponentViewModel, UiComponent>,

    IDbEntityToViewModelConverter<UiPropertyViewModel, UiComponentProperty>,
    IViewModelToDbEntityConverter<UiPropertyViewModel, UiComponentProperty>,

    IDbEntityToViewModelConverter<UiComponentButtonViewModelBase, UiComponentAction>,
    IViewModelToDbEntityConverter<UiComponentButtonViewModelBase, UiComponentAction>,

    IDbEntityToViewModelConverter<ModuleViewModel, Module>,
    IViewModelToDbEntityConverter<ModuleViewModel, Module>,

    IDbEntityToViewModelConverter<PropertyViewModel, Property>,
    IViewModelToDbEntityConverter<PropertyViewModel, Property>,

    IDbEntityToViewModelConverter<UiBootstrapPositionViewModel, UiBootstrapPosition>,
    IViewModelToDbEntityConverter<UiBootstrapPositionViewModel, UiBootstrapPosition>,

    IDbEntityToViewModelConverter<UiPageViewModel, UiPage>,
    IViewModelToDbEntityConverter<UiPageViewModel, UiPage>,

    IDbEntityToViewModelConverter<UiComponentViewModel, UiPageComponent>,

    IDbEntityToViewModelConverter<FunctionalityViewModel, Functionality>,
    IViewModelToDbEntityConverter<FunctionalityViewModel, Functionality>,

    IViewModelToDbEntityConverter<DtoViewModel, Dto>,
    IDbEntityToViewModelConverter<DtoViewModel, Dto>,

    IViewModelToDbEntityConverter<ClaimViewModel, SecurityClaim>,
    IDbEntityToViewModelConverter<ClaimViewModel, SecurityClaim>,

    IViewModelToDbEntityConverter<CqrsQueryViewModel, CqrsSegregate>,
    IViewModelToDbEntityConverter<CqrsCommandViewModel, CqrsSegregate>,
    IDbEntityToViewModelConverter<CqrsViewModelBase, CqrsSegregate>
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