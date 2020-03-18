using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core2D
{
    /// <summary>
    /// Defines observable object contract.
    /// </summary>
    public interface IObservableObject : INotifyPropertyChanged, ICopyable
    {
        /// <summary>
        /// Gets or sets object owner.
        /// </summary>
        IObservableObject Owner { get; set; }

        /// <summary>
        /// Gets or sets observable object name.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets observable object name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Set the IsDirty flag value.
        /// </summary>
        /// <param name="value">The new value of IsDirty flag.</param>
        void MarkAsDirty(bool value);

        /// <summary>
        /// Notify observers about property changes.
        /// </summary>
        /// <param name="propertyName">The property name that changed.</param>
        void Notify([CallerMemberName] string propertyName = default);

        /// <summary>
        /// Update property backing field and notify observers about property change.
        /// </summary>
        /// <typeparam name="T">The type of field.</typeparam>
        /// <param name="field">The field to update.</param>
        /// <param name="value">The new field value.</param>
        /// <param name="propertyName">The property name that changed.</param>
        /// <returns>True if backing field value changed.</returns>
        bool Update<T>(ref T field, T value, [CallerMemberName] string propertyName = default);
    }
}
