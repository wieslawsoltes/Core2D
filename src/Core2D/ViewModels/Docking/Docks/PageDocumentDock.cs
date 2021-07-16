using Core2D.ViewModels.Docking.Documents;
using Dock.Model.ReactiveUI.Controls;
using ReactiveUI;

namespace Core2D.ViewModels.Docking.Docks
{
    public class PageDocumentDock : DocumentDock
    {
        public PageDocumentDock()
        {
            CreateDocument = ReactiveCommand.Create(CreatePage);
        }

        private void CreatePage()
        {
            if (!CanCreateDocument)
            {
                return;
            }

            var page = new PageViewModel();

            Factory?.AddDockable(this, page);
            Factory?.SetActiveDockable(page);
            Factory?.SetFocusedDockable(this, page);
        }
    }
}
