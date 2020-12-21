#nullable disable
using Core2D.ViewModels.Style;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IMarker
    {
        ArrowStyleViewModel Style { get; set; }
        void Draw(object dc);
        void UpdateStyle();
    }
}
