using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D
{
    public interface ISelection
    {
        ISet<BaseShape> SelectedShapes { get; set; }
    }
}
