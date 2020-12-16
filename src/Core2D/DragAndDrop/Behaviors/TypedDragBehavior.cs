using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Core2D.DragAndDrop
{
    public sealed class TypedDragBehavior : Behavior<Control>
    {
        private Point _dragStartPoint;
        private PointerEventArgs _triggerEvent;
        private object _value;
        private bool _lock;

        public static readonly StyledProperty<Type> DataTypeProperty =
            AvaloniaProperty.Register<TypedDragBehavior, Type>(nameof(DataType));

        public static readonly StyledProperty<IDragHandler> HandlerProperty =
            AvaloniaProperty.Register<TypedDragBehavior, IDragHandler>(nameof(Handler));

        public Type DataType
        {
            get => GetValue(DataTypeProperty);
            set => SetValue(DataTypeProperty, value);
        }

        public IDragHandler Handler
        {
            get => GetValue(HandlerProperty);
            set => SetValue(HandlerProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed);
            AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased);
            AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved);
        }

        private async Task DoDragDrop(PointerEventArgs triggerEvent, object value)
        {
            var data = new DataObject();
            data.Set(ContextDropBehavior.DataFormat, value);

            var effect = DragDropEffects.None;

            if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Alt))
            {
                effect |= DragDropEffects.Link;
            }
            else if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                effect |= DragDropEffects.Move;
            }
            else if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                effect |= DragDropEffects.Copy;
            }
            else
            {
                effect |= DragDropEffects.Move;
            }

            await DragDrop.DoDragDrop(triggerEvent, data, effect);
        }

        private void AssociatedObject_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var properties = e.GetCurrentPoint(AssociatedObject).Properties;
            if (properties.IsLeftButtonPressed)
            {
                if (e.Source is IControl control && DataType.IsAssignableFrom(control.DataContext?.GetType()) == true)
                {
                    _dragStartPoint = e.GetPosition(null);
                    _triggerEvent = e;
                    _value = control.DataContext;
                    _lock = true;
                }
            }
        }

        private void AssociatedObject_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var properties = e.GetCurrentPoint(AssociatedObject).Properties;
            if (properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased && _triggerEvent is { })
            {
                _triggerEvent = null;
                _value = null;
                _lock = false;
            }
        }

        private async void AssociatedObject_PointerMoved(object sender, PointerEventArgs e)
        {
            var properties = e.GetCurrentPoint(AssociatedObject).Properties;
            if (properties.IsLeftButtonPressed && _triggerEvent is { })
            {
                var point = e.GetPosition(null);
                var diff = _dragStartPoint - point;
                if (Math.Abs(diff.X) > 3 || Math.Abs(diff.Y) > 3)
                {
                    if (_lock == true)
                    {
                        _lock = false;
                    }
                    else
                    {
                        return;
                    }

                    Handler?.BeforeDragDrop(sender, _triggerEvent, _value);

                    await DoDragDrop(_triggerEvent, _value);

                    Handler?.AfterDragDrop(sender, _triggerEvent, _value);

                    _triggerEvent = null;
                    _value = null;
                }
            }
        }
    }
}
