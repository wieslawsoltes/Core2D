#nullable enable
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;

namespace Core2D.Views.Renderer
{
    public class RenderState
    {
        public FrameContainerViewModel? Container { get; set; }
        public IShapeRenderer? Renderer { get; set; }
        public ISelection? Selection { get; set; }
        public DataFlow? DataFlow { get; set; }
        public RenderType RenderType { get; set; }
    }
}
