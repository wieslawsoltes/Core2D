using DM = Dock.Model;

namespace Core2D.UI.Avalonia.Dock.Views
{
    /// <summary>
    /// Export view.
    /// </summary>
    public class ExportView : DM.DockBase
    {
        public override DM.IDockable Clone()
        {
            var exportView = new ExportView();

            DM.CloneHelper.CloneDockProperties(this, exportView);

            return exportView;
        }
    }
}
