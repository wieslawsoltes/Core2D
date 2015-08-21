// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test2d;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    internal struct UndoRedo
    {
        public readonly Action Undo;
        public readonly Action Redo;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="undo"></param>
        /// <param name="redo"></param>
        public UndoRedo(Action undo, Action redo)
        {
            this.Undo = undo;
            this.Redo = redo;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="undo"></param>
        /// <param name="redo"></param>
        /// <returns></returns>
        public static UndoRedo Create(Action undo, Action redo)
        {
            return new UndoRedo(undo, redo);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class History
    {
        private Stack<UndoRedo> _undos = new Stack<UndoRedo>();
        private Stack<UndoRedo> _redos = new Stack<UndoRedo>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="next"></param>
        /// <param name="update"></param>
        public void Snapshot<T>(T previous, T next, Action<T> update)
        {
            var undo = UndoRedo.Create(() => update(previous), () => update(next));
            if (_redos.Count > 0)
                _redos.Clear();
            _undos.Push(undo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return _undos.Count > 0;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return _redos.Count > 0;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
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
