// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Editor.Views.Interfaces;

namespace Core2D.Editor
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
    }
}
