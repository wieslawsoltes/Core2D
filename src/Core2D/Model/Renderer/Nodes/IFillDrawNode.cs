using Core2D.ViewModels.Style;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IFillDrawNode : IDrawNode
    {
        BaseColorViewModel Color { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
    }
}
