using System;

namespace Core2D.History
{
    public interface IHistory
    {
        void Snapshot<T>(T previous, T next, Action<T> update);

        bool CanUndo();

        bool CanRedo();

        bool Undo();

        bool Redo();

        void Reset();
    }
}
