// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.History;

namespace Core2D.ViewModels.Editor.History;

public class StackHistory : IHistory
{
    private readonly Stack<UndoRedo> _undoStack = new();
    private readonly Stack<UndoRedo> _redoStack = new();

    void IHistory.Snapshot<T>(T previous, T next, Action<T> update)
    {
        var undo = UndoRedo.Create(() => update(previous), () => update(next));
        if (_redoStack.Count > 0)
        {
            _redoStack.Clear();
        }

        _undoStack.Push(undo);
    }

    bool IHistory.CanUndo()
    {
        return _undoStack.Count > 0;
    }

    bool IHistory.CanRedo()
    {
        return _redoStack.Count > 0;
    }

    bool IHistory.Undo()
    {
        if (_undoStack.Count <= 0)
        {
            return false;
        }

        var undo = _undoStack.Pop();
        if (undo.Undo is { })
        {
            undo.Undo();
            if (undo.Redo is { })
            {
                var redo = UndoRedo.Create(undo.Undo, undo.Redo);
                _redoStack.Push(redo);
            }
            return true;
        }
        return false;
    }

    bool IHistory.Redo()
    {
        if (_redoStack.Count <= 0)
        {
            return false;
        }

        var redo = _redoStack.Pop();
        if (redo.Redo is { })
        {
            redo.Redo();
            if (redo.Undo is { })
            {
                var undo = UndoRedo.Create(redo.Undo, redo.Redo);
                _undoStack.Push(undo);
            }
            return true;
        }
        return false;
    }

    void IHistory.Reset()
    {
        if (_undoStack.Count > 0)
        {
            _undoStack.Clear();
        }

        if (_redoStack.Count > 0)
        {
            _redoStack.Clear();
        }
    }
}
