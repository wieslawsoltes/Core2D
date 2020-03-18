using DM = Dock.Model;

namespace Core2D.UI.Avalonia.Dock.Views
{
    /// <summary>
    /// About view.
    /// </summary>
    public class AboutView : DM.DockBase
    {
        public override DM.IDockable Clone()
        {
            var aboutView = new AboutView();

            DM.CloneHelper.CloneDockProperties(this, aboutView);

            return aboutView;
        }
    }
}
