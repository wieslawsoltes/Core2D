// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;

namespace Dependencies
{
    /// <inheritdoc/>
    internal class JsonSerializationBinder : SerializationBinder
    {
        private readonly string _assemblyName;
        private readonly string _namespaceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializationBinder"/> class.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="namespaceName">The namespace name.</param>
        public JsonSerializationBinder(string assemblyName, string namespaceName)
        {
            _assemblyName = assemblyName;
            _namespaceName = namespaceName;
        }

        /// <inheritdoc/>
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name.Substring(0, serializedType.Name.Length);
        }

        /// <inheritdoc/>
        public override Type BindToType(string assemblyName, string typeName)
        {
            string restoredTypeName = string.Concat(_namespaceName, ".", typeName, ", ", _assemblyName);
            return Type.GetType(restoredTypeName, true);
        }
    }
}
