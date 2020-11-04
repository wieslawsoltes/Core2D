using Core2D.Shapes;

namespace Core2D
{
    public interface IConnectable
    {
        bool Connect(PointShape point, PointShape target);

        bool Disconnect(PointShape point, out PointShape result);

        bool Disconnect();
    }
}
