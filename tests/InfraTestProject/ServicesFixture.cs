using HanyCo.Infra.Internals.Data.DataSources;

using Library.Validations;

using Microsoft.Extensions.DependencyInjection;

namespace InfraTestProject;

public abstract class ServicesFixture<TService> : IDisposable
{
    private readonly IServiceProvider _services;
    private bool _disposedValue;

    public ServicesFixture()
    {
        this._services = TestStartup.GetServiceProvider();

        this.WriteDbContext = this._services.GetService<InfraWriteDbContext>().NotNull();
        this.ReadDbContext = this._services.GetService<InfraReadDbContext>().NotNull();
        this.Service = this.InitializeService();
    }

    public InfraReadDbContext ReadDbContext { get; }

    public TService Service { get; }

    public InfraWriteDbContext WriteDbContext { get; }

    void IDisposable.Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public Service GetService<Service>()
        => this._services.GetService<Service>().NotNull();

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

    protected virtual void OnInitializingService(in TService service)
    {
    }

    private TService InitializeService()
    {
        var result = this.GetService<TService>().NotNull();
        this.OnInitializingService(result);
        return result;
    }
}