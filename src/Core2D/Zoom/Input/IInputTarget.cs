
namespace Core2D.UI.Zoom.Input
{
    public interface IInputTarget
    {
        void LeftDown(double x, double y, Modifier modifier);
        void LeftUp(double x, double y, Modifier modifier);
        void RightDown(double x, double y, Modifier modifier);
        void RightUp(double x, double y, Modifier modifier);
        void Move(double x, double y, Modifier modifier);
        double GetWidth();
        double GetHeight();
    }
}
