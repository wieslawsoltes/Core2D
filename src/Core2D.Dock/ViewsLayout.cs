// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D.Dock
{
    /// <summary>
    /// Views layout contract.
    /// </summary>
    public class ViewsLayout : ObservableObject, IViewsLayout
    {
        private ImmutableArray<IViewsPanel> _panels;
        private IView _currentView;

        /// <inheritdoc/>
        public ImmutableArray<IViewsPanel> Panels
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

        /// <summary>
        /// Check whether the <see cref="Panels"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeLeftPanels() => _panels.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="CurrentView"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentView() => _currentView != null;
    }
}
