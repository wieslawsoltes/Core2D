#nullable disable
using System.Collections.Generic;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model
{
    public interface ISvgConverter
    {
        IList<BaseShapeViewModel> Convert(string path, out double width, out double height);

        IList<BaseShapeViewModel> FromString(string text, out double width, out double height);
    }
}
