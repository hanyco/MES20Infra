using Contracts.Services;

using HanyCo.Infra.UI.ViewModels;

using Library.Validations;

namespace HanyCo.Infra.UI.Helpers;

public static class SecurityDescriptorServiceHelper
{
    public static async Task SetSecurityDescriptorsAsync<TEntity>(this ISecurityDescriptorService service, TEntity entity, bool persist = true)
        where TEntity : IHasSecurityDescriptor
    {
        Check.IfArgumentNotNull(service);
        if (entity?.Guid is { } guid && entity.SecurityDescriptors?.Any() is true)
        {
            await service.AssignToEntityIdAsync(guid, entity.SecurityDescriptors.Select(x => x.Id), false).ToEnumerableAsync();
        }
    }

}
