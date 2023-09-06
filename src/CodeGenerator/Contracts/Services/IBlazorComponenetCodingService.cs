using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Interfaces;
using Library.Results;

namespace Contracts.Services;

public interface IBlazorComponentCodingService : IBusinessService
{
    bool ControlTypeHasPropertiesPage(ControlType controlType);

    UiComponentPropertyViewModel CreateBoundPropertyByDto(DtoViewModel viewModel);

    /// <summary>
    /// Creates a new component asynchronously.
    /// </summary>
    /// <param name="dto">The dto.</param>
    /// <returns></returns>
    Task<UiComponentViewModel> CreateNewComponentAsync(CancellationToken token = default);

    /// <summary>
    /// Creates a new component by dto asynchronously.
    /// </summary>
    /// <param name="dto">The dto.</param>
    /// <returns></returns>
    Task<UiComponentViewModel> CreateNewComponentByDtoAsync(DtoViewModel dto, CancellationToken token = default);
    UiComponentViewModel CreateNewComponentByDto(DtoViewModel dto);

    /// <summary>
    /// Creates a unbound action.
    /// </summary>
    /// <returns></returns>
    UiComponentActionViewModel CreateUnboundAction();

    /// <summary>
    /// Creates a unbound property.
    /// </summary>
    /// <returns></returns>
    UiComponentPropertyViewModel CreateUnboundProperty();

    Task<UiComponentPropertyViewModel?> FillUiComponentPropertyViewModelAsync(UiComponentPropertyViewModel? prop, CancellationToken token = default);

    ///// <summary>
    ///// Generates the blazor code behind.
    ///// </summary>
    ///// <param name="model">The model.</param>
    ///// <returns></returns>
    //GenerateCodeResult GenerateBlazorCodeBehinds(in UiComponentViewModel model, GenerateCodesParameters? arguments = null);

    ///// <summary>
    ///// Generates the blazor HTML code.
    ///// </summary>
    ///// <param name="model">The model.</param>
    ///// <returns></returns>
    //Code? GenerateBlazorHtmlCode(in UiComponentViewModel model);

    /// <summary>
    /// Generates the code behind and html codes.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    Result<Codes> GenerateCodes(in UiComponentViewModel model, GenerateCodesParameters? arguments = null);
    
    bool HasPropertiesPage(ControlType? ct);

    /// <summary>
    /// Saves the specific component to path asynchronously.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="path">The path.</param>
    /// <returns></returns>
    Task SaveToPathAsync(UiComponentViewModel viewModel, string path, GenerateCodesParameters? arguments = null, CancellationToken token = default);

    /// <summary>
    /// Saves the specific page to path asynchronously.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="path">The path.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns></returns>
    Task SaveToPathAsync(UiPageViewModel viewModel, string path, GenerateCodesParameters? arguments = null, CancellationToken token = default);
}

public interface IBlazorPageCodingService : IBusinessService, ICodeGenerator<UiPageViewModel>
{
}