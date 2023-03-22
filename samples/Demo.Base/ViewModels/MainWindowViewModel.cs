using CommunityToolkit.Mvvm.ComponentModel;
using Core2D.ViewModels.Editor;

namespace Demo.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ProjectEditorViewModel? _editor;
}