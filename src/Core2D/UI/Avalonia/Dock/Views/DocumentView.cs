// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using DM = Dock.Model;

namespace Core2D.UI.Avalonia.Dock.Views
{
    /// <summary>
    /// Document view.
    /// </summary>
    public class DocumentView : DM.DockBase
    {
        public override DM.IDockable Clone()
        {
            var documentView = new DocumentView();

            DM.CloneHelper.CloneDockProperties(this, documentView);

            return documentView;
        }
    }
}
