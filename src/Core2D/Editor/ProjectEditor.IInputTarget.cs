// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Input;

namespace Core2D.Editor
{
    /// <summary>
    /// Project editor input target implementation.
    /// </summary>
    public partial class ProjectEditor : IInputTarget
    {
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

        /// <inheritdoc/>
        public void LeftDown(double x, double y)
        {
            CurrentTool.LeftDown(x, y);
        }

        /// <inheritdoc/>
        public void LeftUp(double x, double y)
        {
            CurrentTool.LeftUp(x, y);
        }

        /// <inheritdoc/>
        public void RightDown(double x, double y)
        {
            CurrentTool.RightDown(x, y);
        }

        /// <inheritdoc/>
        public void RightUp(double x, double y)
        {
            CurrentTool.RightUp(x, y);
        }

        /// <inheritdoc/>
        public void Move(double x, double y)
        {
            CurrentTool.Move(x, y);
        }
    }
}
