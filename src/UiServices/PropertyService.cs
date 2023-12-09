using Contracts.Services;
using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

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
    private readonly ISecurityService _securityService;
    private readonly InfraWriteDbContext _writeDbContext;

    public PropertyService(InfraReadDbContext readDbContext, InfraWriteDbContext writeDbContext, IEntityViewModelConverter converter, ISecurityService securityService)
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
        _ = await this.SubmitChangesAsync(persist: persist, token: cancellationToken);

        return Result.Success;
    }

    public async Task<bool> DeleteByParentIdAsync(long parentId, bool persist = true, CancellationToken cancellationToken = default)
    {
        var dbProperties = await queryProperties(parentId, cancellationToken);
        await removeProperties(dbProperties, cancellationToken);
        return await this.SubmitChangesAsync(persist: persist, token: cancellationToken) > 0;

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
        var result = this._converter.ToViewModel(dbResult);
        return result;
    }

    public async Task<IReadOnlyList<PropertyViewModel>> GetByParentIdAsync(long parentId, CancellationToken cancellationToken = default)
    {
        var query = from x in this._readDbContext.Properties.Include(x => x.Dto)
                    where x.ParentEntityId == parentId
                    select x;
        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var viewModels = this._converter.ToViewModel(dbResult).Compact().ToReadOnlyList();
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

    public async Task<Result> InsertProperties(IEnumerable<PropertyViewModel> properties, long parentEntityId, bool persist, CancellationToken token = default)
    {
        foreach (var property in properties)
        {
            if (token.IsCancellationRequested)
            {
                return Result.CreateFailure(new OperationCanceledException());
            }

            property.ParentEntityId = parentEntityId;
            var insertResult = await this.InsertAsync(property, false, token);
            if (!insertResult)
            {
                return insertResult;
            }
        }
        return Result.Success;
    }

    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

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
        var result = new Result<PropertyViewModel>(item, errors: errors);
        return await Task.FromResult(result);
    }

    private async Task<Result<PropertyViewModel>> SaveChangesAsync(PropertyViewModel model, bool persist, Action<Property> manipulate, CancellationToken cancellationToken = default)
    {
        _ = await this.ValidateAsync(model, cancellationToken).ThrowOnFailAsync();
        var property = this._converter.ToDbEntity(model)!;
        manipulate(property);
        if (persist)
        {
            _ = await this._writeDbContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        return Result<PropertyViewModel>.CreateSuccess(model);
    }
}