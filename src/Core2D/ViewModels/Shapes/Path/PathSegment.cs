using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// <see cref="PathFigure"/> segment base class.
    /// </summary>
    public abstract class PathSegment : ObservableObject, IPathSegment
    {
        private bool _isStroked;
        private bool _isSmoothJoin;

        /// <inheritdoc/>
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        /// <inheritdoc/>
        public bool IsSmoothJoin
        {
            get => _isSmoothJoin;
            set => Update(ref _isSmoothJoin, value);
        }

        /// <inheritdoc/>
        public abstract IEnumerable<IPointShape> GetPoints();

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
