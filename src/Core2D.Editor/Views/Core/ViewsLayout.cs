// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Editor.Views.Core
{
    /// <summary>
    /// Views layout.
    /// </summary>
    public class ViewsLayout : ObservableObject
    {
        private ViewsPanel _leftPanelTop;
        private ViewsPanel _leftPanelBottom;
        private ViewsPanel _rightPanelTop;
        private ViewsPanel _rightPanelBottom;
        private IView _currentView;

        /// <summary>
        /// Gets or sets current left panel top.
        /// </summary>
        public ViewsPanel LeftPanelTop
        {
            get => _leftPanelTop;
            set => Update(ref _leftPanelTop, value);
        }

        /// <summary>
        /// Gets or sets current left panel bottom.
        /// </summary>
        public ViewsPanel LeftPanelBottom
        {
            get => _leftPanelBottom;
            set => Update(ref _leftPanelBottom, value);
        }

        /// <summary>
        /// Gets or sets current right panel top.
        /// </summary>
        public ViewsPanel RightPanelTop
        {
            get => _rightPanelTop;
            set => Update(ref _rightPanelTop, value);
        }

        /// <summary>
        /// Gets or sets current right panel bottom.
        /// </summary>
        public ViewsPanel RightPanelBottom
        {
            get => _rightPanelBottom;
            set => Update(ref _rightPanelBottom, value);
        }

        /// <summary>
        /// Gets or sets current view.
        /// </summary>
        public IView CurrentView
        {
            get => _currentView;
            set => Update(ref _currentView, value);
        }

        /// <summary>
        /// Check whether the <see cref="CurrentView"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLeftPanelTop() => _leftPanelTop != null;

        /// <summary>
        /// Check whether the <see cref="CurrentView"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLeftPanelBottom() => _leftPanelBottom != null;

        /// <summary>
        /// Check whether the <see cref="CurrentView"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRightPanelTop() => _rightPanelTop != null;

        /// <summary>
        /// Check whether the <see cref="CurrentView"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeRightPanelBottom() => _rightPanelBottom != null;

        /// <summary>
        /// Check whether the <see cref="CurrentView"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentView() => _currentView != null;
    }
}
