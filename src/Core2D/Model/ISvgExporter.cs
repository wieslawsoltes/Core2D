#nullable disable
namespace Core2D.Model
{
    public interface ISvgExporter
    {
        string Create(object item, double width, double height);
    }
}
