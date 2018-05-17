// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;

namespace Dock.Model
{
    /// <summary>
    /// Dock layout.
    /// </summary>
    public class DockLayout : DockBase, IDockLayout
    {
        private IDock _currentView;
        private IList<IDock> _children;
        private IDockFactory _factory;

        /// <inheritdoc/>
        public override string Title { get; }

        /// <inheritdoc/>
        public override object Context { get; }

        /// <inheritdoc/>
        public IDock CurrentView
        {
            get => _currentView;
            set => Update(ref _currentView, value);
        }

        /// <inheritdoc/>
        public IList<IDock> Children
        {
            get => _children;
            set => Update(ref _children, value);
        }

        /// <inheritdoc/>
        public IDockFactory Factory
        {
            get => _factory;
            set => Update(ref _factory, value);
        }

        /// <inheritdoc/>
        public void RemoveView(IDockLayout layout, int index)
        {
            layout.Children.RemoveAt(index);

            if (layout.Children.Count > 0)
            {
                layout.CurrentView = layout.Children[index > 0 ? index - 1 : 0];
            }
        }

        /// <inheritdoc/>
        public void MoveView(IDockLayout layout, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                var item = layout.Children[sourceIndex];
                layout.Children.RemoveAt(sourceIndex);
                layout.Children.Insert(targetIndex, item);
                layout.CurrentView = item;
            }
            else
            {
                int removeIndex = sourceIndex;
                if (layout.Children.Count > removeIndex)
                {
                    var item = layout.Children[sourceIndex];
                    layout.Children.RemoveAt(removeIndex);
                    layout.Children.Insert(targetIndex, item);
                    layout.CurrentView = item;
                }
            }
        }

        /// <inheritdoc/>
        public void SwapView(IDockLayout layout, int sourceIndex, int targetIndex)
        {
            var item1 = layout.Children[sourceIndex];
            var item2 = layout.Children[targetIndex];
            layout.Children[targetIndex] = item1;
            layout.Children[sourceIndex] = item2;
            layout.CurrentView = item2;
        }

        /// <inheritdoc/>
        public void MoveView(IDockLayout sourceLayout, IDockLayout targetLayout, int sourceIndex, int targetIndex)
        {
            var item = sourceLayout.Children[sourceIndex];
            sourceLayout.Children.RemoveAt(sourceIndex);
            targetLayout.Children.Insert(targetIndex, item);

            if (sourceLayout.Children.Count > 0)
            {
                sourceLayout.CurrentView = sourceLayout.Children[sourceIndex > 0 ? sourceIndex - 1 : 0];
            }

            if (targetLayout.Children.Count > 0)
            {
                targetLayout.CurrentView = targetLayout.Children[targetIndex];
            }
        }

        /// <inheritdoc/>
        public void SwapView(IDockLayout sourceLayout, IDockLayout targetLayout, int sourceIndex, int targetIndex)
        {
            var item1 = sourceLayout.Children[sourceIndex];
            var item2 = targetLayout.Children[targetIndex];
            sourceLayout.Children[sourceIndex] = item2;
            targetLayout.Children[targetIndex] = item1;

            sourceLayout.CurrentView = item2;
            targetLayout.CurrentView = item1;
        }

        /// <inheritdoc/>
        public void OnChangeCurrentView(IDock view)
        {
            if (view != null && _currentView != null && view != _currentView)
            {
                _currentView.CloseWindows();
            }

            if (view != null && _currentView != view)
            {
                CurrentView = view;
            }

            if (_currentView != null)
            {
                _currentView.ShowWindows();
            }
        }

        /// <inheritdoc/>
        public void OnChangeCurrentView(string title)
        {
            OnChangeCurrentView(_children.FirstOrDefault(view => view.Title == title));
        }

        /// <summary>
        /// Check whether the <see cref="CurrentView"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentView() => _currentView != null;

        /// <summary>
        /// Check whether the <see cref="Children"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeChildren() => _children != null;

        /// <summary>
        /// Check whether the <see cref="Factory"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFactory() => false;
    }
}
