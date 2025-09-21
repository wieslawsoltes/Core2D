// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Avalonia.Controls;
using Avalonia.Input;
using Core2D.ViewModels.Shapes;

namespace Core2D.Behaviors.DragAndDrop;

public class InsertBlockDropHandler : DefaultDropHandler
{
    private bool Validate(InsertShapeViewModel insert, DragEventArgs e, bool execute)
    {
        foreach (var format in e.Data.GetDataFormats())
        {
            var data = e.Data.Get(format);
            if (data is BlockShapeViewModel block)
            {
                if (execute)
                {
                    insert.Block = block;
                    insert.UpdateConnectorsFromBlock();
                }
                return true;
            }
        }
        return false;
    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (targetContext is InsertShapeViewModel insert)
        {
            return Validate(insert, e, false);
        }
        return false;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (targetContext is InsertShapeViewModel insert)
        {
            return Validate(insert, e, true);
        }
        return false;
    }
}

