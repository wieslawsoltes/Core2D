// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// Data property.
    /// </summary>
    [ContentProperty(nameof(Value))]
    public class Property : ObservableObject
    {
        private string _name;
        private string _value;
        private Data _owner;

        /// <summary>
        /// Gets or sets property name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets property value.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { Update(ref _value, value); }
        }

        /// <summary>
        /// Gets or sets property owner.
        /// </summary>
        public Data Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }

        /// <summary>
        /// Creates a new <see cref="Property"/> instance.
        /// </summary>
        /// <param name="owner">The property owner.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns>The new instance of the <see cref="Property"/> class.</returns>
        public static Property Create(Data owner, string name, string value)
        {
            return new Property()
            {
                Name = name,
                Value = value,
                Owner = owner
            };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
