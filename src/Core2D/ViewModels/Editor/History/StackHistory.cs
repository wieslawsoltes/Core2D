using System;
using System.Collections.Generic;
using Core2D.Model.History;

namespace Core2D.ViewModels.Editor.History
{
    public partial class StackHistory : IHistory
    {
        private readonly Stack<UndoRedo> _undos = new Stack<UndoRedo>();
        private readonly Stack<UndoRedo> _redos = new Stack<UndoRedo>();

        void IHistory.Snapshot<T>(T previous, T next, Action<T> update)
        {
            var undo = UndoRedo.Create(() => update(previous), () => update(next));
            if (_redos.Count > 0)
            {
                _redos.Clear();
            }

            _undos.Push(undo);
        }

        bool IHistory.CanUndo()
        {
            return _undos.Count > 0;
        }

        bool IHistory.CanRedo()
        {
            return _redos.Count > 0;
        }

        bool IHistory.Undo()
        {
            if (_undos.Count <= 0)
            {
                return false;
            }

            var undo = _undos.Pop();
            if (undo.Undo is { })
            {
                undo.Undo();
                if (undo.Redo is { })
                {
                    var redo = UndoRedo.Create(undo.Undo, undo.Redo);
                    _redos.Push(redo);
                }
                return true;
            }
            return false;
        }

        bool IHistory.Redo()
        {
            if (_redos.Count <= 0)
            {
                return false;
            }

            var redo = _redos.Pop();
            if (redo.Redo is { })
            {
                redo.Redo();
                if (redo.Undo is { })
                {
                    var undo = UndoRedo.Create(redo.Undo, redo.Redo);
                    _undos.Push(undo);
                }
                return true;
            }
            return false;
        }

        void IHistory.Reset()
        {
            if (_undos is { } && _undos.Count > 0)
            {
                _undos.Clear();
            }

            if (_redos is { } && _redos.Count > 0)
            {
                _redos.Clear();
            }
        }
    }
}
