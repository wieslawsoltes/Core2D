using Core2D.Shapes;

namespace Core2D
{
    public interface IConnectable
    {
        bool Connect(PointShapeViewModel point, PointShapeViewModel target);

        bool Disconnect(PointShapeViewModel point, out PointShapeViewModel result);

        bool Disconnect();
    }
}
