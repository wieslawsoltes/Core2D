// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        RendererState State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isZooming"></param>
        void ClearCache(bool isZooming);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="container"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, Container container, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="layer"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, Layer layer, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="line"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, XLine line, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="rectangle"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="ellipse"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, XEllipse ellipse, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="bezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, XBezier bezier, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="qbezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, XQBezier qbezier, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="text"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, XText text, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="image"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, XImage image, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="path"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        void Draw(object dc, XPath path, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);
    }
}
