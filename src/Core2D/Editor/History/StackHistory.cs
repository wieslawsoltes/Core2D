using System;
using System.Collections.Generic;
using Core2D.History;

namespace Core2D.Editor.History
{
    /// <summary>
    /// Undo/redo stack based action history.
    /// </summary>
    internal sealed class StackHistory : IHistory
    {
        private readonly Stack<UndoRedo> _undos = new Stack<UndoRedo>();
        private readonly Stack<UndoRedo> _redos = new Stack<UndoRedo>();

        /// <inheritdoc/>
        void IHistory.Snapshot<T>(T previous, T next, Action<T> update)
        {
            var undo = UndoRedo.Create(() => update(previous), () => update(next));
            if (_redos.Count > 0)
            {
                _redos.Clear();
            }

            _undos.Push(undo);
        }

        /// <inheritdoc/>
        bool IHistory.CanUndo()
        {
            return _undos.Count > 0;
        }

        /// <inheritdoc/>
        bool IHistory.CanRedo()
        {
            return _redos.Count > 0;
        }

        /// <inheritdoc/>
        bool IHistory.Undo()
        {
            if (_undos.Count <= 0)
            {
                return false;
            }

            var undo = _undos.Pop();
            if (undo.Undo != null)
            {
                undo.Undo();
                if (undo.Redo != null)
                {
                    var redo = UndoRedo.Create(undo.Undo, undo.Redo);
                    _redos.Push(redo);
                }
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        bool IHistory.Redo()
        {
            if (_redos.Count <= 0)
            {
                return false;
            }

            var redo = _redos.Pop();
            if (redo.Redo != null)
            {
                redo.Redo();
                if (redo.Undo != null)
                {
                    var undo = UndoRedo.Create(redo.Undo, redo.Redo);
                    _undos.Push(undo);
                }
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        void IHistory.Reset()
        {
            if (_undos != null && _undos.Count > 0)
            {
                _undos.Clear();
            }

            if (_redos != null && _redos.Count > 0)
            {
                _redos.Clear();
            }
        }
    }
}
