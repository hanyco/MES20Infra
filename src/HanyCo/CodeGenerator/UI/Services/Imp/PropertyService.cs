﻿using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;
using HanyCo.Infra.UI.ViewModels;

using Library.Exceptions.Validations;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.UI.Services.Imp;

internal sealed class PropertyService : IPropertyService
    , IAsyncValidator<PropertyViewModel>
    , IAsyncSaveService
    , IResetChanges
{
    private readonly IEntityViewModelConverter _converter;
    private readonly InfraReadDbContext _readDbContext;
    private readonly ISecurityDescriptorService _securityService;
    private readonly InfraWriteDbContext _writeDbContext;

    public PropertyService(InfraReadDbContext readDbContext, InfraWriteDbContext writeDbContext, IEntityViewModelConverter converter, ISecurityDescriptorService securityService)
    {
        this._readDbContext = readDbContext;
        this._writeDbContext = writeDbContext;
        this._converter = converter;
        this._securityService = securityService;
    }

    public async Task<Result> DeleteAsync(PropertyViewModel model, bool persist = true)
    {
        Check.ArgumentNotNull(model?.Id);

        _ = this._writeDbContext.RemoveById<Property>(model.Id.Value);
        if (model.Guid is not null)
        {
            await this._securityService.UnassignEntity(model.Guid.Value, persist: false);
        }

        _ = await this.SubmitChangesAsync(persist: persist);

        return Result.Success;
    }

    public async Task<bool> DeleteByParentIdAsync(long parentId, bool persist = true)
    {
        var dbProperties = await queryProperies(parentId);
        await removeProperties(dbProperties);
        return await this.SubmitChangesAsync(persist: persist) > 0;

        async Task<IEnumerable<Property>> queryProperies(long parentId)
        {
            var query = from x in this._writeDbContext.Properties
                        where x.ParentEntityId == parentId
                        select new Property() { Id = x.Id, Guid = x.Guid };
            var dbResult = await query.ToListAsync();
            return dbResult;
        }

        async Task removeProperties(IEnumerable<Property> dbProperties)
        {
            foreach (var prop in dbProperties)
            {
                _ = await this.DeleteAsync(new() { Id = prop.Id, Guid = prop.Guid });
            }
        }
    }

    public async Task<IReadOnlyList<PropertyViewModel>> GetAllAsync()
    {
        var query = from x in this._readDbContext.Properties
                    select x;
        var dbResult = await query.ToListAsync();
        var result = this._converter.ToViewModel(dbResult).Compact().ToReadOnlyList();
        return result;
    }

    public async Task<IReadOnlyList<PropertyViewModel>> GetByDtoIdAsync(long dtoId)
    {
        var dtoIdQuery = from x in this._readDbContext.Properties
                         where x.Id == dtoId
                         select x.DtoId;
        var parentId = await dtoIdQuery.FirstAsync();
        return await this.GetByParentIdAsync(parentId.Value);
    }

    public async Task<PropertyViewModel?> GetByIdAsync(long id)
    {
        var query = from x in this._readDbContext.Properties.Include(x => x.Dto)
                    where x.Id == id
                    select x;
        var dbResult = await query.FirstOrDefaultAsync();
        var sec = dbResult is null ? null : await this._securityService.GetByEntityIdAsync(dbResult.Guid);
        var result = this._converter.ToViewModel<PropertyViewModel, Property>(dbResult, sec);
        return result;
    }

    public async Task<IReadOnlyList<PropertyViewModel>> GetByParentIdAsync(long parentId)
    {
        var query = from x in this._readDbContext.Properties.Include(x => x.Dto)
                    where x.ParentEntityId == parentId
                    select x;
        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var vewiModels = await this._converter.ToViewModelAsync<PropertyViewModel, Property>(dbResult, this._securityService).ToListCompactAsync();
        return vewiModels;
    }

    public async Task<IReadOnlyList<Property>> GetDbPropetiesByParentIdAsync(long parentId)
    {
        var query = from d in this._readDbContext.Properties
                    where d.ParentEntityId == parentId
                    select d;
        var dbResult = await query.ToListAsync();
        return dbResult;
    }

    public async Task<Result<PropertyViewModel>> InsertAsync(PropertyViewModel model, bool persist = true)
    {
        return await this.SaveChangesAsync(model, persist, manipulate);
        void manipulate(Property property) => this._writeDbContext.Properties.Add(property);
    }

    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public async Task<Result<int>> SaveChangesAsync()
        => await this._writeDbContext.SaveChangesResultAsync();

    public async Task<Result<PropertyViewModel>> UpdateAsync(long id, PropertyViewModel model, bool persist = true)
    {
        return await this.SaveChangesAsync(model, persist, manipulate);
        void manipulate(Property property) =>
            this._writeDbContext.Properties.Attach(property)
                                           .SetModified(x => x.Name)
                                           .SetModified(x => x.Comment)
                                           .SetModified(x => x.HasGetter)
                                           .SetModified(x => x.HasGetter)
                                           .SetModified(x => x.IsList)
                                           .SetModified(x => x.PropertyType)
                                           .SetModified(x => x.TypeFullName);
    }

    public async Task<Result<PropertyViewModel>> ValidateAsync(PropertyViewModel item)
    {
        Check.NotNull(item);

        var errors = new List<(object Id, object Error)>();
        if (item.Name.IsNullOrEmpty())
        {
            errors.Add((NullValueValidationException.ErrorCode, $"Property {nameof(item.Name)} cannot be empty."));
        }
        if (item.ParentEntityId == 0)
        {
            errors.Add((RequiredValidationException.ErrorCode, "Parent entity Id cannot be null or 0"));
        }
        if (item.SecurityDescriptors?.Any() is true)
        {
            foreach (var secDesc in item.SecurityDescriptors)
            {
                //x_ = result.With(await this._securityService.ValidateAsync(secDesc));
            }
        }
        var result = Result<PropertyViewModel>.New(item, errors: errors);
        return await Task.FromResult(result);
    }

    private async Task<Result<PropertyViewModel>> SaveChangesAsync(PropertyViewModel model, bool persist, Action<Property> manipulate)
    {
        _ = await this.CheckValidatorAsync(model);
        var property = this._converter.ToDbEntity(model)!;
        manipulate(property);
        if (persist)
        {
            _ = await this._writeDbContext.SaveChangesAsync();
        }

        return Result<PropertyViewModel>.CreateSuccess(model);
    }
}