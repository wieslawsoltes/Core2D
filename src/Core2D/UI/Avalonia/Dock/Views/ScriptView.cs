using DM = Dock.Model;

namespace Core2D.UI.Avalonia.Dock.Views
{
    /// <summary>
    /// Script view.
    /// </summary>
    public class ScriptView : DM.DockBase
    {
        public override DM.IDockable Clone()
        {
            var scriptView = new ScriptView();

            DM.CloneHelper.CloneDockProperties(this, scriptView);

            return scriptView;
        }
    }
}
