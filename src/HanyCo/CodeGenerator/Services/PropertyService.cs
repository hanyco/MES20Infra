using HanyCo.Infra.CodeGen.Domain;
using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Data.EntityFrameworkCore;
using Library.Exceptions.Validations;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services.CodeGen;

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

    public async Task<Result<int>> DeleteAsync(PropertyViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(model?.Id);

        _ = this._writeDbContext.RemoveById<Property>(model.Id.Value);
        var dbResult = await this.SubmitChanges(persist: persist, token: cancellationToken);

        return dbResult;
    }

    public async Task<bool> DeleteByParentIdAsync(long parentId, bool persist = true, CancellationToken cancellationToken = default)
    {
        var dbProperties = await queryProperties(parentId, cancellationToken);
        await removeProperties(dbProperties, cancellationToken);
        return await this.SubmitChanges(persist: persist, token: cancellationToken) > 0;

        async Task<IEnumerable<Property>> queryProperties(long parentId, CancellationToken cancellationToken = default)
        {
            var query = from x in this._writeDbContext.Properties
                        where x.ParentEntityId == parentId
                        select new Property() { Id = x.Id, Guid = x.Guid };
            var dbResult = await query.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
            return dbResult;
        }

        async Task removeProperties(IEnumerable<Property> dbProperties, CancellationToken cancellationToken = default)
        {
            foreach (var prop in dbProperties)
            {
                _ = await this.DeleteAsync(new(prop.Name, PropertyTypeHelper.FromDbType(prop.TypeFullName)) { Id = prop.Id, Guid = prop.Guid }, cancellationToken: cancellationToken);
            }
        }
    }

    public async Task<IReadOnlyList<PropertyViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = from x in this._readDbContext.Properties
                    select x;
        var dbResult = await query.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
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
        var dbResult = await query.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
        return dbResult;
    }

    public async Task<Result<PropertyViewModel>> Insert(PropertyViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        return await this.SaveChangesAsync(model, persist, property => this._writeDbContext.Properties.Add(property), cancellationToken: cancellationToken);
    }

    public async Task<Result> InsertProperties(IEnumerable<PropertyViewModel> properties, long parentEntityId, bool persist, CancellationToken token = default)
    {
        foreach (var property in properties)
        {
            if (token.IsCancellationRequested)
            {
                return Result.Fail<OperationCanceledException>();
            }

            property.ParentEntityId = parentEntityId;
            var insertResult = await this.Insert(property, false, token);
            if (!insertResult.IsSucceed)
            {
                return insertResult;
            }
        }

        return Result.Succeed;
    }

    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    public async Task<Result<PropertyViewModel>> Update(long id, PropertyViewModel model, bool persist = true, CancellationToken cancellationToken = default)
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

    public async Task<Result<PropertyViewModel?>> ValidateAsync(PropertyViewModel? item, CancellationToken cancellationToken = default)
    {
        Check.MustBeNotNull(item);

        var errors = new List<Exception>();
        if (item.Name.IsNullOrEmpty())
        {
            errors.Add(new NullValueValidationException(nameof(item.Name)));
        }
        if (item.ParentEntityId == 0)
        {
            errors.Add(new NullValueValidationException(nameof(item.ParentEntityId)));
        }
        var result = new Result<PropertyViewModel?>(item, errors: errors);
        return await Task.FromResult(result);
    }

    private async Task<Result<PropertyViewModel>> SaveChangesAsync(PropertyViewModel model, bool persist, Action<Property> manipulate, CancellationToken cancellationToken = default)
    {
        _ = await this.ValidateAsync(model, cancellationToken).ThrowOnFailAsync(cancellationToken: cancellationToken);
        var property = this._converter.ToDbEntity(model)!;
        manipulate(property);
        if (persist)
        {
            _ = await this._writeDbContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        return Result.Success(model);
    }
}