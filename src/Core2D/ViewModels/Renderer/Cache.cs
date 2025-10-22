// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Renderer;

namespace Core2D.ViewModels.Renderer;

public class Cache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
{
    private IDictionary<TKey, TValue?> _storage;
    private readonly Action<TValue>? _dispose;

    public Cache(Action<TValue>? dispose = null)
    {
        _dispose = dispose;
        _storage = new Dictionary<TKey, TValue?>();
    }

    public TValue? Get(TKey key)
    {
        return _storage.TryGetValue(key, out var data) ? data : default;
    }

    public void Set(TKey key, TValue? value)
    {
        if (_storage.ContainsKey(key))
        {
            _storage[key] = value;
        }
        else
        {
            _storage.Add(key, value);
        }
    }

    public void Reset()
    {
        if (_dispose is { })
        {
            foreach (var data in _storage)
            {
                if (data.Value is not null)
                {
                    _dispose(data.Value);
                }
            }
        }
        _storage.Clear();
        _storage = new Dictionary<TKey, TValue?>();
    }
}
