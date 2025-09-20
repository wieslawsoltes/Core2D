// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels.Renderer;

public partial class ImageKeyViewModel : ViewModelBase, IImageKey
{
    [AutoNotify] private string? _key;

    public ImageKeyViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        RemoveImageKey = new RelayCommand<string?>(x => GetProject()?.OnRemoveImageKey(x));

        ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
    }

        
    [IgnoreDataMember]
    public ICommand RemoveImageKey { get; }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }
}
