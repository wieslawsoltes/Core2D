
namespace Core2D
{
    public interface ISelectable
    {
        void Move(ISelection selection, decimal dx, decimal dy);

        void Select(ISelection selection);

        void Deselect(ISelection selection);
    }
}
