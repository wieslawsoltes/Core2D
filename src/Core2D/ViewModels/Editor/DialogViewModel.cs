#nullable disable
using System;
using Core2D.Model.Editor;

namespace Core2D.ViewModels.Editor
{
    public partial class DialogViewModel : ViewModelBase
    {
        private readonly IDialogPresenter _dialogPresenter;

        [AutoNotify] private string _title;
        [AutoNotify] private bool _isOverlayVisible;
        [AutoNotify] private bool _isTitleBarVisible;
        [AutoNotify] private bool _isCloseButtonVisible;
        [AutoNotify] private ViewModelBase _viewModel;

        public DialogViewModel(IServiceProvider serviceProvider, IDialogPresenter dialogPresenter) : base(serviceProvider)
        {
            _dialogPresenter = dialogPresenter;
        }

        public void Show()
        {
            _dialogPresenter.ShowDialog(this);
        }

        public void Close()
        {
            _dialogPresenter.CloseDialog(this);
        }
    }
}
