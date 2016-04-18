// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Base class for core command manager.
    /// </summary>
    public class CommandManager
    {
        /// <summary>
        /// Gets or sets registered core commands.
        /// </summary>
        public IDictionary<string, ICoreCommand> Registered { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManager"/> class.
        /// </summary>
        public CommandManager()
        {
            Registered = new Dictionary<string, ICoreCommand>();
        }

        /// <summary>
        /// Register core command.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="command">The command instance.</param>
        public virtual void Register(string name, ICoreCommand command)
        {
            if (!Registered.ContainsKey(name))
            {
                Registered.Add(name, command);
            }
        }

        /// <summary>
        /// Raises <see cref="ICommand.CanExecuteChanged"/> event for registered commands.
        /// </summary>
        public virtual void NotifyCanExecuteChanged()
        {
            if (Registered != null)
            {
                foreach (var command in Registered)
                {
                    command.Value.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Register editor commands.
        /// </summary>
        public virtual void RegisterCommands()
        {
            Registered = typeof(Commands)
                .GetRuntimeProperties()
                .Where(p =>
                {
                    return (p.PropertyType == typeof(ICoreCommand)
                        && p.GetMethod != null
                        && p.SetMethod != null
                        && p.SetMethod.IsStatic
                        && p.GetMethod.IsStatic
                        && p.GetMethod.IsPublic
                        && p.SetMethod.IsPublic);
                })
                .ToDictionary(p => p.Name, p => (ICoreCommand)p.GetValue(null));
        }
    }
}
