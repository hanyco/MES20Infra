namespace Library.Interfaces;

public interface IFactory<out TSelf>
{
    static abstract TSelf Create();
}

public interface IFactory<out TSelf, in TArg>
{
    static abstract TSelf Create(TArg arg);
}

public interface IFactory<out TSelf, in TArg1, in TArg2>
{
    static abstract TSelf Create(TArg1 arg1, TArg2 arg2);
}

public interface IFactory<out TSelf, in TArg1, in TArg2, in TArg3>
{
    static abstract TSelf Create(TArg1 arg1, TArg2 arg2, TArg3 arg3);
}

public interface IFactory<out TSelf, in TArg1, in TArg2, in TArg3, in TArg4>
{
    static abstract TSelf Create(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
}

public interface IFactory<out TSelf, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5>
{
    static abstract TSelf Create(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
}

public interface IAsyncFactor<TSelf>
{
    static abstract Task<TSelf> CreateAsync(CancellationToken cancellationToken = default);
}

public interface IAsyncFactor<TSelf, TArg>
{
    static abstract Task<TSelf> CreateAsync(TArg arg, CancellationToken cancellationToken = default);
}
