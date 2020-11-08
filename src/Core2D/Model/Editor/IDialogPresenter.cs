namespace Core2D.Editor
{
    public interface IDialogPresenter
    {
        ViewModelBase Dialog { get; set; }
        void OnCloseDialog();
    }
}
