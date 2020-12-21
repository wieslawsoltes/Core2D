namespace Core2D.Model.Renderer
{
    public interface ICache<in TKey, TValue>
    {
        TValue? Get(TKey key);

        void Set(TKey key, TValue? value);

        void Reset();
    }
}
