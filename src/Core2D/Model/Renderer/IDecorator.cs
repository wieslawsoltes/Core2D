using System.Collections.Generic;
using Core2D.Containers;
using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IDecorator : IDrawable
    {
        LayerContainerViewModel Layer { get; set; }

        IList<BaseShapeViewModel> Shapes { get; set; }

        bool IsVisible { get; }

        void Update(bool rebuild = true);

        void Show();

        void Hide();

        bool HitTest(InputArgs args);

        void Move(InputArgs args);
    }
}
