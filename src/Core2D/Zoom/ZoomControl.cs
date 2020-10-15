//#define DEBUG_POINTER_EVENTS
using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using Core2D.UI.Zoom.Input;

namespace Core2D.UI.Zoom
{
    public class ZoomControl : Border, IInputService, IZoomService
    {
        private IZoomServiceState _zoomServiceState;
        private IInputTarget _inputTarget;
        private IDrawTarget _drawTarget;
        private Dictionary<IPointer, (IPointer Pointer, Point Point, KeyModifiers InputModifiers)> _pointers;
        private bool _isCaptured;
        private KeyModifiers _capturedInputModifiers;
        private Matrix _currentMatrix;
        private Point _panPosition;

        public static readonly DirectProperty<ZoomControl, IZoomServiceState> ZoomServiceStateProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, IZoomServiceState>(nameof(ZoomServiceState), o => o.ZoomServiceState, (o, v) => o.ZoomServiceState = v);

        public static readonly DirectProperty<ZoomControl, IInputTarget> InputTargetProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, IInputTarget>(nameof(InputTarget), o => o.InputTarget, (o, v) => o.InputTarget = v);

        public static readonly DirectProperty<ZoomControl, IDrawTarget> DrawTargetProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, IDrawTarget>(nameof(DrawTarget), o => o.DrawTarget, (o, v) => o.DrawTarget = v);

        public IZoomServiceState ZoomServiceState
        {
            get { return _zoomServiceState; }
            set { SetAndRaise(ZoomServiceStateProperty, ref _zoomServiceState, value); }
        }

        public IInputTarget InputTarget
        {
            get { return _inputTarget; }
            set { SetAndRaise(InputTargetProperty, ref _inputTarget, value); }
        }

        public IDrawTarget DrawTarget
        {
            get { return _drawTarget; }
            set { SetAndRaise(DrawTargetProperty, ref _drawTarget, value); }
        }

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Func<bool> IsCaptured { get; set; }

        public Action Redraw { get; set; }

        public ZoomControl()
        {
            _zoomServiceState = null;
            _inputTarget = null;
            _drawTarget = null;
            _pointers = new Dictionary<IPointer, (IPointer, Point, KeyModifiers)>();
            _isCaptured = false;
            _capturedInputModifiers = KeyModifiers.None;
        }

        private void GetOffset(out double dx, out double dy, out double zx, out double zy)
        {
            dx = _zoomServiceState.OffsetX;
            dy = _zoomServiceState.OffsetY;
            zx = _zoomServiceState.ZoomX;
            zy = _zoomServiceState.ZoomY;
        }

        private Point AdjustPanPoint(Point point)
        {
            GetOffset(out double dx, out double dy, out double zx, out double zy);
            return new Point(point.X / zx, point.Y / zy);
        }

        private Point AdjustZoomPoint(Point point)
        {
            GetOffset(out double dx, out double dy, out double zx, out double zy);
            return new Point((point.X - dx) / zx, (point.Y - dy) / zy);
        }

        private Point AdjustTargetPoint(Point point)
        {
            GetOffset(out double dx, out double dy, out double zx, out double zy);
            return new Point((point.X - dx) / zx, (point.Y - dy) / zy);
        }

        private void UpdatePointer(PointerEventArgs e)
        {
            if (!_pointers.TryGetValue(e.Pointer, out var _))
            {
                if (e.RoutedEvent == PointerMovedEvent)
                {
                    return;
                }
            }
            _pointers[e.Pointer] = (e.Pointer, e.GetPosition(this), e.KeyModifiers);
        }

        private void GetPointerPressedType(PointerPressedEventArgs e, out bool isLeft)
        {
            isLeft = false;

            if (e.Pointer.Type == PointerType.Mouse)
            {
                isLeft = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;
            }
            else if (e.Pointer.Type == PointerType.Touch)
            {
                isLeft = e.Pointer.IsPrimary;
            }

            _capturedInputModifiers = e.KeyModifiers;
#if DEBUG_POINTER_EVENTS
            System.Diagnostics.Debug.WriteLine(
                $"[Pressed] type: {e.Pointer.Type}, " +
                $"isLeft: {isLeft}, " +
                $"isPrimary: {e.Pointer.IsPrimary}, " +
                $"modifiers: {_capturedInputModifiers}, " +
                $"point: {e.GetPosition(this)}, " +
                $"Captured: {e.Pointer.Captured}");
#endif
        }

