using System.Collections.Generic;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model
{
    public interface ISelection
    {
        ISet<BaseShapeViewModel> SelectedShapes { get; set; }
    }
}
