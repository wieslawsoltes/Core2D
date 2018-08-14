// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D.Data
{
    /// <summary>
    /// Database column.
    /// </summary>
    public class Column : ObservableObject
    {
        private double _width;
        private bool _isVisible;
        private Database _owner;

        /// <summary>
        /// Gets or sets column display width.
        /// </summary>
        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether column is visible.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => Update(ref _isVisible, value);
        }

        /// <summary>
        /// Gets or sets column owner object.
        /// </summary>
        public Database Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="Column"/> instance.
        /// </summary>
        /// <param name="owner">The owner instance.</param>
        /// <param name="name">The column name.</param>
        /// <param name="width">The column width.</param>
        /// <param name="isVisible">The flag indicating whether column is visible.</param>
        /// <returns>The new instance of the <see cref="Column"/> class.</returns>
        public static Column Create(Database owner, string name, double width = double.NaN, bool isVisible = true)
        {
            return new Column()
            {
                Name = name,
                Width = width,
                IsVisible = isVisible,
                Owner = owner
            };
        }

        /// <summary>
        /// Check whether the <see cref="Width"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeWidth() => _width != default;

        /// <summary>
        /// Check whether the <see cref="IsVisible"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsVisible() => _isVisible != default;

        /// <summary>
        /// Check whether the <see cref="Owner"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOwner() => _owner != null;
    }
}
