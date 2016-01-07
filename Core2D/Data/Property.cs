// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    [ContentProperty(nameof(Value))]
    public class Property : ObservableObject
    {
        private string _name;
        private object _value;
        private Data _owner;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { Update(ref _value, value); }
        }

        /// <summary>
        /// Gets or sets property owner object.
        /// </summary>
        public Data Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }

        /// <summary>
        /// Creates a new <see cref="Property"/> instance.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Property Create(Data owner, string name, object value)
        {
            return new Property()
            {
                Name = name,
                Value = value,
                Owner = owner
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
