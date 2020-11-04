using Core2D.Input;
using Core2D.Shapes;

namespace Core2D.Editor
{
    public interface ITool
    {
        string Title { get; }

        void LeftDown(InputArgs args);

        void LeftUp(InputArgs args);

        void RightDown(InputArgs args);

        void RightUp(InputArgs args);

        void Move(InputArgs args);

        void Move(BaseShape shape);

        void Finalize(BaseShape shape);

        void Reset();
    }

    public interface IPathTool : ITool
    {
    }

    public interface IEditorTool : ITool
    {
    }
}
