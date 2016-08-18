// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Core2D.Editor.Tools.Path.Shapes;
using Core2D.Editor.Tools.Selection;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shape;
using Core2D.Shapes;
using static System.Math;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Helper class for <see cref="PathTool.Move"/> editor.
    /// </summary>
    public class PathToolMove : ToolBase
    {        
        /// <inheritdoc/>
        public override string Name => "Move";
    }
}
