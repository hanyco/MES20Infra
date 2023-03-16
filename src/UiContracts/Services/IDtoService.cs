using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace HanyCo.Infra.UI.Services;

/// <summary>
/// The DTO service.
/// </summary>
public interface IDtoService : IBusinesService, IAsyncCrudService<DtoViewModel>, IAsyncSaveService, IResetChanges, IAsyncCreator<DtoViewModel>
{
    /// <summary>
    /// Gets a new DtoViewModel.
    /// </summary>
    /// <returns>A DtoViewModel.</returns>
    new Task<DtoViewModel> CreateAsync() 
        => Task.FromResult(new DtoViewModel() { Module = new() { Id = 0 } });

    /// <summary>
    /// Creates a new DTO by db table.
    /// </summary>
    /// <param name="table">  The table.</param>
    /// <param name="columns">The columns.</param>
    /// <returns>A DtoViewModel.</returns>
    DtoViewModel CreateByDbTable(in DbTableViewModel table, in IEnumerable<DbColumnViewModel> columns);

    /// <summary>
    /// Gets all DTOs by category.
    /// </summary>
    /// <param name="paramsDtos">if set to <c>true</c> [parameters dtos].</param>
    /// <param name="resultDtos">if set to <c>true</c> [result dtos].</param>
    /// <param name="viewModels">if set to <c>true</c> [view models].</param>
    /// <returns></returns>
    public Task<IReadOnlySet<DtoViewModel>> GetAllByCategoryAsync(bool paramsDtos, bool resultDtos, bool viewModels);

    /// <summary>
    /// Gets the DTOs by module id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>A Task.</returns>
    Task<IReadOnlyList<DtoViewModel>> GetByModuleId(long id);

    /// <summary>
    /// Gets the properties by dto id async.
    /// </summary>
    /// <param name="dtoId">The dto id.</param>
    /// <returns>A Task.</returns>
    Task<IReadOnlyList<PropertyViewModel>> GetPropertiesByDtoIdAsync(long dtoId);
}