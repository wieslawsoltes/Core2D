// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Editor.Views;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Perspex.Controls.Data;
using Core2D.Perspex.Controls.Path;
using Core2D.Perspex.Controls.Project;
using Core2D.Perspex.Controls.Shapes;
using Core2D.Perspex.Controls.State;
using Core2D.Perspex.Controls.Style;
using Core2D.Perspex.Views;
using Core2D.Project;
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

namespace Core2D.Perspex.Presenters
{
    public class CachedContentPresenter : ContentPresenter
    {
        private static IDictionary<Type, Func<Control>> Factory;
        private IDictionary<Type, Control> Cache;

        static CachedContentPresenter()
        {
            Factory = new Dictionary<Type, Func<Control>>();

            // Views
            Factory.Add(typeof(DashboardView), () => new DashboardViewControl());
            Factory.Add(typeof(EditorView), () => new EditorViewControl());

            // Project
            Factory.Add(typeof(XOptions), () => new OptionsControl());
            Factory.Add(typeof(XTemplate), () => new TemplateControl());
            Factory.Add(typeof(XPage), () => new PageControl());

            // Data
            Factory.Add(typeof(ImmutableArray<XColumn>), () => new ColumnsControl());
            Factory.Add(typeof(XDatabase), () => new DatabaseControl());
            Factory.Add(typeof(XContext), () => new DataControl());
            Factory.Add(typeof(ImmutableArray<XProperty>), () => new PropertiesControl());
            Factory.Add(typeof(XRecord), () => new RecordControl());
            Factory.Add(typeof(ImmutableArray<XRecord>), () => new RecordsControl());

            // Path
            Factory.Add(typeof(XArcSegment), () => new ArcSegmentControl());
            Factory.Add(typeof(XCubicBezierSegment), () => new CubicBezierSegmentControl());
            Factory.Add(typeof(XLineSegment), () => new LineSegmentControl());
            Factory.Add(typeof(XPathFigure), () => new PathFigureControl());
            Factory.Add(typeof(XPathGeometry), () => new PathGeometryControl());
            Factory.Add(typeof(XPathSize), () => new PathSizeControl());
            Factory.Add(typeof(XPolyCubicBezierSegment), () => new PolyCubicBezierSegmentControl());
            Factory.Add(typeof(XPolyLineSegment), () => new PolyLineSegmentControl());
            Factory.Add(typeof(XPolyQuadraticBezierSegment), () => new PolyQuadraticBezierSegmentControl());
            Factory.Add(typeof(XQuadraticBezierSegment), () => new QuadraticBezierSegmentControl());

            // Shapes
            Factory.Add(typeof(XArc), () => new ArcControl());
            Factory.Add(typeof(XCubicBezier), () => new CubicBezierControl());
            Factory.Add(typeof(XEllipse), () => new EllipseControl());
            Factory.Add(typeof(XGroup), () => new GroupControl());
            Factory.Add(typeof(XImage), () => new ImageControl());
            Factory.Add(typeof(XLine), () => new LineControl());
            Factory.Add(typeof(XPath), () => new PathControl());
            Factory.Add(typeof(XPoint), () => new PointControl());
            Factory.Add(typeof(XQuadraticBezier), () => new QuadraticBezierControl());
            Factory.Add(typeof(XRectangle), () => new RectangleControl());
            Factory.Add(typeof(XText), () => new TextControl());

            // State
            Factory.Add(typeof(ShapeState), () => new ShapeStateControl());

            // Style
            Factory.Add(typeof(ArgbColor), () => new ArgbColorControl());
            Factory.Add(typeof(ArrowStyle), () => new ArrowStyleControl());
            Factory.Add(typeof(FontStyle), () => new FontStyleControl());
            Factory.Add(typeof(LineFixedLength), () => new LineFixedLengthControl());
            Factory.Add(typeof(LineStyle), () => new LineStyleControl());
            Factory.Add(typeof(ShapeStyle), () => new ShapeStyleControl());
            Factory.Add(typeof(BaseStyle), () => new StyleControl());
            Factory.Add(typeof(TextStyle), () => new TextStyleControl());
        }

        public CachedContentPresenter()
        {
            Cache = new Dictionary<Type, Control>();

            this.GetObservable(DataContextProperty).Subscribe((value) =>
            {
                Debug.Print($"CachedContentPresenter DataContext Changed: {value}");
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
            Factory.TryGetValue(type, out createInstance);
            if (createInstance != null)
            {
                var sw = Stopwatch.StartNew();
                var instance = createInstance();
                sw.Stop();
                Debug.Print($"CreateInstance: {type} in {sw.Elapsed.TotalMilliseconds}ms.");
                return instance;
            }
            Debug.Print($"Not Registered: {type}");
            return null;
        }

        private Control GetControl(Type type)
        {
            Control control;
            Cache.TryGetValue(type, out control);
            if (control == null)
            {
                control = CreateControl(type);
                if (control != null)
                {
                    Debug.Print($"New: {type} -> {control}");
                    Cache.Add(type, control);
                    return control;
                }
                Debug.Print($"Failed to create control for type: {type}");
                return null;
            }
            Debug.Print($"Cached: {type} -> {control}");
            return control;
        }
    }
}