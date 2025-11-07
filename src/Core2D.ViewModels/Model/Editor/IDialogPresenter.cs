// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using Core2D.ViewModels.Editor;

namespace Core2D.Model.Editor;

public interface IDialogPresenter
{
    IList<DialogViewModel>? Dialogs { get; set; }

    void ShowDialog(DialogViewModel? dialogViewModel);

    void CloseDialog(DialogViewModel? dialogViewModel);
}
