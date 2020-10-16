using System.Collections.Generic;

namespace Core2D.Data
{
    /// <summary>
    /// Record value.
    /// </summary>
    public class Value : ObservableObject
    {
        private string _content;

        /// <inheritdoc/>
        public string Content
        {
            get => _content;
            set => RaiseAndSetIfChanged(ref _content, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            return new Value()
            {
                Name = Name,
                Content = Content
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
        /// Check whether the <see cref="Content"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeContent() => !string.IsNullOrWhiteSpace(_content);
    }
}
