// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Core2D
{
    /// <summary>
    /// Object representing path shape.
    /// </summary>
    public class XPath : BaseShape
    {
        private XPathGeometry _geometry;

        /// <summary>
        /// Gets or sets path geometry used to draw shape.
        /// </summary>
        /// <remarks>
        /// Path geometry is based on path markup syntax:
        /// - Xaml abbreviated geometry https://msdn.microsoft.com/en-us/library/ms752293(v=vs.110).aspx
        /// - Svg path specification http://www.w3.org/TR/SVG11/paths.html
        /// </remarks>
        public XPathGeometry Geometry
        {
            get { return _geometry; }
            set { Update(ref _geometry, value); }
        }

        /// <summary>
        /// Gets all points from <see cref="XPathGeometry.Figures"/>. 
        /// </summary>
        /// <returns>The <see cref="XPoint"/> array.</returns>
        public ImmutableArray<XPoint> GetAllPoints()
        {
            var builder = ImmutableArray.CreateBuilder<XPoint>();

            if (this.Geometry != null)
            {
                foreach (var figure in this.Geometry.Figures)
                {
                    builder.Add(figure.StartPoint);

                    foreach (var segment in figure.Segments)
                    {
                        if (segment is XArcSegment)
                        {
                            var arcSegment = segment as XArcSegment;
                            builder.Add(arcSegment.Point);
                        }
                        else if (segment is XBezierSegment)
                        {
                            var bezierSegment = segment as XBezierSegment;
                            builder.Add(bezierSegment.Point1);
                            builder.Add(bezierSegment.Point2);
                            builder.Add(bezierSegment.Point3);
                        }
                        else if (segment is XLineSegment)
                        {
                            var lineSegment = segment as XLineSegment;
                            builder.Add(lineSegment.Point);
                        }
                        else if (segment is XPolyBezierSegment)
                        {
                            var polyBezierSegment = segment as XPolyBezierSegment;
                            foreach (var point in polyBezierSegment.Points)
                            {
                                builder.Add(point);
                            }
                        }
                        else if (segment is XPolyLineSegment)
                        {
                            var polyLineSegment = segment as XPolyLineSegment;
                            foreach (var point in polyLineSegment.Points)
                            {
                                builder.Add(point);
                            }
                        }
                        else if (segment is XPolyQuadraticBezierSegment)
                        {
                            var polyQuadraticSegment = segment as XPolyQuadraticBezierSegment;
                            foreach (var point in polyQuadraticSegment.Points)
                            {
                                builder.Add(point);
                            }
                        }
                        else if (segment is XQuadraticBezierSegment)
                        {
                            var qbezierSegment = segment as XQuadraticBezierSegment;
                            builder.Add(qbezierSegment.Point1);
                            builder.Add(qbezierSegment.Point2);
                        }
                    }
                }
            }

            return builder.ToImmutable();
        }
        
        /// <inheritdoc/>
        public override void Bind(Record r)
        {
            // TODO: Implement Bind() method.
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<Property> db, Record r)
        {
            var record = r ?? this.Data.Record;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db, record);
            }

            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    var points = this.GetAllPoints();
                    foreach (var point in points)
                    {
                        point.Draw(dc, renderer, dx, dy, db, record);
                    }
                }
                else
                {
                    var points = this.GetAllPoints();
                    foreach (var point in points)
                    {
                        if (point == renderer.State.SelectedShape)
                        {
                            point.Draw(dc, renderer, dx, dy, db, record);
                        }
                    }
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    var points = this.GetAllPoints();
                    foreach (var point in points)
                    {
                        point.Draw(dc, renderer, dx, dy, db, record);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Move(double dx, double dy)
        {
            var points = this.GetAllPoints();
            foreach (var point in points)
            {
                point.Move(dx, dy);
            }
        }

        /// <summary>
        /// Creates a new <see cref="XPath"/> instance.
        /// </summary>
        /// <param name="name">The shape name.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="geometry">The path geometry.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <returns>The new instance of the <see cref="XPath"/> class.</returns>
        public static XPath Create(
            string name,
            ShapeStyle style,
            XPathGeometry geometry,
            bool isStroked = true,
            bool isFilled = true)
        {
            return new XPath()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Data = new Data()
                {
                    Bindings = ImmutableArray.Create<Binding>(),
                    Properties = ImmutableArray.Create<Property>()
                },
                Geometry = geometry
            };
        }
    }
}
