// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;

namespace Core2D
{
    /// <summary>
    /// Defines bindable contract.
    /// </summary>
    public interface IBindable
    {
        /// <summary>
        /// Binds data using current <see cref="IDataFlow"/>.
        /// </summary>
        /// <param name="dataFlow">The generic data flow object used to bind data.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The database record.</param>
        void Bind(IDataFlow dataFlow, object db, object r);

        /// <summary>
        /// Sets property value.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        void SetProperty(string name, object value);

        /// <summary>
        /// Gets property value.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The property value.</returns>
        object GetProperty(string name);
    }
}
