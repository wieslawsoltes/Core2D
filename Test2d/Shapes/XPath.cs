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
        private bool _isStroked;
        private bool _isFilled;

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
        public bool IsStroked
        {
            get { return _isStroked; }
            set { Update(ref _isStroked, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFilled
        {
            get { return _isFilled; }
            set { Update(ref _isFilled, value); }
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
                Bindings = ImmutableArray.Create<ShapeBinding>(),
                Properties = ImmutableArray.Create<ShapeProperty>(),
                Source = source,
                Geometry = geometry,
                Transform = transform,
                IsStroked = isStroked,
                IsFilled = isFilled
            };
        }
    }
}
