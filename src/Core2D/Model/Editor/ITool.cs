using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor
{
    public interface ITool
    {
        string Title { get; }

        void BeginDown(InputArgs args);

        void BeginUp(InputArgs args);

        void EndDown(InputArgs args);

        void EndUp(InputArgs args);

        void Move(InputArgs args);

        void Move(BaseShape shape);

        void Finalize(BaseShape shape);

        void Reset();
    }
}
