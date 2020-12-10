using System.Collections.Generic;
using Core2D.Model.Input;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer
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
