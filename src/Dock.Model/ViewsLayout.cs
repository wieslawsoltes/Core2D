// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Dock.Model
{
    /// <summary>
    /// Views layout.
    /// </summary>
    public class ViewsLayout : ObservableObject, IViewsLayout
    {
        private IList<IView> _views;
        private IList<IViewsPanel> _panels;
        private IView _currentView;

        /// <inheritdoc/>
        public IList<IView> Views
        {
            get => _views;
            set => Update(ref _views, value);
        }

        /// <inheritdoc/>
        public IList<IViewsPanel> Panels
        {
            get => _panels;
            set => Update(ref _panels, value);
        }

        /// <inheritdoc/>
        public IView CurrentView
        {
            get => _currentView;
            set => Update(ref _currentView, value);
        }

        /// <inheritdoc/>
        public void RemoveView(IViewsPanel panel, int index)
        {
            panel.Views.RemoveAt(index);

            if (panel.Views.Count > 0)
            {
                panel.CurrentView = panel.Views[index > 0 ? index - 1 : 0];
            }
        }

        /// <inheritdoc/>
        public void MoveView(IViewsPanel panel, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                var item = panel.Views[sourceIndex];
                panel.Views.RemoveAt(sourceIndex);
                panel.Views.Insert(targetIndex, item);
                panel.CurrentView = item;
            }
            else
            {
                int removeIndex = sourceIndex;
                if (panel.Views.Count > removeIndex)
                {
                    var item = panel.Views[sourceIndex];
                    panel.Views.RemoveAt(removeIndex);
                    panel.Views.Insert(targetIndex, item);
                    panel.CurrentView = item;
                }
            }
        }

        /// <inheritdoc/>
        public void SwapView(IViewsPanel panel, int sourceIndex, int targetIndex)
        {
            var item1 = panel.Views[sourceIndex];
            var item2 = panel.Views[targetIndex];
            panel.Views[targetIndex] = item1;
            panel.Views[sourceIndex] = item2;
            panel.CurrentView = item2;
        }

        /// <inheritdoc/>
        public void MoveView(IViewsPanel sourcePanel, IViewsPanel targetPanel, int sourceIndex, int targetIndex)
        {
            var item = sourcePanel.Views[sourceIndex];
            sourcePanel.Views.RemoveAt(sourceIndex);
            targetPanel.Views.Insert(targetIndex, item);

            if (sourcePanel.Views.Count > 0)
            {
                sourcePanel.CurrentView = sourcePanel.Views[sourceIndex > 0 ? sourceIndex - 1 : 0];
            }

            if (targetPanel.Views.Count > 0)
            {
                targetPanel.CurrentView = targetPanel.Views[targetIndex];
            }
        }

        /// <inheritdoc/>
        public void SwapView(IViewsPanel sourcePanel, IViewsPanel targetPanel, int sourceIndex, int targetIndex)
        {
            var item1 = sourcePanel.Views[sourceIndex];
            var item2 = targetPanel.Views[targetIndex];
            sourcePanel.Views[sourceIndex] = item2;
            targetPanel.Views[targetIndex] = item1;

            sourcePanel.CurrentView = item2;
            targetPanel.CurrentView = item1;
        }

        /// <inheritdoc/>
        public void OnChangeCurrentView(IView view)
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
        /// Check whether the <see cref="Views"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeViews() => _views != null;

        /// <summary>
        /// Check whether the <see cref="Panels"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLeftPanels() => _panels != null;

        /// <summary>
        /// Check whether the <see cref="CurrentView"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentView() => _currentView != null;
    }
}
