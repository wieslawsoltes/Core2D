#nullable disable
using Core2D.ViewModels.Shapes;

namespace Core2D.Model
{
    public interface IConnectable
    {
        bool Connect(PointShapeViewModel point, PointShapeViewModel target);

        bool Disconnect(PointShapeViewModel point, out PointShapeViewModel result);

        bool Disconnect();
    }
}
