using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IPropertyService : IBusinessService, IAsyncCrud<PropertyViewModel>
{
    Task<bool> DeleteByParentIdAsync(long parentId, bool persist = true, CancellationToken token = default);

    Task<IReadOnlyList<PropertyViewModel>> GetByDtoIdAsync(long dtoId, CancellationToken token = default);

    Task<IReadOnlyList<PropertyViewModel>> GetByParentIdAsync(long parentId, CancellationToken token = default);

    Task<IReadOnlyList<Property>> GetDbPropertiesByParentIdAsync(long parentId, CancellationToken token = default);

    Task<Result> InsertProperties(IEnumerable<PropertyViewModel> properties, long parentEntityId, bool persist, CancellationToken token = default);
}