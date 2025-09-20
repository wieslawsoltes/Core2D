// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;

namespace Core2D.ViewModels.Editor.History;

public readonly struct UndoRedo
{
    public readonly Action? Undo;

    public readonly Action? Redo;

    private UndoRedo(Action? undo, Action? redo)
    {
        Undo = undo;
        Redo = redo;
    }

    public static UndoRedo Create(Action? undo, Action? redo)
    {
        return new UndoRedo(undo, redo);
    }
}
