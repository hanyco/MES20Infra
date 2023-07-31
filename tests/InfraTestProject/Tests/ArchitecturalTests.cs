using System.Reflection;

using HanyCo.Infra.Markers;

using Library.Interfaces;

using UiServices;

namespace InfraTestProject.Tests;

public sealed class ArchitecturalTests
{
    [Fact]
    public void _01_ServiceClassesMustImplementIServiceInterface()
    {
        var asm = typeof(ServicesModule).Assembly;
        var serviceClasses = asm.GetTypes().Where(x => ObjectHelper.HasAttribute<ServiceAttribute>(x, true));
        var badGuys = serviceClasses.Where(x => !ObjectHelper.IsInheritedOrImplemented(x, typeof(IService)));
        foreach (var serviceClass in badGuys)
        {
            Assert.Fail($"{serviceClass} must be inherited from `IService`. Because it's a service");
        }
    }

    [Fact]
    public void _02_ServiceClassMustBeDecoratedByServiceAttribute()
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
    public void _03_ServiceClassesMustBeInternal()
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