        private void GetPointerReleasedType(PointerReleasedEventArgs e, out bool isLeft)
        {
            isLeft = false;

            if (e.Pointer.Type == PointerType.Mouse)
            {
                isLeft = e.InitialPressMouseButton == MouseButton.Left;
            }
            else if (e.Pointer.Type == PointerType.Touch)
            {
                isLeft = e.Pointer.IsPrimary;
            }
#if DEBUG_POINTER_EVENTS
            System.Diagnostics.Debug.WriteLine(
                $"[Released] type: {e.Pointer.Type}, " +
                $"isLeft: {isLeft}, " +
                $"isPrimary: {e.Pointer.IsPrimary}, " +
                $"modifiers: {_capturedInputModifiers}, " +
                $"point: {e.GetPosition(this)}, " +
                $"Captured: {e.Pointer.Captured}");
#endif
            _capturedInputModifiers = KeyModifiers.None;
        }

        private void GetPointerMovedType(PointerEventArgs e, out bool isLeft)
        {
            isLeft = false;

            if (e.Pointer.Type == PointerType.Mouse)
            {
                isLeft = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;
            }
            else if (e.Pointer.Type == PointerType.Touch)
            {
                isLeft = e.Pointer.IsPrimary;
            }
#if DEBUG_POINTER_EVENTS
            System.Diagnostics.Debug.WriteLine(
                $"[Moved] type: {e.Pointer.Type}, " +
                $"isLeft: {isLeft}, " +
                $"isPrimary: {e.Pointer.IsPrimary}, " +
                $"modifiers: {_capturedInputModifiers}, " +
                $"point: {e.GetPosition(this)}, " +
                $"Captured: {e.Pointer.Captured}");
#endif
        }

        private void HandlePointerWheelChanged(PointerWheelEventArgs e)
        {
            if (_zoomServiceState != null)
            {
                var zpoint = AdjustZoomPoint(e.GetPosition(this));
                Wheel(e.Delta.Y, zpoint.X, zpoint.Y);
            }
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            UpdatePointer(e);
            GetPointerPressedType(e, out var isLeft);

            if (_zoomServiceState != null && _inputTarget != null)
            {
                if (isLeft == true)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    _inputTarget.LeftDown(tpoint.X, tpoint.Y, GetModifier(e.KeyModifiers));
                }
                else
                {
                    if (_isCaptured == false)
                    {
                        var zpoint = AdjustPanPoint(e.GetPosition(this));
                        Pressed(zpoint.X, zpoint.Y);
                    }

                    if (_zoomServiceState.IsPanning == false)
                    {
                        var tpoint = AdjustTargetPoint(e.GetPosition(this));
                        _inputTarget.RightDown(tpoint.X, tpoint.Y, GetModifier(e.KeyModifiers));
                    }
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            GetPointerReleasedType(e, out var isLeft);

            if (_zoomServiceState != null && _inputTarget != null)
            {
                if (isLeft == true)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    _inputTarget.LeftUp(tpoint.X, tpoint.Y, GetModifier(e.KeyModifiers));
                }
                else
                {
                    if (_isCaptured == false)
                    {
                        var zpoint = AdjustPanPoint(e.GetPosition(this));
                        Released(zpoint.X, zpoint.Y);
                    }

                    if (_zoomServiceState.IsPanning == false)
                    {
                        var tpoint = AdjustTargetPoint(e.GetPosition(this));
                        _inputTarget.RightUp(tpoint.X, tpoint.Y, GetModifier(e.KeyModifiers));
                    }
                }
            }

            _pointers.Remove(e.Pointer);
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            UpdatePointer(e);
            GetPointerMovedType(e, out var isLeft);

            if (_zoomServiceState != null && _inputTarget != null)
            {
                if (isLeft == false)
                {
                    var zpoint = AdjustPanPoint(e.GetPosition(this));
                    Moved(zpoint.X, zpoint.Y);
                }

                if (_zoomServiceState.IsPanning == false)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    _inputTarget.Move(tpoint.X, tpoint.Y, GetModifier(e.KeyModifiers));
                }
            }
        }

