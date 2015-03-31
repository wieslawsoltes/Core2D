// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Core;

namespace Test.Util
{
    public class StyleObserver
    {
        private readonly Action _invalidate;

        public StyleObserver(ContainerEditor editor)
        {
            _invalidate = () =>
            {
                editor.Renderer.ClearCache();
                editor.Container.Invalidate();
            };

            Add(editor.Container.Styles);

            (editor.Container.Styles as ObservableCollection<XStyle>).CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        Add(e.NewItems.Cast<XStyle>());
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        Remove(e.OldItems.Cast<XStyle>());
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        break;
                }

                _invalidate();
            };
        }

        private void PropertyChangedObserver(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            _invalidate();
        }

        private void Add(XStyle style)
        {
            style.PropertyChanged += PropertyChangedObserver;
            style.Stroke.PropertyChanged += PropertyChangedObserver;
            style.Fill.PropertyChanged += PropertyChangedObserver;
        }

        private void Add(IEnumerable<XStyle> styles)
        {
            foreach (var style in styles)
            {
                Add(style);
            }
        }

        private void Remove(XStyle style)
        {
            style.PropertyChanged -= PropertyChangedObserver;
            style.Stroke.PropertyChanged -= PropertyChangedObserver;
            style.Fill.PropertyChanged -= PropertyChangedObserver;
        }

        private void Remove(IEnumerable<XStyle> styles)
        {
            foreach (var style in styles)
            {
                Remove(style);
            }
        }
    }
}
