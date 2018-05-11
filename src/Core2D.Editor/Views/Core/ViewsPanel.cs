// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D.Editor.Views.Core
{
    /// <summary>
    /// Views panel.
    /// </summary>
    public class ViewsPanel : ObservableObject
    {
        private ImmutableArray<IView> _views;
        private IView _currentView;

        /// <summary>
        /// Gets or sets views.
        /// </summary>
        public ImmutableArray<IView> Views
        {
            get => _views;
            set => Update(ref _views, value);
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
        /// Check whether the <see cref="Views"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeViews() => _views.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="CurrentView"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentView() => _currentView != null;
    }
}
