using System.Collections.Generic;

namespace Core2D.Editor
{
    public interface IDialogPresenter
    {
        IList<DialogViewModel> Dialogs { get; set; }
        void ShowDialog(DialogViewModel dialogViewModel);
        void CloseDialog(DialogViewModel dialogViewModel);
    }
}
