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
