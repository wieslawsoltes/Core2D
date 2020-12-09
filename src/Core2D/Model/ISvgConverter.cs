using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D
{
    public interface ISvgConverter
    {
        IList<BaseShapeViewModel> Convert(string path, out double width, out double height);

        IList<BaseShapeViewModel> FromString(string text, out double width, out double height);
    }
}
