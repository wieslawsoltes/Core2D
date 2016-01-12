// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D
{
    /// <summary>
    /// Undo/redo action history.
    /// </summary>
    public class History
    {
        private Stack<UndoRedo> _undos = new Stack<UndoRedo>();
        private Stack<UndoRedo> _redos = new Stack<UndoRedo>();

        /// <summary>
        /// Make undo/redo history snapshot.
        /// </summary>
        /// <param name="previous">The previous state.</param>
        /// <param name="next">The next state</param>
        /// <param name="update">The update method.</param>
        public void Snapshot<T>(T previous, T next, Action<T> update)
        {
            var undo = UndoRedo.Create(() => update(previous), () => update(next));
            if (_redos.Count > 0)
                _redos.Clear();
            _undos.Push(undo);
        }

        /// <summary>
        /// Check if undo action can execute.
        /// </summary>
        /// <returns>True if undo action can execute.</returns>
        public bool CanUndo()
        {
            return _undos.Count > 0;
        }

        /// <summary>
        /// Check if redo action can execute.
        /// </summary>
        /// <returns>True if redo action can execute.</returns>
        public bool CanRedo()
        {
            return _redos.Count > 0;
        }

        /// <summary>
        /// Execute undo action.
        /// </summary>
        /// <returns>True if undo action was executed.</returns>
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

        /// <summary>
        /// Execute redo action.
        /// </summary>
        /// <returns>True if redo action was executed.</returns>
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

        /// <summary>
        /// Reset undo/redo actions history.
        /// </summary>
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
