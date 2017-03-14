// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Attributes;

namespace Core2D.Data.Database
{
    /// <summary>
    /// Database column.
    /// </summary>
    public class XColumn : ObservableObject
    {
        private Guid _id;
        private string _name;
        private double _width;
        private bool _isVisible;
        private XDatabase _owner;

        /// <summary>
        /// Gets or sets column Id.
        /// </summary>
        public Guid Id
        {
            get => _id;
            set => Update(ref _id, value);
        }

        /// <summary>
        /// Gets or sets column name.
        /// </summary>
        [Content]
        public string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }

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
        public XDatabase Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <summary>
        /// Creates a new <see cref="XColumn"/> instance.
        /// </summary>
        /// <param name="owner">The owner instance.</param>
        /// <param name="name">The column name.</param>
        /// <param name="width">The column width.</param>
        /// <param name="isVisible">The flag indicating whether column is visible.</param>
        /// <returns>The new instance of the <see cref="XColumn"/> class.</returns>
        public static XColumn Create(XDatabase owner, string name, double width = double.NaN, bool isVisible = true)
        {
            return new XColumn()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Width = width,
                IsVisible = isVisible,
                Owner = owner
            };
        }
    }
}
