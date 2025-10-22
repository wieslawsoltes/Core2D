// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Editor;

public class AboutInfoViewModel : ViewModelBase
{
    public string? Title { get; set; }

    public string? Version { get; set; }

    public string? Description { get; set; }

    public string? Copyright { get; set; }

    public string? License { get; set; }

    public AboutInfoViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    public override string ToString() =>
        $"{nameof(Title)}: {Title}{Environment.NewLine}" +
        $"{nameof(Version)}: {Version}{Environment.NewLine}" +
        $"{nameof(Description)}: {Description}{Environment.NewLine}" +
        $"{nameof(Copyright)}: {Copyright}{Environment.NewLine}" +
        $"{nameof(License)}: {License}{Environment.NewLine}";
}
