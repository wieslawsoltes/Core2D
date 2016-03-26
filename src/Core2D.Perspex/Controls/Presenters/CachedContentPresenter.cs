// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
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

namespace Core2D.Perspex.Controls.Presenters
{
    public class CachedContentPresenter : ContentPresenter
    {
        private static IDictionary<Type, Type> ControlTypes;

        private static IDictionary<Type, Control> ControlCache = new Dictionary<Type, Control>();

        static CachedContentPresenter()
        {
            ControlTypes = new Dictionary<Type, Type>();

            // Data
            //ControlTypes.Add(typeof(ImmutableArray<XColumn>), typeof(ColumnsControl));
            ControlTypes.Add(typeof(XDatabase), typeof(DatabaseControl));
            ControlTypes.Add(typeof(XContext), typeof(DataControl));
            //ControlTypes.Add(typeof(ImmutableArray<XProperty>), typeof(PropertiesControl));
            ControlTypes.Add(typeof(XRecord), typeof(RecordControl));
            //ControlTypes.Add(typeof(ImmutableArray<XRecord>), typeof(RecordsControl));

            // Path
            ControlTypes.Add(typeof(XArcSegment), typeof(ArcSegmentControl));
            ControlTypes.Add(typeof(XCubicBezierSegment), typeof(CubicBezierSegmentControl));
            ControlTypes.Add(typeof(XLineSegment), typeof(LineSegmentControl));
            ControlTypes.Add(typeof(XPathFigure), typeof(PathFigureControl));
            ControlTypes.Add(typeof(XPathGeometry), typeof(PathGeometryControl));
            ControlTypes.Add(typeof(XPathSize), typeof(PathSizeControl));
            ControlTypes.Add(typeof(XPolyCubicBezierSegment), typeof(PolyCubicBezierSegmentControl));
            ControlTypes.Add(typeof(XPolyLineSegment), typeof(PolyLineSegmentControl));
            ControlTypes.Add(typeof(XPolyQuadraticBezierSegment), typeof(PolyQuadraticBezierSegmentControl));
            ControlTypes.Add(typeof(XQuadraticBezierSegment), typeof(QuadraticBezierSegmentControl));

            // Shapes
            ControlTypes.Add(typeof(XArc), typeof(ArcControl));
            ControlTypes.Add(typeof(XCubicBezier), typeof(CubicBezierControl));
            ControlTypes.Add(typeof(XEllipse), typeof(EllipseControl));
            ControlTypes.Add(typeof(XGroup), typeof(GroupControl));
            ControlTypes.Add(typeof(XImage), typeof(ImageControl));
            ControlTypes.Add(typeof(XLine), typeof(LineControl));
            ControlTypes.Add(typeof(XPath), typeof(PathControl));
            ControlTypes.Add(typeof(XPoint), typeof(PointControl));
            ControlTypes.Add(typeof(XQuadraticBezier), typeof(QuadraticBezierControl));
            ControlTypes.Add(typeof(XRectangle), typeof(RectangleControl));
            ControlTypes.Add(typeof(XText), typeof(TextControl));

            // State
            ControlTypes.Add(typeof(ShapeState), typeof(ShapeStateControl));

            // Style
            ControlTypes.Add(typeof(ArgbColor), typeof(ArgbColorControl));
            ControlTypes.Add(typeof(ArrowStyle), typeof(ArrowStyleControl));
            ControlTypes.Add(typeof(FontStyle), typeof(FontStyleControl));
            ControlTypes.Add(typeof(LineFixedLength), typeof(LineFixedLengthControl));
            ControlTypes.Add(typeof(LineStyle), typeof(LineStyleControl));
            ControlTypes.Add(typeof(ShapeStyle), typeof(ShapeStyleControl));
            //ControlTypes.Add(typeof(BaseStyle), typeof(StyleControl));
            ControlTypes.Add(typeof(TextStyle), typeof(TextStyleControl));
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
            Type controlType;
            ControlTypes.TryGetValue(type, out controlType);
            if (controlType != null)
            {
                return (Control)Activator.CreateInstance(controlType);
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
