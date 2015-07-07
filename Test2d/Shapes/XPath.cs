// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XPath : BaseShape
    {
        private string _source;
        private XPathGeometry _geometry;
        private ShapeTransform _transform;

        /// <summary>
        /// Gets or sets path source markup used to draw shape.
        /// Source markup syntax: 
        /// https://msdn.microsoft.com/en-us/library/ms752293(v=vs.110).aspx
        /// </summary>
        public string Source
        {
            get { return _source; }
            set { Update(ref _source, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public XPathGeometry Geometry
        {
            get { return _geometry; }
            set { Update(ref _geometry, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeTransform Transform
        {
            get { return _transform; }
            set { Update(ref _transform, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void Bind(Record r)
        {
            // TODO: Implement Bind() method.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="renderer"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var record = r ?? this.Record;

            if (State.HasFlag(ShapeState.Visible))
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public override void Move(double dx, double dy)
        {
            _transform.OffsetX += dx;
            _transform.OffsetY += dy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="style"></param>
        /// <param name="source"></param>
        /// <param name="geometry"></param>
        /// <param name="transform"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <returns></returns>
        public static XPath Create(
            string name,
            ShapeStyle style,
            string source,
            XPathGeometry geometry,
            ShapeTransform transform,
            bool isStroked = true,
            bool isFilled = true)
        {
            return new XPath()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Bindings = ImmutableArray.Create<ShapeBinding>(),
                Properties = ImmutableArray.Create<ShapeProperty>(),
                Code = ShapeCode.Create(),
                Source = source,
                Geometry = geometry,
                Transform = transform
            };
        }
    }
}
