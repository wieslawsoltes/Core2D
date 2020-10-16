using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// <see cref="PathFigure"/> segment base class.
    /// </summary>
    public abstract class PathSegment : ObservableObject
    {
        private bool _isStroked;

        /// <inheritdoc/>
        public bool IsStroked
        {
            get => _isStroked;
            set => RaiseAndSetIfChanged(ref _isStroked, value);
        }

        /// <inheritdoc/>
        public abstract void GetPoints(IList<PointShape> points);

        /// <inheritdoc/>
        public abstract string ToXamlString();

        /// <inheritdoc/>
        public abstract string ToSvgString();

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();
        }

        /// <summary>
        /// Check whether the <see cref="IsStroked"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsStroked() => _isStroked != default;
    }
}
