using DM = Dock.Model;

namespace Core2D.UI.Dock.Views
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
