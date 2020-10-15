using Core2D.Style;

namespace Core2D.Renderer
{
    public interface IMarker
    {
        ArrowStyle Style { get; set; }
        void Draw(object dc);
        void UpdateStyle();
    }
}
