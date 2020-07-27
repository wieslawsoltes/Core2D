using System.Collections.Generic;

namespace Core2D.Data
{
    /// <summary>
    /// Database column.
    /// </summary>
    public class Column : ObservableObject, IColumn
    {
        private bool _isVisible;

        /// <inheritdoc/>
        public bool IsVisible
        {
            get => _isVisible;
            set => Update(ref _isVisible, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            return new Column()
            {
                Name = this.Name,
                IsVisible = this.IsVisible
            };
        }

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
        /// Check whether the <see cref="IsVisible"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsVisible() => _isVisible != default;
    }
}
