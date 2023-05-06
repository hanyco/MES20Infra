using System.Reflection;

using HanyCo.Infra.Markers;

using Library.Interfaces;

using UiServices;

namespace InfraTestProject.Tests;

public sealed class ArchitecturalTests
{
    [Fact]
    public void _01_ServiceClassMustBeDecoratedByServiceAttribute()
    {
        var asm = typeof(ServicesModule).Assembly;
        var serviceClasses = asm.GetTypes().Where(x => ObjectHelper.IsInheritedOrImplemented(x, typeof(IService)));
        var badBoys = serviceClasses.Where(x => x.GetCustomAttribute<ServiceAttribute>() == null);

        foreach (var serviceClass in badBoys)
        {
            if (serviceClass.GetCustomAttribute<ServiceAttribute>() != null)
            {
                Assert.Fail($"{serviceClass} must be decorated by [Service]. Because it's a service");
            }
        }
    }

    [Fact]
    public void _02_ServiceClassesMustImplementIServiceInterface()
    {
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