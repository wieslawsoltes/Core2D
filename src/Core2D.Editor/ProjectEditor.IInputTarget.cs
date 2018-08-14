// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Editor.Input;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor input target implementation.
    /// </summary>
    public partial class ProjectEditor : IInputTarget
    {
        /// <inheritdoc/>
        public void LeftDown(InputArgs args) => CurrentTool.LeftDown(args);

        /// <inheritdoc/>
        public void LeftUp(InputArgs args) => CurrentTool.LeftUp(args);

        /// <inheritdoc/>
        public void RightDown(InputArgs args) => CurrentTool.RightDown(args);

        /// <inheritdoc/>
        public void RightUp(InputArgs args) => CurrentTool.RightUp(args);

        /// <inheritdoc/>
        public void Move(InputArgs args) => CurrentTool.Move(args);

        /// <inheritdoc/>
        public bool IsLeftDownAvailable()
        {
            return _project?.CurrentContainer?.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project?.CurrentStyleLibrary?.Selected != null;
        }

        /// <inheritdoc/>
        public bool IsLeftUpAvailable()
        {
            return _project?.CurrentContainer?.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project?.CurrentStyleLibrary?.Selected != null;
        }

        /// <inheritdoc/>
        public bool IsRightDownAvailable()
        {
            return _project?.CurrentContainer?.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project?.CurrentStyleLibrary?.Selected != null;
        }

        /// <inheritdoc/>
        public bool IsRightUpAvailable()
        {
            return _project?.CurrentContainer?.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project?.CurrentStyleLibrary?.Selected != null;
        }

        /// <inheritdoc/>
        public bool IsMoveAvailable()
        {
            return _project?.CurrentContainer?.CurrentLayer != null
                && _project.CurrentContainer.CurrentLayer.IsVisible
                && _project?.CurrentStyleLibrary?.Selected != null;
        }

        /// <inheritdoc/>
        public bool IsSelectionAvailable()
        {
            return Renderers?[0]?.State?.SelectedShape != null
                || Renderers?[0]?.State?.SelectedShapes != null;
        }
    }
}
