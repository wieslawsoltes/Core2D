// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;	
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
        private string _name = null;
        private ImmutableArray<ObservableObject> _resources = ImmutableArray.Create<ObservableObject>();

        /// <summary>
        /// Gets or sets resource name.
        /// </summary>
        [Name]
        public string Id
        {
            get => _id;
            set => Update(ref _id, value);
        }

        /// <summary>
        /// Gets or sets resource name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }

        /// <summary>
        /// Gets or sets shape resources.
        /// </summary>
        public ImmutableArray<ObservableObject> Resources
        {
            get => _resources;
            set => Update(ref _resources, value);
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
                Notify(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether the <see cref="Id"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeId() => !String.IsNullOrWhiteSpace(_id);

        /// <summary>
        /// Check whether the <see cref="Name"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeName() => !String.IsNullOrWhiteSpace(_name);

        /// <summary>
        /// Check whether the <see cref="Resources"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public bool ShouldSerializeResources() => _resources.IsEmpty == false;
    }
}
