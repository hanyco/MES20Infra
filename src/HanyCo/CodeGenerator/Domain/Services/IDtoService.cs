using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

/// <summary>
/// The DTO service.
/// </summary>
public interface IDtoService : IBusinessService, IAsyncCrud<DtoViewModel>, IAsyncSaveChanges, IResetChanges, IAsyncCreator<DtoViewModel>
{
    Task<bool> AnyByNameAsync(string name);

    /// <summary>
    /// Gets a new DtoViewModel.
    /// </summary>
    /// <returns>A DtoViewModel.</returns>
    new Task<DtoViewModel> CreateAsync(CancellationToken token = default)
        => Task.FromResult(new DtoViewModel() { Module = new() { Id = 0 } });

    /// <summary>
    /// Creates a new DTO by db table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="columns">The columns.</param>
    /// <returns>A DtoViewModel.</returns>
    [return: NotNull]
    DtoViewModel CreateByDbTable(in DbTableViewModel table, in IEnumerable<DbColumnViewModel> columns);

    Task<Result> DeleteByIdAsync(long dtoId, bool persist = true, CancellationToken token = default);

    /// <summary>
    /// Gets all DTOs by category.
    /// </summary>
    /// <param name="paramsDtos">if set to <c>true</c> [parameters dtos].</param>
    /// <param name="resultDtos">if set to <c>true</c> [result dtos].</param>
    /// <param name="viewModels">if set to <c>true</c> [view models].</param>
    /// <returns></returns>
    public Task<IReadOnlySet<DtoViewModel>> GetAllByCategoryAsync(bool? paramsDtos, bool? resultDtos, bool? viewModels, CancellationToken token = default);

    /// <summary>
    /// Gets the DTOs by module id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>A Task.</returns>
    Task<IReadOnlyList<DtoViewModel>> GetByModuleId(long id, CancellationToken token = default);

    /// <summary>
    /// Gets the properties by dto id async.
    /// </summary>
    /// <param name="dtoId">The dto id.</param>
    /// <returns>A Task.</returns>
    Task<IReadOnlyList<PropertyViewModel>> GetPropertiesByDtoIdAsync(long dtoId, CancellationToken token = default);
}