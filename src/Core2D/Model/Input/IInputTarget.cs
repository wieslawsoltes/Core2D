namespace Core2D.Input
{
    public interface IInputTarget
    {
        void BeginDown(InputArgs args);

        void BeginUp(InputArgs args);

        void EndDown(InputArgs args);

        void EndUp(InputArgs args);

        void Move(InputArgs args);

        bool IsBeginDownAvailable();

        bool IsBeginUpAvailable();

        bool IsEndDownAvailable();

        bool IsEndUpAvailable();

        bool IsMoveAvailable();
    }
}
