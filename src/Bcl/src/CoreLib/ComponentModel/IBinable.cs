﻿namespace Library.ComponentModel;

public interface IAsyncBindable<TViewModel>
{
    Task BindAsync(TViewModel viewModel, CancellationToken cancellationToken = default);
}

public interface IAsyncBindable
{
    Task BindAsync(CancellationToken cancellationToken = default);
}

public interface IAsyncBindableBidirectionalViewModel<TViewModel> : IAsyncBindable<TViewModel>, IBidirectionalViewModel<TViewModel>;

public interface IAsyncBindableUnidirectionalViewModel<TViewModel> : IAsyncBindable<TViewModel>, IUnidirectionalViewModel<TViewModel>;

public interface IBidirectionalViewModel<TViewModel> : IUnidirectionalViewModel<TViewModel>
{
    new TViewModel ViewModel { get; set; }
}

public interface IBinable<TViewMode>
{
    void Bind(TViewMode viewMode);

    void Rebind();
}

public interface IBindable
{
    void Bind();
}

public interface IResetable
{
    void Reset();
}

public interface IResetable<T>
{
    T Reset();
}

public interface IUnidirectionalViewModel<TViewModel>
{
    TViewModel ViewModel { get; }
}

public interface ISupportReadOnly
{
    bool IsReadOnly { get; set; }
}