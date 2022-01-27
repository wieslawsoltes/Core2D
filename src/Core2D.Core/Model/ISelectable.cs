#nullable enable
namespace Core2D.Model;

public interface ISelectable
{
    void Move(ISelection? selection, double dx, double dy);

    void Select(ISelection? selection);

    void Deselect(ISelection? selection);
}
