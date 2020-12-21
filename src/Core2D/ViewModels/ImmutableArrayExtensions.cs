#nullable disable
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.ViewModels;

namespace Core2D.ViewModels
{
    public static class ImmutableArrayExtensions
    {
        public static ImmutableArray<T>.Builder Copy<T>(this ref ImmutableArray<T> array, IDictionary<object, object> shared) where T : ViewModelBase
        {
            var copy = ImmutableArray.CreateBuilder<T>();

            foreach (var item in array)
            {
                copy.Add((T)item.Copy(shared));
            }

            return copy;
        }
    }
}
