using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Interfaces;

namespace HanyCo.Infra.UI.Services;

public interface IBlazorCodingService : IBusinesService
{
    /// <summary>
    /// Creates a new component asynchronously.
    /// </summary>
    /// <param name="dto">The dto.</param>
    /// <returns></returns>
    Task<UiComponentViewModel> CreateNewComponentAsync();

    /// <summary>
    /// Creates a new component by dto asynchronously.
    /// </summary>
    /// <param name="dto">The dto.</param>
    /// <returns></returns>
    Task<UiComponentViewModel> CreateNewComponentByDtoAsync(DtoViewModel dto);

    /// <summary>
    /// Generates the blazor HTML code.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    Code? GenerateBlazorHtmlCode(in UiComponentViewModel model);

    /// <summary>
    /// Generates the blazor code behind.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    GenerateCodeResult GenerateBlazorCodeBehinds(in UiComponentViewModel model, GenerateCodesParameters? arguments = null);

    /// <summary>
    /// Generates the code behind and html codes.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    Codes GenerateCodes(in UiComponentViewModel model, GenerateCodesParameters? arguments = null);

    Task<UiComponentPropertyViewModel?> FillUiComponentPropertyViewModelAsync(UiComponentPropertyViewModel? prop);

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <param name="propertyViewModel">The property view model.</param>
    /// <returns></returns>
    UiComponentPropertyViewModel GetProperty(in PropertyViewModel propertyViewModel);

    /// <summary>
    /// Creates a unbound property.
    /// </summary>
    /// <returns></returns>
    UiComponentPropertyViewModel CreateUnboundProperty();

    UiComponentPropertyViewModel CreateBoundPropertyByDto(DtoViewModel viewModel);

    /// <summary>
    /// Creates a unbound action.
    /// </summary>
    /// <returns></returns>
    UiComponentActionViewModel CreateUnboundAction();

    /// <summary>
    /// Saves the specific component to path asynchronously.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="path">The path.</param>
    /// <returns></returns>
    Task SaveToPathAsync(UiComponentViewModel viewModel, string path, GenerateCodesParameters? arguments = null);

    /// <summary>
    /// Saves the specific page to path asynchronously.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="path">The path.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns></returns>
    Task SaveToPathAsync(UiPageViewModel viewModel, string path, GenerateCodesParameters? arguments = null);

    bool ControlTypeHasPropertiesPage(ControlType controlType);

    bool HasPropertiesPage(ControlType? ct);
}
