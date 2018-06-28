// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// <see cref="PathFigure"/> segment base class.
    /// </summary>
    public abstract class PathSegment : ObservableObject, ICopyable
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
        public abstract IEnumerable<PointShape> GetPoints();

        /// <inheritdoc/>
        public virtual object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="IsStroked"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsStroked() => _isStroked != default;

        /// <summary>
        /// Check whether the <see cref="IsSmoothJoin"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsSmoothJoin() => _isSmoothJoin != default;
    }
}
