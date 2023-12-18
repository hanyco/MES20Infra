using Library.Web.Bases;
using Library.Web.Middlewares.Markers;

using Microsoft.AspNetCore.Http;

namespace HanyCo.Infra.Web.Middlewares;

public abstract class MesMiddlewareBase : MiddlewareBase, IInfraMiddleware
{
    protected MesMiddlewareBase(RequestDelegate next) : base(next)
    {
    }
}