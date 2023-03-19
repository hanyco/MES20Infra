using System.ComponentModel;

using Library.Threading.MultistepProgress;

using Xunit.Abstractions;

namespace InfraTestProject.Tests;

public abstract class ServiceTestBase<TService, TServiceFixture> : IClassFixture<TServiceFixture>
    where TServiceFixture : ServicesFixture<TService>
{
    public ServiceTestBase(TServiceFixture fixture)
    {
        this.Fixture = fixture;
        this.Service = this.Fixture.Service;
    }

    public TService Service { get; }

    protected TServiceFixture Fixture { get; }
}