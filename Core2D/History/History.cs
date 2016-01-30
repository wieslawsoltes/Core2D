// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D
{
    /// <summary>
    /// Undo/redo stack based action history.
    /// </summary>
    public class History : IHistory
    {
        private Stack<UndoRedo> _undos = new Stack<UndoRedo>();
        private Stack<UndoRedo> _redos = new Stack<UndoRedo>();

        /// <inheritdoc/>
        public void Snapshot<T>(T previous, T next, Action<T> update)
        {
            var undo = UndoRedo.Create(() => update(previous), () => update(next));
            if (_redos.Count > 0)
                _redos.Clear();
            _undos.Push(undo);
        }

        /// <inheritdoc/>
        public bool CanUndo()
        {
            return _undos.Count > 0;
        }

        /// <inheritdoc/>
        public bool CanRedo()
        {
            return _redos.Count > 0;
        }

        /// <inheritdoc/>
        public bool Undo()
        {
            if (_undos.Count <= 0)
                return false;

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
        public bool Redo()
        {
            if (_redos.Count <= 0)
                return false;

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
        public void Reset()
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
