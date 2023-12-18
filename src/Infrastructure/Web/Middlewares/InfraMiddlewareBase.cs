using Microsoft.AspNetCore.Http;

namespace HanyCo.Infra.Web.Middlewares;

public abstract class InfraMiddlewareBase : MesMiddlewareBase
{
    protected InfraMiddlewareBase(RequestDelegate next)
        : base(next)
    {
    }
}