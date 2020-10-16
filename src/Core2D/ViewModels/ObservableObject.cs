using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core2D
{
    /// <summary>
    /// Observable object.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged, ICopyable
    {
        private bool _isDirty;
        private ObservableObject _owner = null;
        private string _name = "";

        /// <summary>
        /// Gets or sets object owner.
        /// </summary>
        public virtual ObservableObject Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
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
        /// Gets is dirty flag.
        /// </summary>
        public virtual bool IsDirty()
        {
            return _isDirty;
        }

        /// <summary>
        /// Invalidates dirty flag.
        /// </summary>
        public virtual void Invalidate()
        {
            _isDirty = false;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Copies the object.
        /// </summary>
        /// <param name="shared">The shared objects dictionary.</param>
        /// <returns>The copy of the object.</returns>
        public abstract object Copy(IDictionary<object, object> shared);

        /// <summary>
        /// Notify observers about property changes.
        /// </summary>
        /// <param name="propertyName">The property name that changed.</param>
        public void Notify([CallerMemberName] string propertyName = default)
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
        public bool Update<T>(ref T field, T value, [CallerMemberName] string propertyName = default)
        {
            if (!Equals(field, value))
            {
                field = value;
                _isDirty = true;
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
        /// Check whether the <see cref="Name"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeName() => !string.IsNullOrWhiteSpace(_name);

        /// <summary>
        /// The <see cref="_isDirty"/> property is not serialized.
        /// </summary>
        /// <returns>Returns always false.</returns>
        public virtual bool ShouldSerializeIsDirty() => false;
    }
}
