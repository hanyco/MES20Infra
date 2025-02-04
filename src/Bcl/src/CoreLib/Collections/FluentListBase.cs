﻿using Library.DesignPatterns.Markers;

using System.Collections;

namespace Library.Collections;

[Fluent]
public abstract class FluentListBase<TItem, TList> : IFluentList<TList, TItem>, IBuilder<List<TItem>>
    where TList : FluentListBase<TItem, TList>
{
    private readonly List<TItem> _list;

    protected FluentListBase(List<TItem> list) =>
        this._list = list ?? [];

    protected FluentListBase(IEnumerable<TItem> list) =>
        this._list = new List<TItem>(list);

    protected FluentListBase(int capacity) =>
        this._list = new List<TItem>(capacity);

    protected FluentListBase() =>
        this._list = [];

    public int Count => this._list.Count;

    bool IFluentCollection<TList, TItem>.IsReadOnly { get; }

    public TItem this[int index]
    {
        get => this._list[index];
        set => this._list[index] = value;
    }

    private TList This => this.Cast().As<TList>()!;

    public TList Add(TItem item) =>
        this.This.Fluent(() => this._list.Add(item));

    public TList AddIf(Func<TItem, bool> predicate, TItem item)
    {
        if (predicate(item))
        {
            this._list.Add(item);
        }

        return this.This;
    }

    public TList AddIf(Func<bool> predicate, Func<TItem> getItem)
    {
        if (predicate())
        {
            this._list.Add(getItem());
        }

        return this.This;
    }

    public TList AddRange(IEnumerable<TItem> items)
    {
        if (items?.Any() is true)
        {
            this._list.AddRange(items);
        }
        return this.This;
    }

    public List<TItem> AsList() =>
            this._list;

    public List<TItem> Build() =>
        this.AsList();

    public TList Clear() =>
        this.This.Fluent(this._list.Clear);

    public (TList List, bool Result) Contains(TItem item) =>
        (this.This, this._list.Contains(item));

    public TList CopyTo(TItem[] array, int arrayIndex) =>
        this.This.Fluent(() => this._list.CopyTo(array, arrayIndex));

    public IEnumerator<TItem> GetEnumerator() =>
        this._list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)this._list).GetEnumerator();

    public (TList List, int Result) IndexOf(TItem item) =>
        (this.This, this._list.IndexOf(item));

    public TList Insert(int index, TItem item) =>
        this.This.Fluent(() => this._list.Insert(index, item));

    public TList Remove(TItem item) =>
        this.This.Fluent(this._list.Remove(item));

    public TList RemoveAt(int index) =>
        this.This.Fluent(() => this._list.RemoveAt(index));
}