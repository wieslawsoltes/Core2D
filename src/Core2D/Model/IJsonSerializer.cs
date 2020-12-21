#nullable disable
namespace Core2D.Model
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T value);

        T Deserialize<T>(string json);
    }
}
