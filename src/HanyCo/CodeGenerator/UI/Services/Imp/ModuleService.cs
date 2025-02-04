﻿using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.UI.Services.Imp;

internal sealed class ModuleService : IBusinesService,IModuleService
{
    private readonly IEntityViewModelConverter _converter;
    private readonly InfraReadDbContext _readDbContext;

    public ModuleService(InfraReadDbContext readDbContext, IEntityViewModelConverter converter) =>
        (this._readDbContext, this._converter) = (readDbContext, converter);

    public Task<IReadOnlyList<ModuleViewModel>> GetAllAsync()
        => this.GetAllAsync<ModuleViewModel, Module>(this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<ModuleViewModel?> GetByIdAsync(long id)
        => this.GetByIdAsync<ModuleViewModel, Module>(id, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<IEnumerable<Module>> GetChildEntitiesAsync(Module entity)
        => this.GetChildEntitiesByIdAsync(entity.ArgumentNotNull().Id);

    public async Task<IEnumerable<Module>> GetChildEntitiesByIdAsync(long parentId)
    {
        var query = from child in this._readDbContext.Modules
                    where child.ParentId == parentId
                    select child;
        var dbResult = await query.ToListAsync();
        return dbResult;
    }

    public async Task<Module?> GetParentEntityAsync(long childId)
    {
        var query = from child in this._readDbContext.Modules
                    from parent in this._readDbContext.Modules
                    where child.Id == childId
                    where parent.Id == child.ParentId
                    select parent;
        var dbResult = await query.FirstOrDefaultAsync();
        return dbResult;
    }

    public async Task<IEnumerable<Module>> GetRootEntitiesAsync()
    {
        var query = from m in this._readDbContext.Modules
                    where m.ParentId == null
                    select m;
        var dbResult = await query.ToListAsync();
        return dbResult;
    }
}