#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Renderer;

namespace Core2D.ViewModels.Renderer
{
    public partial class ImageKeyViewModel : ViewModelBase, IImageKey
    {
        [AutoNotify] private string? _key;

        public ImageKeyViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }
    }
}
