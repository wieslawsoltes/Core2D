// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using DM = Dock.Model;

namespace Core2D.UI.Avalonia.Dock.Views
{
    /// <summary>
    /// Dashboard view.
    /// </summary>
    public class DashboardView : DM.DockBase
    {
        public override DM.IDockable Clone()
        {
            var dashboardView = new DashboardView();

            DM.CloneHelper.CloneDockProperties(this, dashboardView);

            return dashboardView;
        }
    }
}
