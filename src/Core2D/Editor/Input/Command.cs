// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Editor.Input
{
    /// <summary>
    /// Parameterless input command.
    /// </summary>
    public abstract class Command : Command<object>
    {
        /// <inheritdoc/>
        public override bool CanRun(object parameter)
        {
            return this.CanRun();
        }

        /// <inheritdoc/>
        public override void Run(object parameter)
        {
            this.Run();
        }

        /// <summary>
        /// Check if can invoke execute action.
        /// </summary>
        /// <returns>True if can invoke execute action.</returns>
        public abstract bool CanRun();

        /// <summary>
        /// Invoke execute action.
        /// </summary>
        public abstract void Run();
    }
}
