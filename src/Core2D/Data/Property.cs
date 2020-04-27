using System.Collections.Generic;

namespace Core2D.Data
{
    /// <summary>
    /// Data property.
    /// </summary>
    public class Property : ObservableObject, IProperty
    {
        private string _value;

        /// <inheritdoc/>
        public string Value
        {
            get => _value;
            set => Update(ref _value, value);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            return new Property()
            {
                Name = Name,
                Value = Value
            };
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => _value.ToString();

        /// <summary>
        /// Check whether the <see cref="Value"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeValue() => !string.IsNullOrWhiteSpace(_value);
    }
}
