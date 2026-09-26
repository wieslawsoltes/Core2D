// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;

namespace Core2D.Controls.Studio;

/// <summary>The workspace header; commands and document state are supplied by the editor view model.</summary>
public partial class WorkspaceHeader : UserControl
{
    /// <summary>Initializes the declarative workspace header.</summary>
    public WorkspaceHeader()
    {
        InitializeComponent();
    }
}
