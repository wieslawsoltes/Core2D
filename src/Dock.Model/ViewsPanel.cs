// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Dock.Model
{
    /// <summary>
    /// Views panel.
    /// </summary>
    public class ViewsPanel : ObservableObject, IGridPanel, IViewsPanel
    {
        private int _row;
        private int _column;
        private ImmutableArray<IView> _views;
        private IView _currentView;

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
        public ImmutableArray<IView> Views
        {
            get => _views;
            set => Update(ref _views, value);
        }

        /// <inheritdoc/>
        public IView CurrentView
        {
            get => _currentView;
            set => Update(ref _currentView, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewsPanel"/> class.
        /// </summary>
        public ViewsPanel()
        {
            _views = ImmutableArray<IView>.Empty;
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
        public virtual bool ShouldSerializeViews() => _views.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="CurrentView"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentView() => _currentView != null;
    }
}
