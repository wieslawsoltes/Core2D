using Core2D.ViewModels.Editor;
using ReactiveUI;

namespace IntegrationTest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ProjectEditorViewModel? _editor;
        
        public ProjectEditorViewModel? Editor
        {
            get => _editor;
            set => this.RaiseAndSetIfChanged(ref _editor, value);
        }
    }
}
