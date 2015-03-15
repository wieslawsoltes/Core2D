using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Test
{
    // Native

    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            // initialize container

            var container = new XContainer() { Layers = new ObservableCollection<ILayer>() };
            var layer = new XLayer() { Shapes = new ObservableCollection<XShape>() };

            container.Layers.Add(layer);
            container.Current = layer;

            // initialize renderer

            var renderer = new WpfRenderer(container);

            container.Invalidate = () => renderer.Invalidate();

            // initialize editor

            var editor = new PortableEditor(container);

            // initialize canvas

            canvas.Children.Add(renderer);

            canvas.PreviewMouseLeftButtonDown += (s, e) =>
            {
                var p = e.GetPosition(canvas);
                editor.Left(p.X, p.Y);
            };
            
            canvas.PreviewMouseRightButtonDown += (s, e) =>
            {
                var p = e.GetPosition(canvas);
                editor.Right(p.X, p.Y);
            };
            
            canvas.PreviewMouseMove += (s, e) =>
            {
                var p = e.GetPosition(canvas);
                editor.Move(p.X, p.Y);
            };

            // initialize menu

            fileExit.Click += (s, e) => this.Close();

            toolNone.Click += (s, e) => editor.CurrentTool = PortableEditor.Tool.None;
            toolLine.Click += (s, e) => editor.CurrentTool = PortableEditor.Tool.Line;
            toolRectangle.Click += (s, e) => editor.CurrentTool = PortableEditor.Tool.Rectangle;
            toolEllipse.Click += (s, e) => editor.CurrentTool = PortableEditor.Tool.Ellipse;
            toolBezier.Click += (s, e) => editor.CurrentTool = PortableEditor.Tool.Bezier;

            optionsIsFilled.Click += (s, e) => editor.DefaultIsFilled = !editor.DefaultIsFilled;

            // initialize demo

            //Demo(container, 800, 600, 10);
        }

        private static void Demo(IContainer container, double width, double height, int shapes)
        {
            var style1 = XStyle.Create(255, 255, 0, 0, 255, 255, 255, 255, 2.0);
            var style2 = XStyle.Create(255, 0, 255, 0, 255, 255, 255, 255, 2.0);
            var style3 = XStyle.Create(255, 0, 0, 255, 255, 255, 255, 255, 2.0);
            var style4 = XStyle.Create(255, 0, 255, 255, 255, 255, 255, 255, 2.0);
            var layer = container.Current;
            var rand = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < shapes; i++)
            {
                double x1 = rand.NextDouble() * width;
                double y1 = rand.NextDouble() * height;
                double x2 = rand.NextDouble() * width;
                double y2 = rand.NextDouble() * height;
                var l = XLine.Create(x1, y1, x2, y2, style1);
                layer.Shapes.Add(l);
            }

            for (int i = 0; i < shapes; i++)
            {
                double x1 = rand.NextDouble() * width;
                double y1 = rand.NextDouble() * height;
                double x2 = rand.NextDouble() * width;
                double y2 = rand.NextDouble() * height;
                var r = XRectangle.Create(x1, y1, x2, y2, style2);
                layer.Shapes.Add(r);
            }

            for (int i = 0; i < shapes; i++)
            {
                double x1 = rand.NextDouble() * width;
                double y1 = rand.NextDouble() * height;
                double x2 = rand.NextDouble() * width;
                double y2 = rand.NextDouble() * height;
                var e = XEllipse.Create(x1, y1, x2, y2, style3);
                layer.Shapes.Add(e);
            }

            for (int i = 0; i < shapes; i++)
            {
                double x1 = rand.NextDouble() * width;
                double y1 = rand.NextDouble() * height;
                double x2 = rand.NextDouble() * width;
                double y2 = rand.NextDouble() * height;
                double x3 = rand.NextDouble() * width;
                double y3 = rand.NextDouble() * height;
                double x4 = rand.NextDouble() * width;
                double y4 = rand.NextDouble() * height;
                var e = XBezier.Create(x1, y1, x2, y2, x3, y3, x4, y4, style4);
                layer.Shapes.Add(e);
            }

            container.Invalidate();
        }
    }

    public class WpfRenderer : FrameworkElement, IRenderer
    {
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var sw = System.Diagnostics.Stopwatch.StartNew();

            Render(drawingContext);

            sw.Stop();
            System.Diagnostics.Trace.WriteLine("OnRender: " + sw.Elapsed.TotalMilliseconds + "ms");
        }

        private void Render(DrawingContext dc)
        {
            foreach (var layer in _container.Layers)
            {
                foreach (var shape in layer.Shapes)
                {
                    shape.Draw(dc, this);
                }
            }
        }

        private readonly IContainer _container;

        public WpfRenderer(IContainer container)
        {
            _container = container;
        }

        public void Invalidate()
        {
            this.InvalidateVisual();
        }
        
        private Brush CreateBrush(XColor color)
        {
            var brush = new SolidColorBrush(
                Color.FromArgb(
                    color.A,
                    color.R,
                    color.G,
                    color.B));
            brush.Freeze();
            return brush;
        }

        private Pen CreatePen(XColor color, double thickness)
        {
            var brush = CreateBrush(color);
            var pen = new Pen(brush, thickness);
            pen.Freeze();		
            return pen;
        }
        
        private Rect CreateRect(XPoint topLeft, XPoint bottomRight)
        {
            double tlx = Math.Min(topLeft.X, bottomRight.X);
            double tly = Math.Min(topLeft.Y, bottomRight.Y);
            double brx = Math.Max(topLeft.X, bottomRight.X);
            double bry = Math.Max(topLeft.Y, bottomRight.Y);
            return new Rect(
                new Point(tlx, tly),
                new Point(brx, bry));
        }
        
        // Tuple<Brush, Pen>: Brush is used for Fill, Pen if used for Stroke
        private readonly IDictionary<XStyle, Tuple<Brush, Pen>> _styleCache = 
            new Dictionary<XStyle, Tuple<Brush, Pen>>();
        private readonly bool _enableStyleCache = true;
        
        public void Draw(object dc, XLine line)
        {
            var _dc = dc as DrawingContext;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache && _styleCache.TryGetValue(line.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(line.Style.Fill);
                stroke = CreatePen(
                    line.Style.Stroke, 
                    line.Style.Thickness);
                
                if (_enableStyleCache)
                    _styleCache.Add(line.Style, Tuple.Create(fill, stroke));
            }

            _dc.DrawLine(
                stroke,
                new Point(line.Start.X, line.Start.Y), 
                new Point(line.End.X, line.End.Y));
        }
        
        public void Draw(object dc, XRectangle rectangle)
        {
            var _dc = dc as DrawingContext;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache && _styleCache.TryGetValue(rectangle.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(rectangle.Style.Fill);
                stroke = CreatePen(
                    rectangle.Style.Stroke, 
                    rectangle.Style.Thickness);
                
                if (_enableStyleCache)
                    _styleCache.Add(rectangle.Style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                rectangle.TopLeft, 
                rectangle.BottomRight);
            _dc.DrawRectangle(
                rectangle.IsFilled ? fill : null,
                stroke, 
                rect);
        }
        
        public void Draw(object dc, XEllipse ellipse)
        {
            var _dc = dc as DrawingContext;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache && _styleCache.TryGetValue(ellipse.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(ellipse.Style.Fill);
                stroke = CreatePen(
                    ellipse.Style.Stroke, 
                    ellipse.Style.Thickness);
                
                if (_enableStyleCache)
                    _styleCache.Add(ellipse.Style, Tuple.Create(fill, stroke));
            }
            
            var rect = CreateRect(
                ellipse.TopLeft, 
                ellipse.BottomRight);
            double rx = rect.Width / 2.0;
            double ry = rect.Height / 2.0;
            var center = new Point(rect.X + rx, rect.Y + ry);
            _dc.DrawEllipse(
                ellipse.IsFilled ? fill : null,
                stroke, 
                center,
                rx, ry);
        }

        private readonly IDictionary<XBezier, PathGeometry> _bezierCache =
            new Dictionary<XBezier, PathGeometry>();
        private readonly bool _enableBezierCache = true;

        public void Draw(object dc, XBezier bezier)
        {
            var _dc = dc as DrawingContext;
            
            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache && _styleCache.TryGetValue(bezier.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(bezier.Style.Fill);
                stroke = CreatePen(
                    bezier.Style.Stroke,
                    bezier.Style.Thickness);
                
                if (_enableStyleCache)
                    _styleCache.Add(bezier.Style, Tuple.Create(fill, stroke));
            }

            PathGeometry pg;
            if (_enableBezierCache && _bezierCache.TryGetValue(bezier, out pg))
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new Point(bezier.Point1.X, bezier.Point1.Y);
                var bs = pf.Segments[0] as BezierSegment;
                bs.Point1 = new Point(bezier.Point2.X, bezier.Point2.Y);
                bs.Point2 = new Point(bezier.Point3.X, bezier.Point3.Y);
                bs.Point3 = new Point(bezier.Point4.X, bezier.Point4.Y);
            }
            else
            {
                var pf = new PathFigure() { StartPoint = new Point(bezier.Point1.X, bezier.Point1.Y) };
                var bs = new BezierSegment(
                        new Point(bezier.Point2.X, bezier.Point2.Y),
                        new Point(bezier.Point3.X, bezier.Point3.Y),
                        new Point(bezier.Point4.X, bezier.Point4.Y),
                        true);
                //bs.Freeze();
                pf.Segments.Add(bs);
                //pf.Freeze();
                pg = new PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                if (_enableBezierCache)
                    _bezierCache.Add(bezier, pg);
            }

            _dc.DrawGeometry(bezier.IsFilled ? fill : null, stroke, pg);
        }
    }

    // Portable

    public class XColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static XColor Create(byte a, byte r, byte g, byte b)
        {
            return new XColor() { A = a, R = r, G = g, B = b };
        }
    }
    
    public class XStyle
    {
        public bool IsDirty { get; set; }
        public XColor Stroke { get; set; }
        public XColor Fill { get; set; }
        public double Thickness { get; set;	}

        public static XStyle Create(
            byte sa, byte sr, byte sg, byte sb,
            byte fa, byte fr, byte fg, byte fb,
            double thickness)
        {
            return new XStyle()
            {
                Stroke = XColor.Create(sa, sr, sg, sb),
                Fill = XColor.Create(fa, fr, fg, fb),
                Thickness = thickness
            };
        }
    }
    
    public class XPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static XPoint Create(double x, double y)
        {
            return new XPoint() { X = x, Y = y };
        }
    }
    
    public abstract class XShape
    {
        public abstract void Draw(object dc, IRenderer renderer);
    }

    public interface ILayer
    {
        IList<XShape> Shapes { get; set; }
    }
    
    public interface IContainer
    {
        IList<ILayer> Layers { get; set; }
        ILayer Current { get; set; }
        Action Invalidate { get; set; }
    }

    public class XLayer : ILayer
    {
        public IList<XShape> Shapes { get; set; }
    }

    public class XContainer : IContainer
    {
        public IList<ILayer> Layers { get; set; }
        public ILayer Current { get; set; }
        public Action Invalidate { get; set; }
    }

    public interface IRenderer
    {
        void Invalidate();
        void Draw(object dc, XLine line);
        void Draw(object dc, XRectangle rectangle);
        void Draw(object dc, XEllipse ellipse);
        void Draw(object dc, XBezier bezier);
    }

    public class XLine : XShape
    {
        public XStyle Style { get; set; }
        public XPoint Start { get; set; }
        public XPoint End { get; set; }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XLine Create(
            double x1, double y1, 
            double x2, double y2, 
            XStyle style)
        {
            return new XLine()
            {
                Style = style,
                Start = XPoint.Create(x1, y1),
                End = XPoint.Create(x2, y2)
            };
        }
        
        public static XLine Create(
            double x, double y, 
            XStyle style)
        {
            return Create(x, y, x, y, style);
        }
    }

    public class XRectangle : XShape
    {
        public XStyle Style { get; set; }
        public XPoint TopLeft { get; set; }
        public XPoint BottomRight { get; set; }
        public bool IsFilled { get; set; }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XRectangle Create(
            double x1, double y1, 
            double x2, double y2, 
            XStyle style, 
            bool isFilled = false)
        {
            return new XRectangle()
            {
                Style = style,
                TopLeft = XPoint.Create(x1, y1),
                BottomRight = XPoint.Create(x2, y2),
                IsFilled = isFilled
            };
        }
        
        public static XRectangle Create(
            double x, double y, 
            XStyle style,
            bool isFilled = false)
        {
            return Create(x, y, x, y, style, isFilled);
        }
    }
    
    public class XEllipse : XShape
    {
        public XStyle Style { get; set; }
        public XPoint TopLeft { get; set; }
        public XPoint BottomRight { get; set; }
        public bool IsFilled { get; set; }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XEllipse Create(
            double x1, double y1, 
            double x2, double y2, 
            XStyle style, 
            bool isFilled = false)
        {
            return new XEllipse()
            {
                Style = style,
                TopLeft = XPoint.Create(x1, y1),
                BottomRight = XPoint.Create(x2, y2),
                IsFilled = isFilled
            };
        }
        
        public static XEllipse Create(
            double x, double y, 
            XStyle style, 
            bool isFilled = false)
        {
            return Create(x, y, x, y, style, isFilled);
        }
    }

    public class XBezier : XShape
    {
        public XStyle Style { get; set; }
        public XPoint Point1 { get; set; }
        public XPoint Point2 { get; set; }
        public XPoint Point3 { get; set; }
        public XPoint Point4 { get; set; }
        public bool IsFilled { get; set; }

        public override void Draw(object dc, IRenderer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XBezier Create(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4,
            XStyle style,
            bool isFilled = false)
        {
            return new XBezier()
            {
                Style = style,
                Point1 = XPoint.Create(x1, y1),
                Point2 = XPoint.Create(x2, y2),
                Point3 = XPoint.Create(x3, y3),
                Point4 = XPoint.Create(x4, y4),
                IsFilled = isFilled
            };
        }

        public static XBezier Create(
            double x, double y,
            XStyle style,
            bool isFilled = false)
        {
            return Create(x, y, x, y, x, y, x, y, style, isFilled);
        }
    }

    public class PortableEditor
    {
        public enum Tool
        {
            None,
            Line,
            Rectangle,
            Ellipse,
            Bezier
        }
        
        public enum State
        {
            None,
            One,
            Two,
            Three,
            Four
        }

        private Tool _tool;
        public Tool CurrentTool
        {
            get { return _tool; }
            set { _tool = value; }
        }

        private State _state;
        public State CurrentState
        {
            get { return _state; }
            set { _state = value; }
        }

        private bool _defaultIsFilled;
        public bool DefaultIsFilled
        {
            get { return _defaultIsFilled; }
            set { _defaultIsFilled = value; }
        }

        private XStyle _style;
        public XStyle CurrentStyle
        {
            get { return _style; }
            set { _style = value; }
        }

        private readonly IContainer _container;
        private XShape _temp;

        public PortableEditor(IContainer container)
        {
            _container = container;
            _temp = null;
            _style = XStyle.Create(255, 0, 0, 0, 255, 255, 255, 255, 2.0);
            _defaultIsFilled = false;
            _tool = Tool.Line;
            _state = State.None;
        }
        
        public void Left(double x, double y)
        {
            switch (_tool) 
            {
                // None
                case Tool.None:
                    {
                    }
                    break;
                // Line
                case Tool.Line:
                    {
                        switch (_state) 
                        {
                            case State.None:
                                {
                                    _temp = XLine.Create(x, y, _style);
                                    _container.Current.Shapes.Add(_temp);
                                    _container.Invalidate();
                                    _state = State.One;
                                }
                                break;
                            case State.One:	
                                {
                                    var line = _temp as XLine;
                                    line.End.X = x;
                                    line.End.Y = y;
                                    _container.Invalidate();
                                    _state = State.None;
                                }
                                break;
                        }
                    }
                    break;
                // Rectangle
                case Tool.Rectangle:
                    {
                        switch (_state) 
                        {
                            case State.None:
                                {
                                    _temp = XRectangle.Create(x, y, _style, _defaultIsFilled);
                                    _container.Current.Shapes.Add(_temp);
                                    _container.Invalidate();
                                    _state = State.One;
                                }
                                break;
                            case State.One:	
                                {
                                    var rectangle = _temp as XRectangle;
                                    rectangle.BottomRight.X = x;
                                    rectangle.BottomRight.Y = y;
                                    _container.Invalidate();
                                    _state = State.None;
                                }
                                break;
                        }
                    }
                    break;
                // Ellipse
                case Tool.Ellipse:
                    {
                        switch (_state) 
                        {
                            case State.None:
                                {
                                    _temp = XEllipse.Create(x, y, _style, _defaultIsFilled);
                                    _container.Current.Shapes.Add(_temp);
                                    _container.Invalidate();
                                    _state = State.One;
                                }
                                break;
                            case State.One:	
                                {
                                    var ellipse = _temp as XEllipse;
                                    ellipse.BottomRight.X = x;
                                    ellipse.BottomRight.Y = y;
                                    _container.Invalidate();
                                    _state = State.None;
                                }
                                break;
                        }
                    }
                    break;
                // Bezier
                case Tool.Bezier:
                    {
                        switch (_state)
                        {
                            case State.None:
                                {
                                    _temp = XBezier.Create(x, y, _style);
                                    _container.Current.Shapes.Add(_temp);
                                    _container.Invalidate();
                                    _state = State.One;
                                }
                                break;
                            case State.One:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    bezier.Point3.X = x;
                                    bezier.Point3.Y = y;
                                    bezier.Point4.X = x;
                                    bezier.Point4.Y = y;
                                    _container.Invalidate();
                                    _state = State.Two;
                                }
                                break;
                            case State.Two:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    bezier.Point3.X = x;
                                    bezier.Point3.Y = y;
                                    _container.Invalidate();
                                    _state = State.Three;
                                }
                                break;
                            case State.Three:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    _container.Invalidate();
                                    _state = State.None;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        
        public void Right(double x, double y)
        {
            switch (_tool) 
            {
                // None
                case Tool.None:
                    {
                    }
                    break;
                // Line
                case Tool.Line:
                    {
                        switch (_state) 
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:	
                                {
                                    _container.Current.Shapes.Remove(_temp);
                                    _container.Invalidate();
                                    _state = State.None;
                                }
                                break;
                        }
                    }
                    break;
                // Rectangle
                case Tool.Rectangle:
                    {
                        switch (_state) 
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:	
                                {
                                    _container.Current.Shapes.Remove(_temp);
                                    _container.Invalidate();
                                    _state = State.None;
                                }
                                break;
                        }
                    }
                    break;
                // Ellipse
                case Tool.Ellipse:
                    {
                        switch (_state) 
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:	
                                {
                                    _container.Current.Shapes.Remove(_temp);
                                    _container.Invalidate();
                                    _state = State.None;
                                }
                                break;
                        }
                    }
                    break;
                // Bezier
                case Tool.Bezier:
                    {
                        switch (_state)
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:
                            case State.Two:
                            case State.Three:
                                {
                                    _container.Current.Shapes.Remove(_temp);
                                    _container.Invalidate();
                                    _state = State.None;
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        
        public void Move(double x, double y)
        {
            switch (_tool) 
            {
                // None
                case Tool.None:
                    {
                    }
                    break;
                // Line
                case Tool.Line:
                    {
                        switch (_state) 
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:	
                                {
                                    var line = _temp as XLine;
                                    line.End.X = x;
                                    line.End.Y = y;
                                    _container.Invalidate();
                                }
                                break;
                        }
                    }
                    break;
                // Rectangle
                case Tool.Rectangle:
                    {
                        switch (_state) 
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:	
                                {
                                    var rectangle = _temp as XRectangle;
                                    rectangle.BottomRight.X = x;
                                    rectangle.BottomRight.Y = y;
                                    _container.Invalidate();
                                }
                                break;
                        }
                    }
                    break;
                // Ellipse
                case Tool.Ellipse:
                    {
                        switch (_state) 
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:	
                                {
                                    var ellipse = _temp as XEllipse;
                                    ellipse.BottomRight.X = x;
                                    ellipse.BottomRight.Y = y;
                                    _container.Invalidate();
                                }
                                break;
                        }
                    }
                    break;
                // Bezier
                case Tool.Bezier:
                    {
                        switch (_state)
                        {
                            case State.None:
                                {
                                }
                                break;
                            case State.One:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    bezier.Point3.X = x;
                                    bezier.Point3.Y = y;
                                    bezier.Point4.X = x;
                                    bezier.Point4.Y = y;
                                    _container.Invalidate();
                                }
                                break;
                            case State.Two:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    bezier.Point3.X = x;
                                    bezier.Point3.Y = y;
                                    _container.Invalidate();
                                }
                                break;
                            case State.Three:
                                {
                                    var bezier = _temp as XBezier;
                                    bezier.Point2.X = x;
                                    bezier.Point2.Y = y;
                                    _container.Invalidate();
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
