using HanyCo.Infra.Internals.Data.DataSources;

using Library.Validations;

using Microsoft.Extensions.DependencyInjection;

namespace InfraTestProject;

public class ServicesFixture : IDisposable
{
    private readonly IServiceProvider _services;
    private bool _disposedValue;

    public ServicesFixture()
    {
        this._services = TestStartup.GetServiceProvider();

        this.WriteDbContext = this._services.GetService<InfraWriteDbContext>().NotNull();
        this.ReadDbContext = this._services.GetService<InfraReadDbContext>().NotNull();
    }

    public InfraReadDbContext ReadDbContext { get; }

    public InfraWriteDbContext WriteDbContext { get; }

    void IDisposable.Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public TService GetService<TService>()
        => this._services.GetService<TService>().NotNull();

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                this.WriteDbContext?.Dispose();
                this.ReadDbContext?.Dispose();
            }

            this._disposedValue = true;
        }
    }
}
