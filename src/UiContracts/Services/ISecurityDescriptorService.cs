using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Types;
using Library.Validations;

namespace HanyCo.Infra.UI.Services;

public interface ISecurityDescriptorService : IBusinessService, IAsyncCrudService<SecurityDescriptorViewModel, Id>, IAsyncValidator<SecurityDescriptorViewModel>
{
    //Task DeleteByEntityIdAsync(Guid entityId, bool persist = true);
    IAsyncEnumerable<SecurityDescriptorViewModel> AssignToEntityIdAsync(Guid entityId, IEnumerable<Id>? securityDescriptorIds, bool persist = true);

    Task<bool> ExistsById(Guid id);

    /// <summary>
    /// Gets the by entity identifier asynchronously.
    /// </summary>
    /// <param name="entityId">The entity identifier.</param>
    /// <returns></returns>
    Task<IEnumerable<SecurityDescriptorViewModel>> GetByEntityIdAsync(Guid entityId);

    /// <summary>
    /// Unassigns the entity.
    /// </summary>
    /// <param name="entityId">             The entity identifier.</param>
    /// <param name="securityDescriptorIds">
    /// The security descriptor ids. <strong>💡 NULL means all</strong>
    /// </param>
    /// <param name="persist">              if set to <c>true</c> [persist].</param>
    /// <remarks>NULL for <c>securityDescriptorIds</c> means all Security Descriptors</remarks>
    /// <returns></returns>
    Task UnassignEntity(Guid entityId, IEnumerable<Id>? securityDescriptorIds = null, bool persist = true);

    Task SetSecurityDescriptorsAsync<TEntity>(TEntity entity, bool persist = true)
        where TEntity : IHasSecurityDescriptor;
}