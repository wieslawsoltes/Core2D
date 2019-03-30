// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core2D.Attributes;

namespace Core2D
{
    /// <summary>
    /// Observable object.
    /// </summary>
    public abstract class ObservableObject : IObservableObject
    {
        private IObservableObject _owner = null;
        private string _id = Guid.NewGuid().ToString();
        private string _name = "";

        /// <inheritdoc/>
        public virtual IObservableObject Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <inheritdoc/>
        [Name]
        public virtual string Id
        {
            get => _id;
            set => Update(ref _id, value);
        }

        /// <inheritdoc/>
        public virtual string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }

        /// <summary>
        /// Gets or sets is dirty flag.
        /// </summary>
        internal bool IsDirty { get; set; }

        /// <inheritdoc/>
        public void MarkAsDirty(bool value) => IsDirty = value;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public abstract object Copy(IDictionary<object, object> shared);

        /// <inheritdoc/>
        public void Notify([CallerMemberName] string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <inheritdoc/>
        public bool Update<T>(ref T field, T value, [CallerMemberName] string propertyName = default)
        {
            if (!Equals(field, value))
            {
                field = value;
                IsDirty = true;
                Notify(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether the <see cref="Owner"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOwner() => _owner != null;

        /// <summary>
        /// Check whether the <see cref="Id"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeId() => !string.IsNullOrWhiteSpace(_id);

        /// <summary>
        /// Check whether the <see cref="Name"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeName() => !string.IsNullOrWhiteSpace(_name);

        /// <summary>
        /// The <see cref="IsDirty"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeIsDirty() => false;
    }
}
