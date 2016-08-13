// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Renderer.Vdx
{
    /// <summary>
    /// Native Visio VDX shape renderer.
    /// </summary>
    public partial class VdxRenderer : ShapeRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VdxRenderer"/> class.
        /// </summary>
        public VdxRenderer()
        {
            ClearCache(isZooming: false);
        }

        /// <summary>
        /// Creates a new <see cref="VdxRenderer"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="VdxRenderer"/> class.</returns>
        public static VdxRenderer Create() => new VdxRenderer();

        /// <inheritdoc/>
        public override void Fill(object dc, double x, double y, double width, double height, ArgbColor color)
        {
            // TODO: Implement Fill.
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XLine line, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            // TODO: Implement Draw line.
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            // TODO: Implement Draw rectangle.
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XEllipse ellipse, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            // TODO: Implement Draw ellipse.
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            // TODO: Implement Draw arc.
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XCubicBezier cubicBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            // TODO: Implement Draw cubic bezier.
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XQuadraticBezier quadraticBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            // TODO: Implement Draw quadratic bezier.
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XText text, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            // TODO: Implement Draw text.
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XImage image, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            // TODO: Implement Draw image.
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XPath path, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            // TODO: Implement Draw path.
        }
    }
}
