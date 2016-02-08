// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using System.Reflection;

namespace Core2D.Perspex
{
    /// <summary>
    /// Editor commands manager.
    /// </summary>
    public class EditorCommandManager : CommandManager
    {
        /// <summary>
        /// Register editor commands.
        /// </summary>
        public void RegisterCommands()
        {
            Registered = typeof(Commands)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(ICoreCommand))
                .ToDictionary(p => p.Name, p => (ICoreCommand)p.GetValue(null));
        }
    }
}
