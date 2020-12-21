#nullable disable
namespace Core2D.Model.Renderer
{
    public interface ICache<TKey, TValue>
    {
        TValue Get(TKey key);

        void Set(TKey key, TValue value);

        void Reset();
    }
}
