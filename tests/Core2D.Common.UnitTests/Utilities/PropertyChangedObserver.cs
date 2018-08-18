// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.ComponentModel;

namespace Core2D.Common.UnitTests
{
    public class PropertyChangedObserver
    {
        public List<string> PropertyNames { get; } = new List<string>();

        public PropertyChangedObserver(INotifyPropertyChanged observable)
        {
            observable.PropertyChanged += (sender, e) => PropertyNames.Add(e.PropertyName);
        }
    }
}
