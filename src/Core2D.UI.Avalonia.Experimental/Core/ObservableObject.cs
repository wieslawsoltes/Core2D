// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core2D.Attributes;

namespace Core2D
{
    /// <summary>
    /// Observable object base class.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        private string _id = null;
        private string _name = "";

        /// <summary>
        /// Gets or sets observable object name.
        /// </summary>
        [Name]
        public virtual string Id
        {
            get => _id;
            set => Update(ref _id, value);
        }

        /// <summary>
        /// Gets or sets observable object name.
        /// </summary>
        public virtual string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }

        /// <summary>
        /// Gets or sets is dirty flag.
        /// </summary>
        internal bool IsDirty { get; set; }

        /// <summary>
        /// Set the <see cref="IsDirty"/> flag value.
        /// </summary>
        /// <param name="value">The new value of <see cref="IsDirty"/> flag.</param>
        public void MarkAsDirty(bool value) => IsDirty = value;

        /// <summary>
        /// Set unique string for <see cref="Id"/> using <see cref="Guid.NewGuid"/>
        /// </summary>
        public void SetUniqueId()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify observers about property changes.
        /// </summary>
        /// <param name="propertyName">The property name that changed.</param>
        public void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Update property backing field and notify observers about property change.
        /// </summary>
        /// <typeparam name="T">The type of field.</typeparam>
        /// <param name="field">The field to update.</param>
        /// <param name="value">The new field value.</param>
        /// <param name="propertyName">The property name that changed.</param>
        /// <returns>True if backing field value changed.</returns>
        public bool Update<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
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
        /// Check whether the <see cref="Id"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeId() => !String.IsNullOrWhiteSpace(_id);

        /// <summary>
        /// Check whether the <see cref="Name"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeName() => !String.IsNullOrWhiteSpace(_name);

        /// <summary>
        /// The <see cref="IsDirty"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeIsDirty() => false;
    }
}
