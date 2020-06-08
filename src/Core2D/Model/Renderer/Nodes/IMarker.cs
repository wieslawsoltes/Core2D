using Core2D.Style;

namespace Core2D.Renderer
{
    public interface IMarker
    {
        IArrowStyle Style { get; set; }
        void Draw(object dc);
        void UpdateStyle();
    }
}
