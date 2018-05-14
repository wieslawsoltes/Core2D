// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Dock.Model
{
    /// <summary>
    /// Dock layout.
    /// </summary>
    public class DockLayout : ObservableObject, IDockLayout
    {
        private IList<IDockContainer> _containers;
        private IList<IDockView> _views;
        private IDockView _currentView;

        /// <inheritdoc/>
        public IList<IDockContainer> Containers
        {
            get => _containers;
            set => Update(ref _containers, value);
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
        public void RemoveView(IDockContainer container, int index)
        {
            container.Views.RemoveAt(index);

            if (container.Views.Count > 0)
            {
                container.CurrentView = container.Views[index > 0 ? index - 1 : 0];
            }
        }

        /// <inheritdoc/>
        public void MoveView(IDockContainer container, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                var item = container.Views[sourceIndex];
                container.Views.RemoveAt(sourceIndex);
                container.Views.Insert(targetIndex, item);
                container.CurrentView = item;
            }
            else
            {
                int removeIndex = sourceIndex;
                if (container.Views.Count > removeIndex)
                {
                    var item = container.Views[sourceIndex];
                    container.Views.RemoveAt(removeIndex);
                    container.Views.Insert(targetIndex, item);
                    container.CurrentView = item;
                }
            }
        }

        /// <inheritdoc/>
        public void SwapView(IDockContainer container, int sourceIndex, int targetIndex)
        {
            var item1 = container.Views[sourceIndex];
            var item2 = container.Views[targetIndex];
            container.Views[targetIndex] = item1;
            container.Views[sourceIndex] = item2;
            container.CurrentView = item2;
        }

        /// <inheritdoc/>
        public void MoveView(IDockContainer sourceContainer, IDockContainer targetContainer, int sourceIndex, int targetIndex)
        {
            var item = sourceContainer.Views[sourceIndex];
            sourceContainer.Views.RemoveAt(sourceIndex);
            targetContainer.Views.Insert(targetIndex, item);

            if (sourceContainer.Views.Count > 0)
            {
                sourceContainer.CurrentView = sourceContainer.Views[sourceIndex > 0 ? sourceIndex - 1 : 0];
            }

            if (targetContainer.Views.Count > 0)
            {
                targetContainer.CurrentView = targetContainer.Views[targetIndex];
            }
        }

        /// <inheritdoc/>
        public void SwapView(IDockContainer sourceContainer, IDockContainer targetContainer, int sourceIndex, int targetIndex)
        {
            var item1 = sourceContainer.Views[sourceIndex];
            var item2 = targetContainer.Views[targetIndex];
            sourceContainer.Views[sourceIndex] = item2;
            targetContainer.Views[targetIndex] = item1;

            sourceContainer.CurrentView = item2;
            targetContainer.CurrentView = item1;
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

        /// <summary>
        /// Check whether the <see cref="Containers"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeContainers() => _containers != null;

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
    }
}
