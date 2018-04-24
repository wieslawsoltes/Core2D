// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Interfaces
{
    /// <summary>
    /// Executes code scripts.
    /// </summary>
    public interface IScriptRunner
    {
        /// <summary>
        /// Executes code script.
        /// </summary>
        /// <param name="code">The script code.</param>
        void Execute(string code);

        /// <summary>
        /// Executes code script and return current script state.
        /// </summary>
        /// <param name="code">The script code.</param>
        /// <param name="state">The script state to continue execution from a previous state.</param>
        /// <returns>The next script state.</returns>
        object Execute(string code, object state);
    }
}
