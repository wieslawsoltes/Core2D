using Core2D.ViewModels.Style;

namespace Core2D.Model.Renderer
{
    public interface IGrid
    {
        bool IsGridEnabled { get; set; }

        bool IsBorderEnabled { get; set; }

        double GridOffsetLeft { get; set; }

        double GridOffsetTop { get; set; }

        double GridOffsetRight { get; set; }

        double GridOffsetBottom { get; set; }

        double GridCellWidth { get; set; }

        double GridCellHeight { get; set; }

        BaseColorViewModel GridStrokeColor { get; set; }

        double GridStrokeThickness { get; set; }

        bool IsDirty();
    }
}
