
namespace Core2D.Input
{
    public interface IInputTarget
    {
        void LeftDown(InputArgs args);

        void LeftUp(InputArgs args);

        void RightDown(InputArgs args);

        void RightUp(InputArgs args);

        void Move(InputArgs args);

        bool IsLeftDownAvailable();

        bool IsLeftUpAvailable();

        bool IsRightDownAvailable();

        bool IsRightUpAvailable();

        bool IsMoveAvailable();
    }
}
