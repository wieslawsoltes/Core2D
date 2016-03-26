// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Perspex.Controls.Data;
using Core2D.Perspex.Controls.Path;
using Core2D.Perspex.Controls.Shapes;
using Core2D.Perspex.Controls.State;
using Core2D.Perspex.Controls.Style;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using Perspex;
using Perspex.Controls;
using Perspex.Controls.Presenters;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Core2D.Perspex.Controls.Presenters
{
    public class CachedContentPresenter : ContentPresenter
    {
        private static IDictionary<Type, Func<Control>> ControlFactory;

        private static IDictionary<Type, Control> ControlCache = new Dictionary<Type, Control>();

        static CachedContentPresenter()
        {
            ControlFactory = new Dictionary<Type, Func<Control>>();

            // Data
            ControlFactory.Add(typeof(ImmutableArray<XColumn>), () => new ColumnsControl());
            ControlFactory.Add(typeof(XDatabase), () => new DatabaseControl());
            ControlFactory.Add(typeof(XContext), () => new DataControl());
            ControlFactory.Add(typeof(ImmutableArray<XProperty>), () => new PropertiesControl());
            ControlFactory.Add(typeof(XRecord), () => new RecordControl());
            ControlFactory.Add(typeof(ImmutableArray<XRecord>), () => new RecordsControl());

            // Path
            ControlFactory.Add(typeof(XArcSegment), () => new ArcSegmentControl());
            ControlFactory.Add(typeof(XCubicBezierSegment), () => new CubicBezierSegmentControl());
            ControlFactory.Add(typeof(XLineSegment), () => new LineSegmentControl());
            ControlFactory.Add(typeof(XPathFigure), () => new PathFigureControl());
            ControlFactory.Add(typeof(XPathGeometry), () => new PathGeometryControl());
            ControlFactory.Add(typeof(XPathSize), () => new PathSizeControl());
            ControlFactory.Add(typeof(XPolyCubicBezierSegment), () => new PolyCubicBezierSegmentControl());
            ControlFactory.Add(typeof(XPolyLineSegment), () => new PolyLineSegmentControl());
            ControlFactory.Add(typeof(XPolyQuadraticBezierSegment), () => new PolyQuadraticBezierSegmentControl());
            ControlFactory.Add(typeof(XQuadraticBezierSegment), () => new QuadraticBezierSegmentControl());

            // Shapes
            ControlFactory.Add(typeof(XArc), () => new ArcControl());
            ControlFactory.Add(typeof(XCubicBezier), () => new CubicBezierControl());
            ControlFactory.Add(typeof(XEllipse), () => new EllipseControl());
            ControlFactory.Add(typeof(XGroup), () => new GroupControl());
            ControlFactory.Add(typeof(XImage), () => new ImageControl());
            ControlFactory.Add(typeof(XLine), () => new LineControl());
            ControlFactory.Add(typeof(XPath), () => new PathControl());
            ControlFactory.Add(typeof(XPoint), () => new PointControl());
            ControlFactory.Add(typeof(XQuadraticBezier), () => new QuadraticBezierControl());
            ControlFactory.Add(typeof(XRectangle), () => new RectangleControl());
            ControlFactory.Add(typeof(XText), () => new TextControl());

            // State
            ControlFactory.Add(typeof(ShapeState), () => new ShapeStateControl());

            // Style
            ControlFactory.Add(typeof(ArgbColor), () => new ArgbColorControl());
            ControlFactory.Add(typeof(ArrowStyle), () => new ArrowStyleControl());
            ControlFactory.Add(typeof(FontStyle), () => new FontStyleControl());
            ControlFactory.Add(typeof(LineFixedLength), () => new LineFixedLengthControl());
            ControlFactory.Add(typeof(LineStyle), () => new LineStyleControl());
            ControlFactory.Add(typeof(ShapeStyle), () => new ShapeStyleControl());
            ControlFactory.Add(typeof(BaseStyle), () => new StyleControl());
            ControlFactory.Add(typeof(TextStyle), () => new TextStyleControl());
        }

        public CachedContentPresenter()
        {
            this.GetObservable(DataContextProperty).Subscribe((value) =>
            {
                Debug.Print($"DataContext Changed: {value} for {GetHashCode()}");
                SetContent(value);
            });
        }

        private void SetContent(object value)
        {
            Control control = null;
            object target = value;
            if (target != null)
            {
                control = GetControl(target.GetType());
            }
            this.Content = control;
        }

        private Control CreateControl(Type type)
        {
            Func<Control> createInstance;
            ControlFactory.TryGetValue(type, out createInstance);
            if (createInstance != null)
            {
                var sw = Stopwatch.StartNew();
                var instance = createInstance();
                sw.Stop();
                Debug.Print($"CreateInstance: {type} in {sw.Elapsed.TotalMilliseconds}ms.");
                return instance;
            }
            else
            {
                Debug.Print($"Not Registered: {type}");
            }
            return null;
        }

        private Control GetControl(Type type)
        {
            Control control;
            ControlCache.TryGetValue(type, out control);
            if (control == null)
            {
                control = CreateControl(type);
                if (control != null)
                {
                    Debug.Print($"New: {type} -> {control}");
                    ControlCache.Add(type, control);
                }
            }
            else
            {
                Debug.Print($"Cached: {type} -> {control}");
            }
            return control;
        }
    }
}
