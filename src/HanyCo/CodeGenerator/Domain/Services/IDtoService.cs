using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IDtoService : IBusinessService, IAsyncCrud<DtoViewModel>, IAsyncSaveChanges, IResetChanges, IAsyncCreator<DtoViewModel>
{
    Task<bool> AnyByNameAsync(string name);

    new Task<DtoViewModel> CreateAsync(CancellationToken token = default) =>
        Task.FromResult(new DtoViewModel() { Module = new() { Id = 0 } });

    [return: NotNull]
    DtoViewModel CreateByDbTable(in DbTableViewModel table, in IEnumerable<DbColumnViewModel> columns);

    Task<Result> DeleteById(long dtoId, bool persist = true, CancellationToken token = default);

    public Task<IReadOnlySet<DtoViewModel>> GetAllByCategoryAsync(bool? paramsDtos, bool? resultDtos, bool? viewModels, CancellationToken token = default);

    Task<IReadOnlyList<DtoViewModel>> GetByModuleId(long id, CancellationToken token = default);

    Task<IReadOnlyList<PropertyViewModel>> GetPropertiesByDtoIdAsync(long dtoId, CancellationToken token = default);
}