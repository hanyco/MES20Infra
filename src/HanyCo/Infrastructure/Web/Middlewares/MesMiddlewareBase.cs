using Library.Web.Bases;

using Microsoft.AspNetCore.Http;

namespace HanyCo.Infra.Web.Middlewares;

public abstract class MesMiddlewareBase(RequestDelegate next) : MiddlewareBase(next)
{
}