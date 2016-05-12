// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Avalonia.Interactions.Actions
{
    public class FocusControlAction : AvaloniaObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            (sender as Control)?.Focus();
            return null;
        }
    }
}
