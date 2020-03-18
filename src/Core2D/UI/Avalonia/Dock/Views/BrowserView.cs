using DM = Dock.Model;

namespace Core2D.UI.Avalonia.Dock.Views
{
    /// <summary>
    /// Browser view.
    /// </summary>
    public class BrowserView : DM.DockBase
    {
        public override DM.IDockable Clone()
        {
            var browserView = new BrowserView();

            DM.CloneHelper.CloneDockProperties(this, browserView);

            return browserView;
        }
    }
}
