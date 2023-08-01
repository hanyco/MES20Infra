using HanyCo.Infra.Security.Helpers;
using HanyCo.Infra.Security.Markers;
using HanyCo.Infra.Security.Model;
using Library.Validations;
using Library.Web.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace HanyCo.Infra.Security;

public static class MvcHelper
{
    private static readonly HashSet<ApiAuthorizationInfo> _cache;

    public static IEnumerable<ApiAuthorizationInfo> Initialize(IActionDescriptorCollectionProvider provider)
    {
        Check.MustBeArgumentNotNull(provider);
        {
            var actionDescriptors = provider.ActionDescriptors.Items;
            foreach (var actionDescriptor in actionDescriptors)
            {
                if (actionDescriptor is not ControllerActionDescriptor descriptor)
                {
                    continue;
                }

                _cache.Add(new(new(descriptor), descriptor.MethodInfo.GetCustomAttribute<MesAuthorizeAttribute>()?.ToAuthorizationInfo()));
            }
        }
        return _cache.AsEnumerable();
    }

    public static IEnumerable<ApiAuthorizationInfo> GetAllActions() =>
        _cache.ToEnumerable();

    public static IEnumerable<ApiAuthorizationInfo> GetAuthorizationRequiredActions() =>
        _cache.Where(x => x.Authorization?.ClaimInfo is not null);

    public static ApiAuthorizationInfo? GetActionInfoByAction(this HttpContext context, ApiInfo api) =>
        _cache.FirstOrDefault(x => x.Api == api);

    public static ApiAuthorizationInfo? GetInfo(this ApiInfo api, HttpContext context) =>
        context.GetActionInfoByAction(api);

    public static ApiInfo GetCurrentAction(this HttpContext context) =>
        (context.GetRouteValue("area")?.ToString(),
        context.GetRouteValue("controler")?.ToString(),
        context.GetRouteValue("action")?.ToString());
}

