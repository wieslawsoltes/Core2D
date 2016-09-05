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
    }
}
