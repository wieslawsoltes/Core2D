// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Helper class for <see cref="Tool.None"/> editor.
    /// </summary>
    public class ToolNone : ToolBase
    {
        private Editor _editor;

        /// <summary>
        /// Initialize new instance of <see cref="ToolNone"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ToolNone"/> object.</param>
        public ToolNone(Editor editor)
            : base()
        {
            _editor = editor;
        }
    }
}
