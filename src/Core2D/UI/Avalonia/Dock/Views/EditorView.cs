// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using DM = Dock.Model;

namespace Core2D.UI.Avalonia.Dock.Views
{
    /// <summary>
    /// Editor view.
    /// </summary>
    public class EditorView : DM.DockBase
    {
        public override DM.IDockable Clone()
        {
            var editorView = new EditorView();

            DM.CloneHelper.CloneDockProperties(this, editorView);

            return editorView;
        }
    }
}
