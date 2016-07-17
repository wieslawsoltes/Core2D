// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Helper class for <see cref="PathTool.Arc"/> editor.
    /// </summary>
    internal class ToolPathArc : ToolBase
    {
        private ProjectEditor _editor;
        private ToolPath _toolPath;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPathArc"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        /// <param name="toolPath">The current <see cref="ToolPath"/> object.</param>
        public ToolPathArc(ProjectEditor editor, ToolPath toolPath)
            : base()
        {
            _editor = editor;
            _toolPath = toolPath;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            // TODO: Add Arc path helper LeftDown method implementation.
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);

            // TODO: Add Arc path helper RightDown method implementation.
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            // TODO: Add Arc path helper Move method implementation.
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            // TODO: Add Arc path helper ToStateOne method implementation.
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            // TODO: Add Arc path helper ToStateTwo method implementation.
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();

            // TODO: Add Arc path helper ToStateThree method implementation.
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            // TODO: Add Arc path helper Move method implementation.
        }

        /// <inheritdoc/>
        public override void Finalize(BaseShape shape)
        {
            base.Finalize(shape);

            // TODO: Add Arc path helper Finalize method implementation.
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            // TODO: Add Arc path helper Remove method implementation.
        }
    }
}
