using Core2D.Style;

namespace Core2D.Renderer
{
    public interface IMarker
    {
        ArrowStyleViewModel Style { get; set; }
        void Draw(object dc);
        void UpdateStyle();
    }
}
