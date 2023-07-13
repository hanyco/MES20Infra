using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;

namespace Contracts.Services;

public interface IPropertyService : IBusinessService, IAsyncCrud<PropertyViewModel>
{
    Task<bool> DeleteByParentIdAsync(long parentId, bool persist = true, CancellationToken token = default);

    Task<IReadOnlyList<PropertyViewModel>> GetByDtoIdAsync(long dtoId, CancellationToken token = default);

    Task<IReadOnlyList<PropertyViewModel>> GetByParentIdAsync(long parentId, CancellationToken token = default);

    Task<IReadOnlyList<Property>> GetDbPropertiesByParentIdAsync(long parentId, CancellationToken token = default);
}