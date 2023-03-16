using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;

namespace HanyCo.Infra.UI.Services;

public interface IPropertyService : IBusinesService, IAsyncCrudService<PropertyViewModel>
{
    Task<bool> DeleteByParentIdAsync(long parentId, bool persist = true);

    Task<IReadOnlyList<PropertyViewModel>> GetByDtoIdAsync(long dtoId);

    Task<IReadOnlyList<PropertyViewModel>> GetByParentIdAsync(long parentId);

    Task<IReadOnlyList<Property>> GetDbPropertiesByParentIdAsync(long parentId);
}