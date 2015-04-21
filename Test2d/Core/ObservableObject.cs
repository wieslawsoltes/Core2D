// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.ComponentModel;

namespace Test2d
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
