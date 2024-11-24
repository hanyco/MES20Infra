using Library.Interfaces;

namespace Library.Types;

public sealed class EmptyDisposable : IDisposable, IEmpty<EmptyDisposable>
{
    private bool _disposedValue;
    public static EmptyDisposable Empty => field ??= NewEmpty();

    public static EmptyDisposable NewEmpty() =>
        new();

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
            }
            this._disposedValue = true;
        }
    }
}