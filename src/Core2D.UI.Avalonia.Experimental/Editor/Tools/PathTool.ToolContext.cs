// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Containers;
using Core2D.Editor.Bounds;
using Core2D.Shapes;
using Core2D.Style;
using Core2D.Renderer;
using Core2D.Shape;

namespace Core2D.Editor.Tools
{
    public partial class PathTool : IToolContext
    {
        private PointShape _nextPoint;
        private IToolContext _context;

        private void SetContext(IToolContext context)
        {
            _context = context;
        }

        private void SetRenderer(ShapeRenderer renderer)
        {
            if (_context != null)
            {
                _context.Renderer = renderer;
            }
        }

        private void SetNextPoint(PointShape point)
        {
            _nextPoint = point;
        }

        public ShapeRenderer Renderer
        {
            get => _context?.Renderer;
            set => SetRenderer(value);
        }

        public IHitTest HitTest
        {
            get => _context?.HitTest;
            set => throw new InvalidOperationException($"Can not set {HitTest} property value.");
        }

        public ILayerContainer CurrentContainer
        {
            get => _figure;
            set => throw new InvalidOperationException($"Can not set {CurrentContainer} property value.");
        }

        public ILayerContainer WorkingContainer
        {
            get => _figure;
            set => throw new InvalidOperationException($"Can not set {WorkingContainer} property value.");
        }

        public ShapeStyle CurrentStyle
        {
            get => _context?.CurrentStyle;
            set => throw new InvalidOperationException($"Can not set {CurrentStyle} property value.");
        }

        public BaseShape PointShape
        {
            get => _context?.PointShape;
            set => throw new InvalidOperationException($"Can not set {PointShape} property value.");
        }

        public Action Capture
        {
            get => _context?.Capture;
            set => throw new InvalidOperationException($"Can not set {Capture} property value.");
        }

        public Action Release
        {
            get => _context?.Release;
            set => throw new InvalidOperationException($"Can not set {Release} property value.");
        }

        public Action Invalidate
        {
            get => _context?.Invalidate;
            set => throw new InvalidOperationException($"Can not set {Invalidate} property value.");
        }

        public PointShape GetNextPoint(double x, double y, bool connect, double radius)
        {
            return _nextPoint ?? _context?.GetNextPoint(x, y, connect, radius);
        }
    }
}
