using System.Collections.Generic;

namespace Core2D.Editor
{
    public interface IDialogPresenter
    {
        IList<Dialog> Dialogs { get; set; }
        void ShowDialog(Dialog dialog);
        void CloseDialog(Dialog dialog);
    }
}
