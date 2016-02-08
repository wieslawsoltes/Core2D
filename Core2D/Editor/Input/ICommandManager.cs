// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Windows.Input;

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Defines command manager contract.
    /// </summary>
    public interface ICommandManager
    {
        /// <summary>
        /// Gets or sets registered core commands.
        /// </summary>
        IDictionary<string, ICoreCommand> Registered { get; set; }

        /// <summary>
        /// Register core command.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="command">The command instance.</param>
        void Register(string name, ICoreCommand command);

        /// <summary>
        /// Raises <see cref="ICommand.CanExecuteChanged"/> event for registered commands.
        /// </summary>
        void NotifyCanExecuteChanged();
    }
}
