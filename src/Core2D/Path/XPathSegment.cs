// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// <see cref="XPathFigure"/> segment base class.
    /// </summary>
    public abstract class XPathSegment : ObservableObject
    {
        private bool _isStroked;
        private bool _isSmoothJoin;

        /// <summary>
        /// Gets or sets flag indicating whether segment is stroked.
        /// </summary>
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether segment is smooth join.
        /// </summary>
        public bool IsSmoothJoin
        {
            get => _isSmoothJoin;
            set => Update(ref _isSmoothJoin, value);
        }

        /// <summary>
        /// Get all points in the segment.
        /// </summary>
        /// <returns>All points in the segment.</returns>
        public abstract IEnumerable<XPoint> GetPoints();

        /// <summary>
        /// Check whether the <see cref="IsStroked"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsStroked() => _isStroked != default(bool);

        /// <summary>
        /// Check whether the <see cref="IsSmoothJoin"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsSmoothJoin() => _isSmoothJoin != default(bool);
    }
}
