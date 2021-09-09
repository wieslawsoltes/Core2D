#nullable enable
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels.Renderer
{
    public partial class ImageKeyViewModel : ViewModelBase, IImageKey
    {
        [AutoNotify] private string? _key;

        public ImageKeyViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            RemoveImageKey = new Command<string?>(x => GetProject()?.OnRemoveImageKey(x));

            ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
        }

        public ICommand RemoveImageKey { get; }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }
    }
}
