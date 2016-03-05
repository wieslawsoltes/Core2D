// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex;
using Perspex.Controls;
using Perspex.Xaml.Interactivity;

namespace Core2D.Perspex.Interactions.Actions
{
    public class FocusControlAction : PerspexObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            (sender as Control)?.Focus();
            return null;
        }
    }
}
