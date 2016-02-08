// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Base commands manager.
    /// </summary>
    public class CommandManager : ICommandManager
    {
        /// <inheritdoc/>
        public IDictionary<string, ICoreCommand> Registered { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManager"/> class.
        /// </summary>
        public CommandManager()
        {
            Registered = new Dictionary<string, ICoreCommand>();
        }

        /// <inheritdoc/>
        public void Register(string name, ICoreCommand command)
        {
            if (!Registered.ContainsKey(name))
            {
                Registered.Add(name, command);
            }
        }

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged()
        {
            if (Registered != null)
            {
                foreach (var command in Registered)
                {
                    command.Value.NotifyCanExecuteChanged();
                }
            }
        }
    }
}