        private Modifier GetModifier(KeyModifiers inputModifiers)
        {
            var modifier = Modifier.None;

            if (inputModifiers.HasFlag(KeyModifiers.Alt))
            {
                modifier |= Modifier.Alt;
            }

            if (inputModifiers.HasFlag(KeyModifiers.Control))
            {
                modifier |= Modifier.Control;
            }

            if (inputModifiers.HasFlag(KeyModifiers.Shift))
            {
                modifier |= Modifier.Shift;
            }

            return modifier;
        }

        public void Wheel(double delta, double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                _zoomServiceState.IsZooming = true;
                _zoomServiceState.AutoFitMode = FitMode.None;
                ZoomDeltaTo(delta, x, y);
                Invalidate(true);
            }
        }

        public void Pressed(double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null && _zoomServiceState.IsPanning == false)
            {
                _zoomServiceState.IsPanning = true;
                _zoomServiceState.AutoFitMode = FitMode.None;
                StartPan(x, y);
                Invalidate(true);
            }
        }

        public void Released(double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null && _zoomServiceState.IsPanning == true)
            {
                Invalidate(true);
                _zoomServiceState.IsPanning = false;
            }
        }

        public void Moved(double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null && _zoomServiceState.IsPanning == true)
            {
                PanTo(x, y);
                Invalidate(true);
            }
        }

        public void Invalidate(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                if (redraw)
                {
                    Redraw?.Invoke();
                }
            }
        }

        private void UpdateCurrentMatrix()
        {
            _currentMatrix = new Matrix(
                _zoomServiceState.ZoomX,
                0,
                0,
                _zoomServiceState.ZoomY,
                _zoomServiceState.OffsetX,
                _zoomServiceState.OffsetY);
        }

        private void UpdateZoomServiceState()
        {
            _zoomServiceState.ZoomX = _currentMatrix.M11;
            _zoomServiceState.ZoomY = _currentMatrix.M22;
            _zoomServiceState.OffsetX = _currentMatrix.M31;
            _zoomServiceState.OffsetY = _currentMatrix.M32;
        }

        public void ZoomTo(double zoom, double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                UpdateCurrentMatrix();
                _currentMatrix = new Matrix(zoom, 0, 0, zoom, x - (zoom * x), y - (zoom * y)) * _currentMatrix;
                UpdateZoomServiceState();
            }
        }

        public void ZoomDeltaTo(double delta, double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                ZoomTo(delta > 0 ? _zoomServiceState.ZoomSpeed : 1 / _zoomServiceState.ZoomSpeed, x, y);
            }
        }

        public void StartPan(double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                _panPosition = new Point(x, y);
            }
        }

        public void PanTo(double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                double dx = x - _panPosition.X;
                double dy = y - _panPosition.Y;
                Point delta = new Point(dx, dy);
                _panPosition = new Point(x, y);
                UpdateCurrentMatrix();
                _currentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, delta.X, delta.Y) * _currentMatrix;
                UpdateZoomServiceState();
            }
        }

        public void Reset()
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                _currentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
                UpdateZoomServiceState();
            }
        }

        public void Center(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                double ox = (panelWidth - elementWidth) / 2;
                double oy = (panelHeight - elementHeight) / 2;
                _currentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, ox, oy);
                UpdateZoomServiceState();
            }
        }

        public void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                double zx = panelWidth / elementWidth;
                double zy = panelHeight / elementHeight;
                double ox = (panelWidth - elementWidth * zx) / 2;
                double oy = (panelHeight - elementHeight * zy) / 2;
                _currentMatrix = new Matrix(zx, 0.0, 0.0, zy, ox, oy);
                UpdateZoomServiceState();
            }
        }

        public void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                double zx = panelWidth / elementWidth;
                double zy = panelHeight / elementHeight;
                double zoom = Math.Min(zx, zy);
                double ox = (panelWidth - elementWidth * zoom) / 2;
                double oy = (panelHeight - elementHeight * zoom) / 2;
                _currentMatrix = new Matrix(zoom, 0.0, 0.0, zoom, ox, oy);
                UpdateZoomServiceState();
            }
        }

        public void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                double zx = panelWidth / elementWidth;
                double zy = panelHeight / elementHeight;
                double zoom = Math.Max(zx, zy);
                double ox = (panelWidth - elementWidth * zoom) / 2;
                double oy = (panelHeight - elementHeight * zoom) / 2;
                _currentMatrix = new Matrix(zoom, 0.0, 0.0, zoom, ox, oy);
                UpdateZoomServiceState();
            }
        }

        public void ResetZoom(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                Reset();
                Invalidate(redraw);
            }
        }

        public void CenterZoom(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                Center(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        public void FillZoom(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                Fill(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        public void UniformZoom(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                Uniform(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        public void UniformToFillZoom(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                UniformToFill(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            HandlePointerWheelChanged(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            HandlePointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            HandlePointerReleased(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            HandlePointerMoved(e);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            var md = (this.GetVisualRoot() as IInputRoot)?.MouseDevice;
            if (md != null)
            {
                this.Capture = () =>
                {
#if DEBUG_POINTER_EVENTS
                    System.Diagnostics.Debug.WriteLine($"[Capture] {_isCaptured}");
#endif
                    _isCaptured = true;
                };

                this.Release = () =>
                {
#if DEBUG_POINTER_EVENTS
                    System.Diagnostics.Debug.WriteLine($"[Release] {_isCaptured}");
#endif
                    _isCaptured = false;
                };

                this.IsCaptured = () =>
                {
#if DEBUG_POINTER_EVENTS
                    System.Diagnostics.Debug.WriteLine($"[IsCaptured] {_isCaptured}");
#endif
                    return _isCaptured;
                };

                this.Redraw = () =>
                {
                    this.InvalidateVisual();
                };
            }

            if (_inputTarget != null && _drawTarget != null)
            {
                _drawTarget.InputService = this;
                _drawTarget.ZoomService = this;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (_inputTarget != null && _drawTarget != null)
            {
                _drawTarget.InputService = null;
                _drawTarget.ZoomService = null;
            }
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            this.Focus();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_zoomServiceState != null && _inputTarget != null && _drawTarget != null)
            {
                bool initializeZoom =
                    double.IsNaN(_zoomServiceState.ZoomX)
                    || double.IsNaN(_zoomServiceState.ZoomY)
                    || double.IsNaN(_zoomServiceState.OffsetX)
                    || double.IsNaN(_zoomServiceState.OffsetY);

                if (initializeZoom == true)
                {
                    switch (_zoomServiceState.InitFitMode)
                    {
                        case FitMode.None:
                        case FitMode.Reset:
                            ResetZoom(false);
                            break;
                        case FitMode.Center:
                            CenterZoom(false);
                            break;
                        case FitMode.Fill:
                            FillZoom(false);
                            break;
                        case FitMode.Uniform:
                            UniformZoom(false);
                            break;
                        case FitMode.UniformToFill:
                            UniformToFillZoom(false);
                            break;
                    }
                }

                if (initializeZoom == false && _zoomServiceState.IsPanning == false && _zoomServiceState.IsZooming == false)
                {
                    switch (_zoomServiceState.AutoFitMode)
                    {
                        case FitMode.None:
                            break;
                        case FitMode.Reset:
                            ResetZoom(false);
                            break;
                        case FitMode.Center:
                            CenterZoom(false);
                            break;
                        case FitMode.Fill:
                            FillZoom(false);
                            break;
                        case FitMode.Uniform:
                            UniformZoom(false);
                            break;
                        case FitMode.UniformToFill:
                            UniformToFillZoom(false);
                            break;
                    }
                }

                GetOffset(out double dx, out double dy, out double zx, out double zy);

                _drawTarget.Draw(context, Bounds.Width, Bounds.Height, dx, dy, zx, zy);

                if (_zoomServiceState.IsZooming == true)
                {
                    _zoomServiceState.IsZooming = false;
                }
            }
#if DEBUG_POINTER_EVENTS
            var brush = new Avalonia.Media.Immutable.ImmutableSolidColorBrush(Colors.Magenta);
            foreach (var value in _pointers.Values)
            {
                context.DrawGeometry(brush, null, new EllipseGeometry(new Rect(value.Point.X - 25, value.Point.Y - 25, 50, 50)));
            }
#endif
        }
    }
}
