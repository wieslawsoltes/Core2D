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
