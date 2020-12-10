using Core2D.Model.Input;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Editor
{
    public interface ITool
    {
        string Title { get; }
        
        void BeginDown(InputArgs args);

        void BeginUp(InputArgs args);

        void EndDown(InputArgs args);

        void EndUp(InputArgs args);

        void Move(InputArgs args);

        void Move(BaseShapeViewModel shape);

        void Finalize(BaseShapeViewModel shape);

        void Reset();
    }
}
