#nullable disable
namespace Core2D.Model
{
    public interface IXamlExporter
    {
        string Create(object item, string key);
    }
}
