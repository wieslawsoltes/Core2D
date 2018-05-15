// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;

namespace Dock.Model
{
    /// <summary>
    /// Dock base.
    /// </summary>
    public class DockLayout : ObservableObject, IDockLayout
    {
        private int _row;
        private int _column;
        private IList<IDockView> _views;
        private IDockView _currentView;
        private IList<IDockLayout> _children;

        /// <inheritdoc/>
        public int Row
        {
            get => _row;
            set => Update(ref _row, value);
        }

        /// <inheritdoc/>
        public int Column
        {
            get => _column;
            set => Update(ref _column, value);
        }

        /// <inheritdoc/>
        public IList<IDockView> Views
        {
            get => _views;
            set => Update(ref _views, value);
        }

        /// <inheritdoc/>
        public IDockView CurrentView
        {
            get => _currentView;
            set => Update(ref _currentView, value);
        }

        /// <inheritdoc/>
        public IList<IDockLayout> Children
        {
            get => _children;
            set => Update(ref _children, value);
        }

        /// <inheritdoc/>
        public void RemoveView(IDockLayout layout, int index)
        {
            layout.Views.RemoveAt(index);

            if (layout.Views.Count > 0)
            {
                layout.CurrentView = layout.Views[index > 0 ? index - 1 : 0];
            }
        }

        /// <inheritdoc/>
        public void MoveView(IDockLayout layout, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                var item = layout.Views[sourceIndex];
                layout.Views.RemoveAt(sourceIndex);
                layout.Views.Insert(targetIndex, item);
                layout.CurrentView = item;
            }
            else
            {
                int removeIndex = sourceIndex;
                if (layout.Views.Count > removeIndex)
                {
                    var item = layout.Views[sourceIndex];
                    layout.Views.RemoveAt(removeIndex);
                    layout.Views.Insert(targetIndex, item);
                    layout.CurrentView = item;
                }
            }
        }

        /// <inheritdoc/>
        public void SwapView(IDockLayout layout, int sourceIndex, int targetIndex)
        {
            var item1 = layout.Views[sourceIndex];
            var item2 = layout.Views[targetIndex];
            layout.Views[targetIndex] = item1;
            layout.Views[sourceIndex] = item2;
            layout.CurrentView = item2;
        }

        /// <inheritdoc/>
        public void MoveView(IDockLayout sourceLayout, IDockLayout targetLayout, int sourceIndex, int targetIndex)
        {
            var item = sourceLayout.Views[sourceIndex];
            sourceLayout.Views.RemoveAt(sourceIndex);
            targetLayout.Views.Insert(targetIndex, item);

            if (sourceLayout.Views.Count > 0)
            {
                sourceLayout.CurrentView = sourceLayout.Views[sourceIndex > 0 ? sourceIndex - 1 : 0];
            }

            if (targetLayout.Views.Count > 0)
            {
                targetLayout.CurrentView = targetLayout.Views[targetIndex];
            }
        }

        /// <inheritdoc/>
        public void SwapView(IDockLayout sourceLayout, IDockLayout targetLayout, int sourceIndex, int targetIndex)
        {
            var item1 = sourceLayout.Views[sourceIndex];
            var item2 = targetLayout.Views[targetIndex];
            sourceLayout.Views[sourceIndex] = item2;
            targetLayout.Views[targetIndex] = item1;

            sourceLayout.CurrentView = item2;
            targetLayout.CurrentView = item1;
        }

        /// <inheritdoc/>
        public void OnChangeCurrentView(IDockView view)
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
            OnChangeCurrentView(_views.FirstOrDefault(view => view.Title == title));
        }

        /// <summary>
        /// Check whether the <see cref="Row"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRow() => true;

        /// <summary>
        /// Check whether the <see cref="Column"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeColumn() => true;

        /// <summary>
        /// Check whether the <see cref="Views"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeViews() => _views != null;

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
    }
}
