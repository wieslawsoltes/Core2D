using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.MatrixExtensions;
using Avalonia.VisualTree;
using Core2D.Editor;
using Core2D.Editor.Input;

namespace Core2D.UI.Avalonia.Controls
{
    /// <summary>
    /// Provides input events for <see cref="IInputTarget"/>.
    /// </summary>
    public class InputControl : Border
    {
        /// <summary>
        /// Input target property.
        /// </summary>
        public static readonly StyledProperty<IInputTarget> InputTargetProperty =
            AvaloniaProperty.Register<InputControl, IInputTarget>(nameof(InputTarget));

        /// <summary>
        /// Gets or sets input target.
        /// </summary>
        public IInputTarget InputTarget
        {
            get { return GetValue(InputTargetProperty); }
            set { SetValue(InputTargetProperty, value); }
        }

        /// <summary>
        /// Relative to visual property.
        /// </summary>
        public static readonly StyledProperty<IVisual> RelativeToProperty =
            AvaloniaProperty.Register<InputControl, IVisual>(nameof(RelativeTo));

        /// <summary>
        /// Gets or sets relative to visual.
        /// </summary>
        public IVisual RelativeTo
        {
            get { return GetValue(RelativeToProperty); }
            set { SetValue(RelativeToProperty, value); }
        }

        /// <summary>
        /// Transformed visual property.
        /// </summary>
        public static readonly StyledProperty<IVisual> TransformedProperty =
            AvaloniaProperty.Register<InputControl, IVisual>(nameof(Transformed));

        /// <summary>
        /// Gets or sets transformed visual.
        /// </summary>
        public IVisual Transformed
        {
            get { return GetValue(TransformedProperty); }
            set { SetValue(TransformedProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputControl"/> class.
        /// </summary>
        public InputControl()
        {
            PointerPressed += (sender, e) => HandlePointerPressed(e);
            PointerReleased += (sender, e) => HandlePointerReleased(e);
            PointerMoved += (sender, e) => HandlePointerMoved(e);
        }

        private ModifierFlags GetModifier(KeyModifiers inputModifiers)
        {
            var modifier = ModifierFlags.None;

            if (inputModifiers.HasFlag(KeyModifiers.Alt))
            {
                modifier |= ModifierFlags.Alt;
            }

            if (inputModifiers.HasFlag(KeyModifiers.Control))
            {
                modifier |= ModifierFlags.Control;
            }

            if (inputModifiers.HasFlag(KeyModifiers.Shift))
            {
                modifier |= ModifierFlags.Shift;
            }

            return modifier;
        }

        private Point AdjustGetPosition(Point point)
        {
            if (Transformed?.RenderTransform != null)
            {
                return MatrixHelper.TransformPoint(Transformed.RenderTransform.Value.Invert(), point);
            }
            return point;
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            var properties = e.GetCurrentPoint(RelativeTo).Properties;
            if (properties.IsLeftButtonPressed)
            {
                if (InputTarget != null)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    var args = new InputArgs(point.X, point.Y, GetModifier(e.KeyModifiers));
                    if (InputTarget.IsLeftDownAvailable())
                    {
                        InputTarget.LeftDown(args);
                    }
                }
            }
            else if (properties.IsRightButtonPressed)
            {
                if (InputTarget != null)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    var args = new InputArgs(point.X, point.Y, GetModifier(e.KeyModifiers));
                    if (InputTarget.IsRightDownAvailable())
                    {
                        InputTarget.RightDown(args);
                    }
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
            {
                if (InputTarget != null)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    var args = new InputArgs(point.X, point.Y, GetModifier(e.KeyModifiers));
                    if (InputTarget.IsLeftUpAvailable())
                    {
                        InputTarget.LeftUp(args);
                    }
                }
            }
            else if (e.InitialPressMouseButton == MouseButton.Right)
            {
                if (InputTarget != null)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    var args = new InputArgs(point.X, point.Y, GetModifier(e.KeyModifiers));
                    if (InputTarget.IsRightUpAvailable())
                    {
                        InputTarget.RightUp(args);
                    }
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            if (InputTarget != null)
            {
                var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                var args = new InputArgs(point.X, point.Y, GetModifier(e.KeyModifiers));
                if (InputTarget.IsMoveAvailable())
                {
                    InputTarget.Move(args);
                }
            }
        }
    }
}
