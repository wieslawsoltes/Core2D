
namespace Core2D
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T value);

        T Deserialize<T>(string json);
    }
}
