using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.Helpers;

using Library.BusinessServices;
using Library.Exceptions.Validations;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal sealed class PropertyService : IPropertyService
    , IAsyncValidator<PropertyViewModel>
    , IAsyncSaveChanges
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

    public async Task<Result> DeleteAsync(PropertyViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(model?.Id);

        _ = this._writeDbContext.RemoveById<Property>(model.Id.Value);
        if (model.Guid is not null)
        {
            await this._securityService.UnassignEntity(model.Guid.Value, persist: false, token: cancellationToken);
        }

        _ = await this.SubmitChangesAsync(persist: persist);

        return Result.Success;
    }

    public async Task<bool> DeleteByParentIdAsync(long parentId, bool persist = true, CancellationToken cancellationToken = default)
    {
        var dbProperties = await queryProperties(parentId, cancellationToken);
        await removeProperties(dbProperties, cancellationToken);
        return await this.SubmitChangesAsync(persist: persist) > 0;

        async Task<IEnumerable<Property>> queryProperties(long parentId, CancellationToken cancellationToken = default)
        {
            var query = from x in this._writeDbContext.Properties
                        where x.ParentEntityId == parentId
                        select new Property() { Id = x.Id, Guid = x.Guid };
            var dbResult = await query.ToListAsync(cancellationToken: cancellationToken);
            return dbResult;
        }

        async Task removeProperties(IEnumerable<Property> dbProperties, CancellationToken cancellationToken = default)
        {
            foreach (var prop in dbProperties)
            {
                _ = await this.DeleteAsync(new() { Id = prop.Id, Guid = prop.Guid }, cancellationToken: cancellationToken);
            }
        }
    }

    public async Task<IReadOnlyList<PropertyViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = from x in this._readDbContext.Properties
                    select x;
        var dbResult = await query.ToListAsync(cancellationToken: cancellationToken);
        var result = this._converter.ToViewModel(dbResult).Compact().ToReadOnlyList();
        return result;
    }

    public async Task<IReadOnlyList<PropertyViewModel>> GetByDtoIdAsync(long dtoId, CancellationToken cancellationToken = default)
    {
        var dtoIdQuery = from x in this._readDbContext.Properties
                         where x.Id == dtoId
                         select x.DtoId;
        var parentId = await dtoIdQuery.FirstAsync(cancellationToken: cancellationToken);
        return await this.GetByParentIdAsync(parentId.Value, cancellationToken);
    }

    public async Task<PropertyViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var query = from x in this._readDbContext.Properties.Include(x => x.Dto)
                    where x.Id == id
                    select x;
        var dbResult = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        var sec = dbResult is null ? null : await this._securityService.GetByEntityIdAsync(dbResult.Guid);
        var result = this._converter.ToViewModel<PropertyViewModel, Property>(dbResult, sec);
        return result;
    }

    public async Task<IReadOnlyList<PropertyViewModel>> GetByParentIdAsync(long parentId, CancellationToken cancellationToken = default)
    {
        var query = from x in this._readDbContext.Properties.Include(x => x.Dto)
                    where x.ParentEntityId == parentId
                    select x;
        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var viewModels = await this._converter.ToViewModelAsync<PropertyViewModel, Property>(dbResult, this._securityService).ToListCompactAsync(cancellationToken: cancellationToken);
        return viewModels;
    }

    public async Task<IReadOnlyList<Property>> GetDbPropertiesByParentIdAsync(long parentId, CancellationToken cancellationToken = default)
    {
        var query = from d in this._readDbContext.Properties
                    where d.ParentEntityId == parentId
                    select d;
        var dbResult = await query.ToListAsync(cancellationToken: cancellationToken);
        return dbResult;
    }

    public async Task<Result<PropertyViewModel>> InsertAsync(PropertyViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        return await this.SaveChangesAsync(model, persist, manipulate, cancellationToken: cancellationToken);
        void manipulate(Property property) => this._writeDbContext.Properties.Add(property);
    }

    public async Task InsertProperties(IEnumerable<PropertyViewModel> properties, long parentEntityId, bool persist, CancellationToken token = default)
    {
        foreach (var property in properties)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            property.ParentEntityId = parentEntityId;
            _ = await this.InsertAsync(property, false, token);
            if (property.SecurityDescriptors?.Any() is true)
            {
                await this._securityService.SetSecurityDescriptorsAsync(property, persist, token);
            }
        }
    }
    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await this._writeDbContext.SaveChangesResultAsync();

    public async Task<Result<PropertyViewModel>> UpdateAsync(long id, PropertyViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        return await this.SaveChangesAsync(model, persist, manipulate, cancellationToken: cancellationToken);
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

    public async Task<Result<PropertyViewModel>> ValidateAsync(PropertyViewModel item, CancellationToken cancellationToken = default)
    {
        Check.MutBeNotNull(item);

        var errors = new List<(object Id, object Error)>();
        if (item.Name.IsNullOrEmpty())
        {
            errors.Add((NullValueValidationException.ErrorCode, $"Property {nameof(item.Name)} cannot be empty."));
        }
        if (item.ParentEntityId == 0)
        {
            errors.Add((ValidationExceptionBase.ErrorCode, "Parent entity Id cannot be null or 0"));
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

    private async Task<Result<PropertyViewModel>> SaveChangesAsync(PropertyViewModel model, bool persist, Action<Property> manipulate, CancellationToken cancellationToken = default)
    {
        _ = await this.CheckValidatorAsync(model);
        var property = this._converter.ToDbEntity(model)!;
        manipulate(property);
        if (persist)
        {
            _ = await this._writeDbContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        return Result<PropertyViewModel>.CreateSuccess(model);
    }
}