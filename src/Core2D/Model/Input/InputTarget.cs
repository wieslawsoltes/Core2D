#nullable disable
namespace Core2D.Model.Input
{
    public abstract class InputTarget
    {
        public abstract void BeginDown(InputArgs args);

        public abstract void BeginUp(InputArgs args);

        public abstract void EndDown(InputArgs args);

        public abstract void EndUp(InputArgs args);

        public abstract void Move(InputArgs args);

        public abstract bool IsBeginDownAvailable();

        public abstract bool IsBeginUpAvailable();

        public abstract bool IsEndDownAvailable();

        public abstract bool IsEndUpAvailable();

        public abstract bool IsMoveAvailable();
    }
}
