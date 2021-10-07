#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Core2D.ViewModels;

public static class CopyExtensions
{
    public static T? GetCurrentItem<T>(this T? item, ref ImmutableArray<T> previous, ref ImmutableArray<T> next) where T : ViewModelBase
    {
        var currentIndex = item is null ? -1 : previous.IndexOf(item);
        var current = currentIndex == -1 ? null : next[currentIndex];
        return current;
    }

    public static T? GetCurrentItem<T, TU>(
        this T? item, 
        ref ImmutableArray<TU> previous, 
        ref ImmutableArray<TU> next,
        Func<TU, ImmutableArray<T>> getter) 
        where T : ViewModelBase
        where TU : ViewModelBase
    {
        var currentIndex = -1;
        var currentItems = default(TU?);
        var current = default(T?);
        if (item is not null)
        {
            foreach (var p in previous)
            {
                var items = getter(p);
                var index = items.IndexOf(item);
                if (index >= 0)
                {
                    currentItems = p;
                    currentIndex = index;
                    break;
                }
            }
        }

        if (item is not null && currentIndex != -1 && currentItems is not null)
        {
            var index = previous.IndexOf(currentItems);
            if (index >= 0)
            {                   
                var items = getter(next[index]);
                current = items[currentIndex];
            }
        }

        return current;
    }

        
    public static ImmutableArray<T>.Builder CopyShared<T>(this ref ImmutableArray<T> array, IDictionary<object, object>? shared) where T : ViewModelBase
    {
        var copy = ImmutableArray.CreateBuilder<T>();

        foreach (var item in array)
        {
            var itemCopy = item.CopyShared(shared);
            if (itemCopy is not null)
            {
                copy.Add(itemCopy);
            }
        }

        return copy;
    }
        
    public static T? CopyShared<T>(this T? item, IDictionary<object, object>? shared) where T : ViewModelBase
    {
        if (item is null)
        {
            return null;
        }

        if (shared is null)
        {
            return (T)item.Copy(shared);
        }

        if (shared.TryGetValue(item, out var copy))
        {
            return (T)copy;
        }

        copy = item.Copy(shared);
        shared[item] = copy;
        shared[copy] = item;

        return (T)copy;
    }
}