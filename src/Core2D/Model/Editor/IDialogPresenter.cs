using System.Collections.Generic;
using Core2D.ViewModels.Editor;

namespace Core2D.Model.Editor
{
    public interface IDialogPresenter
    {
        IList<DialogViewModel> Dialogs { get; set; }
        void ShowDialog(DialogViewModel dialogViewModel);
        void CloseDialog(DialogViewModel dialogViewModel);
    }
}
