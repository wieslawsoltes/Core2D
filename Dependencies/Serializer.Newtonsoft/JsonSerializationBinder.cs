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
        private readonly string _namespacePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializationBinder"/> class.
        /// </summary>
        /// <param name="assemblyName">The class library assembly name.</param>
        /// <param name="namespacePrefix">The class library namespace prefix.</param>
        public JsonSerializationBinder(string assemblyName, string namespacePrefix)
        {
            _assemblyName = assemblyName;
            _namespacePrefix = namespacePrefix;
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
            string restoredTypeName = string.Concat(_namespacePrefix, ".", typeName, ", ", _assemblyName);
            return Type.GetType(restoredTypeName, true);
        }
    }
}
