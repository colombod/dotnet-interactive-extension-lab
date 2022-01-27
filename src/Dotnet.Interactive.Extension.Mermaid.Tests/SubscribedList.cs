﻿using System.Collections;
using System.Collections.Immutable;

namespace Dotnet.Interactive.Extension.Mermaid.Tests;

public class SubscribedList<T> : IReadOnlyList<T>, IDisposable
{
    private ImmutableArray<T> _list = ImmutableArray<T>.Empty;
    private readonly IDisposable _subscription;

    public SubscribedList(IObservable<T> source)
    {
        _subscription = source.Subscribe(x => { _list = _list.Add(x); });
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_list).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _list.Length;

    public T this[int index] => _list[index];

    public void Dispose() => _subscription.Dispose();
}