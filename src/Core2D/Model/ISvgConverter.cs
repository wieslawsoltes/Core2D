#nullable enable
using System.Collections.Generic;
using System.IO;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model;

public interface ISvgConverter
{
    IList<BaseShapeViewModel>? Convert(Stream stream, out double width, out double height);

    IList<BaseShapeViewModel>? FromString(string text, out double width, out double height);
}
