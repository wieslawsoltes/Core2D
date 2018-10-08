// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Editor.Tools
{
    public class MoveTool : ToolBase
    {
        public PathTool PathTool { get; set; }

        public override string Title => "Move";

        public MoveToolSettings Settings { get; set; }

        public MoveTool(PathTool pathTool)
        {
            PathTool = pathTool;
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            PathTool.Move();
        }
    }
}
