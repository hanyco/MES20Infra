namespace Library.ComponentModel;

public interface IAsyncInitialzable
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

public interface IInitialzable
{
    void Initialize();
}

public interface IInitialzable<in TArgs>
{
    void Initialize(TArgs args);
}