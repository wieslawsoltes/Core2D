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

        private void ArcLeftDown(double x, double y)
        {
            // TODO: Add Arc path helper LeftDown method implementation.
        }

        private void ArcRightDown(double x, double y)
        {
            // TODO: Add Arc path helper RightDown method implementation.
        }

        private void ArcMove(double x, double y)
        {
            // TODO: Add Arc path helper Move method implementation.
        }

        private void ToStateOneArc()
        {
            // TODO: Add Arc path helper ToStateOne method implementation.
        }

        private void ToStateTwoArc()
        {
            // TODO: Add Arc path helper ToStateTwo method implementation.
        }

        private void ToStateThreeArc()
        {
            // TODO: Add Arc path helper ToStateThree method implementation.
        }

        private void MoveArcHelpers()
        {
            // TODO: Add Arc path helper Move method implementation.
        }

        private void RemoveArcHelpers()
        {
            // TODO: Add Arc path helper Remove method implementation.
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);
            ArcLeftDown(x, y);
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);
            ArcRightDown(x, y);
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);
            ArcMove(x, y);
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();
            ToStateOneArc();
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();
            ToStateTwoArc();
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();
            ToStateThreeArc();
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);
            MoveArcHelpers();
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();
            RemoveArcHelpers();
        }
    }
}
