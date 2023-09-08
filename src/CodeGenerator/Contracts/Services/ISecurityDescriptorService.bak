using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Types;
using Library.Validations;

namespace Contracts.Services;

public interface ISecurityDescriptorService : IBusinessService, IAsyncCrud<SecurityDescriptorViewModel, Id>, IAsyncValidator<SecurityDescriptorViewModel>
{
    //Task DeleteByEntityIdAsync(Guid entityId, bool persist = true);
    IAsyncEnumerable<SecurityDescriptorViewModel> AssignToEntityIdAsync(Guid entityId, IEnumerable<Id>? securityDescriptorIds, bool persist = true, CancellationToken token = default);

    Task<bool> ExistsById(Guid id, CancellationToken token = default);

    /// <summary>
    /// Gets the by entity identifier asynchronously.
    /// </summary>
    /// <param name="entityId">The entity identifier.</param>
    /// <returns></returns>
    Task<IEnumerable<SecurityDescriptorViewModel>> GetByEntityIdAsync(Guid entityId, CancellationToken token = default);

    Task SetSecurityDescriptorsAsync<TEntity>(TEntity entity, bool persist = true, CancellationToken token = default)
            where TEntity : IHasSecurityDescriptor;

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
    Task UnassignEntity(Guid entityId, IEnumerable<Id>? securityDescriptorIds = null, bool persist = true, CancellationToken token = default);
}