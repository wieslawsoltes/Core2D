#nullable enable
namespace Core2D.Model.Renderer;

public interface ICache<in TKey, TValue> where TKey : notnull
{
    TValue? Get(TKey key);

    void Set(TKey key, TValue? value);

    void Reset();
}