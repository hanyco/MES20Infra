﻿namespace Library.DesignPatterns.Markers;

public interface IBuilder<out TResult>
{
    TResult BuildAll();
}

public interface IBuilder
{
    void Build();
}

public interface IBuilder<in TArgs, out TResult>
{
    TResult Build(TArgs args);
}

public interface IBuilderExtender<in TThis>
{
    static abstract void Build(TThis @this);
}

public interface IBuilderExtender<in TThis, out TResult>
{
    static abstract TResult Build(TThis @this);
}

public interface IBuilderExtender<in TThis, in TArgs, out TResult>
{
    static abstract TResult Build(TThis @this, TArgs args);
}