using Core2D.Renderer;
using Core2D.Style;

namespace Core2D
{
    public interface IDrawable
    {
        ShapeStyleViewModel StyleViewModel { get; set; }

        bool IsStroked { get; set; }

        bool IsFilled { get; set; }

        void DrawShape(object dc, IShapeRenderer renderer);

        void DrawPoints(object dc, IShapeRenderer renderer);

        bool Invalidate(IShapeRenderer renderer);
    }
}
