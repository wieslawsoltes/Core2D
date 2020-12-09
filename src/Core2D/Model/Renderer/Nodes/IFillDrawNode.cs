using Core2D.Style;

namespace Core2D.Renderer
{
    public interface IFillDrawNode : IDrawNode
    {
        BaseColorViewModel ColorViewModel { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
    }
}
