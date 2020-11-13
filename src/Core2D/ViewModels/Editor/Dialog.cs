using System;
using System.Collections.Generic;

namespace Core2D.Editor
{
    public class Dialog : ViewModelBase
    {
        private readonly IDialogPresenter _dialogPresenter;
        private string _title;
        private bool _isOverlayVisible;
        private bool _isTitleBarVisible;
        private bool _isCloseButtonVisible;
        private ViewModelBase _viewModel;

        public string Title
        {
            get => _title;
            set => RaiseAndSetIfChanged(ref _title, value);
        }

        public bool IsOverlayVisible
        {
            get => _isOverlayVisible;
            set => RaiseAndSetIfChanged(ref _isOverlayVisible, value);
        }

        public bool IsTitleBarVisible
        {
            get => _isTitleBarVisible;
            set => RaiseAndSetIfChanged(ref _isTitleBarVisible, value);
        }

        public bool IsCloseButtonVisible
        {
            get => _isCloseButtonVisible;
            set => RaiseAndSetIfChanged(ref _isCloseButtonVisible, value);
        }

        public ViewModelBase ViewModel
        {
            get => _viewModel;
            set => RaiseAndSetIfChanged(ref _viewModel, value);
        }

        public Dialog(IDialogPresenter dialogPresenter)
        {
            _dialogPresenter = dialogPresenter;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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
