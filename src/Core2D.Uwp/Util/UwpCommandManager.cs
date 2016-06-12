// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Core2D.Editor.Input;
using System.Linq;
using System.Reflection;

namespace Core2D.Uwp
{
    /// <summary>
    /// Editor commands manager.
    /// </summary>
    public class UwpCommandManager : CommandManager
    {
        /// <inheritdoc/>
        public override void RegisterCommands()
        {
            Registered = typeof(Commands)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(ICoreCommand))
                .ToDictionary(p => p.Name, p => (ICoreCommand)p.GetValue(null));
        }
    }
}
