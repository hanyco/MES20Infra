using System.Reflection;



using HanyCo.Infra.Markers;

using Library.Helpers;
using Library.Interfaces;

namespace InfraTestProject.Tests;

[Trait("Category", nameof(ArchitecturalTests))]
public sealed class ArchitecturalTests
{
    [Fact]
    public void ServiceClassesMustImplementIServiceInterface()
    {
        var asm = typeof(ServicesModule).Assembly;
        var serviceClasses = asm.GetTypes().Where(x => ObjectHelper.HasAttribute<ServiceAttribute>(x, true));
        var badGuys = serviceClasses.Where(x => !ObjectHelper.IsInheritedOrImplemented(x, typeof(IService)));
        foreach (var serviceClass in badGuys)
        {
            Assert.Fail($"{serviceClass} must be inherited from `{typeof(IService).FullName}`. Because it's a service");
        }
    }

    [Fact]
    public void ServiceClassMustBeDecoratedByServiceAttribute()
    {
        var asm = typeof(ServicesModule).Assembly;
        var serviceClasses = asm.GetTypes().Where(x => ObjectHelper.IsInheritedOrImplemented(x, typeof(IService)));
        var badGuys = serviceClasses.Where(x => !ObjectHelper.HasAttribute<ServiceAttribute>(x, true));

        foreach (var serviceClass in badGuys)
        {
            Assert.Fail($"{serviceClass} must be decorated by `[Service]`. Because it's a service");
        }
    }

    [Fact]
    public void ServiceClassesMustBeInternal()
    {
        var asm = typeof(ServicesModule).Assembly;
        var servicesByServiceAttr = asm.GetTypes().Where(x => x.GetCustomAttribute<ServiceAttribute>() != null);
        foreach (var serviceClass in servicesByServiceAttr)
        {
            if (serviceClass.IsPublic)
            {
                Assert.Fail($"{serviceClass} cannot be public. Because it's a service");
            }
        }
    }
}