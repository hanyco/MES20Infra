using Contracts.Services;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.BusinessServices;
using Library.Interfaces;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal sealed class ModuleService : IBusinessService, IModuleService
{
    private readonly IEntityViewModelConverter _converter;
    private readonly InfraReadDbContext _readDbContext;

    public ModuleService(InfraReadDbContext readDbContext, IEntityViewModelConverter converter)
        => (this._readDbContext, this._converter) = (readDbContext, converter);

    public Task<IReadOnlyList<ModuleViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => ServiceHelper.GetAllAsync<ModuleViewModel, Module>(this, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<ModuleViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => ServiceHelper.GetByIdAsync<ModuleViewModel, Module>(this, id, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public IAsyncEnumerable<Module> GetChildEntitiesAsync(Module entity, CancellationToken cancellationToken = default)
        => this.GetChildEntitiesByIdAsync(entity.ArgumentNotNull().Id);

    public IAsyncEnumerable<Module> GetChildEntitiesByIdAsync(long parentId, CancellationToken cancellationToken = default)
    {
        var query = from child in this._readDbContext.Modules
                    where child.ParentId == parentId
                    select child;
        var dbResult = query.AsAsyncEnumerable();
        return dbResult;
    }

    public async Task<Module?> GetParentEntityAsync(long childId, CancellationToken cancellationToken = default)
    {
        var query = from child in this._readDbContext.Modules
                    from parent in this._readDbContext.Modules
                    where child.Id == childId
                    where parent.Id == child.ParentId
                    select parent;
        var dbResult = await query.FirstOrDefaultAsync();
        return dbResult;
    }

    public IAsyncEnumerable<Module> GetRootEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var query = from m in this._readDbContext.Modules
                    where m.ParentId == null
                    select m;
        var dbResult = query.AsAsyncEnumerable();
        return dbResult;
    }
}