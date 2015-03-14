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

            var container = new WpfContainer();
            var editor = new PortableEditor(container);

            canvas.Children.Add(container);

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

            //Demo(container);
        }

        private static void Demo(IContainer container)
        {
            var layer = container.Layers.First();

            var style1 = new XStyle()
            {
                Stroke = new XColor() { A = 255, R = 255, G = 0, B = 0 },
                Fill = new XColor() { A = 255, R = 255, G = 255, B = 255 },
                Thickness = 2.0
            };

            var style2 = new XStyle()
            {
                Stroke = new XColor() { A = 255, R = 0, G = 255, B = 0 },
                Fill = new XColor() { A = 255, R = 255, G = 255, B = 255 },
                Thickness = 2.0
            };

            var style3 = new XStyle()
            {
                Stroke = new XColor() { A = 255, R = 0, G = 0, B = 255 },
                Fill = new XColor() { A = 255, R = 255, G = 255, B = 255 },
                Thickness = 2.0
            };

            var rand = new Random(Guid.NewGuid().GetHashCode());

            double width = 800;
            double height = 600;
            int shapes = 10;

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

            container.Invalidate();
        }
    }

    public class WpfContainer : FrameworkElement, IContainer
    {
        public IList<ILayer> Layers { get; set;	}
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            
            foreach (var layer in Layers)
            {
                foreach (var shape in layer.Shapes)
                {
                    shape.Draw(drawingContext, this);
                }
            }
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
        private readonly IDictionary<XStyle, Tuple<Brush, Pen>> _styleCache = new Dictionary<XStyle, Tuple<Brush, Pen>>();
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
                rx, 
                ry);
        }
    }

    // Portable

    public class XColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
    
    public class XStyle
    {
        public bool IsDirty { get; set; }
        public XColor Stroke { get; set; }
        public XColor Fill { get; set; }
        public double Thickness { get; set;	}
    }
    
    public class XPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
    
    public abstract class XShape
    {
        public abstract void Draw(object dc, IContainer renderer);
    }

    public interface ILayer
    {
        IList<XShape> Shapes { get; set; }
    }
    
    public interface IContainer
    {
        IList<ILayer> Layers { get; set; }
        void Invalidate();
        void Draw(object dc, XLine line);
        void Draw(object dc, XRectangle rectangle);
        void Draw(object dc, XEllipse ellipse);
    }

    public class XLine : XShape
    {
        public XStyle Style { get; set; }
        public XPoint Start { get; set; }
        public XPoint End { get; set; }
        
        public override void Draw(object dc, IContainer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XLine Create(double x1, double y1, double x2, double y2, XStyle style)
        {
            return new XLine()
            {
                Style = style,
                Start = new XPoint() { X = x1, Y = y1 },
                End = new XPoint() { X = x2, Y = y2 }
            };
        }
        
        public static XLine Create(double x, double y, XStyle style)
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
        
        public override void Draw(object dc, IContainer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XRectangle Create(double x1, double y1, double x2, double y2, XStyle style, bool isFilled = false)
        {
            return new XRectangle()
            {
                Style = style,
                TopLeft = new XPoint() { X = x1, Y = y1 },
                BottomRight = new XPoint() { X = x2, Y = y2 },
                IsFilled = isFilled
            };
        }
        
        public static XRectangle Create(double x, double y, XStyle style, bool isFilled = false)
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
        
        public override void Draw(object dc, IContainer renderer)
        {
            renderer.Draw(dc, this);
        }

        public static XEllipse Create(double x1, double y1, double x2, double y2, XStyle style, bool isFilled = false)
        {
            return new XEllipse()
            {
                Style = style,
                TopLeft = new XPoint() { X = x1, Y = y1 },
                BottomRight = new XPoint() { X = x2, Y = y2 },
                IsFilled = isFilled
            };
        }
        
        public static XEllipse Create(double x, double y, XStyle style, bool isFilled = false)
        {
            return Create(x, y, x, y, style, isFilled);
        }
    }
    
    public class Layer : ILayer
    {
        public IList<XShape> Shapes { get; set; }
    }

    public class PortableEditor
    {
        public enum Tool
        {
            None,
            Line,
            Rectangle,
            Ellipse
        }
        
        public enum State
        {
            None,
            One,
            Two,
            Three
        }
        
        private readonly IContainer _container;
        private ILayer _layer;
        private XShape _temp;
        private XStyle _style;
        private bool _defaultIsFilled;
        private Tool _tool;
        private State _state;
        
        public PortableEditor(IContainer container)
        {
            _container = container;
            _container.Layers = new ObservableCollection<ILayer>();
            
            _layer = new Layer();
            _layer.Shapes = new ObservableCollection<XShape>();

            _container.Layers.Add(_layer);

            _temp = null;
            
            _style = new XStyle()
            {
                Stroke = new XColor() { A = 255, R = 0, G = 0, B = 0 },
                Fill = new XColor() { A = 255, R = 255, G = 255, B = 255 },
                Thickness = 2.0
            };
            
            _defaultIsFilled = false;
            _tool = Tool.Rectangle;
            
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
                                    _layer.Shapes.Add(_temp);
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
                                    _layer.Shapes.Add(_temp);
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
                                    _layer.Shapes.Add(_temp);
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
                                    _layer.Shapes.Remove(_temp);
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
                                    _layer.Shapes.Remove(_temp);
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
                                    _layer.Shapes.Remove(_temp);
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
            }
        }
    }
}
