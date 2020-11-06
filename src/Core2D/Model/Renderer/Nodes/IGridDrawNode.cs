namespace Core2D.Renderer
{
    public interface IGridDrawNode : IDrawNode
    {
        IGrid Grid { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
    }
}
