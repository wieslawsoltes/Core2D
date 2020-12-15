using Avalonia;
using Avalonia.Data;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;

namespace Core2D.Modules.Renderer
{
    public class RendererOptions
    {
        public static readonly AttachedProperty<IShapeRenderer> RendererProperty =
            AvaloniaProperty.RegisterAttached<RendererOptions, AvaloniaObject, IShapeRenderer>("Renderer", null, true, BindingMode.TwoWay);

        public static IShapeRenderer GetRenderer(AvaloniaObject obj)
        {
            return obj.GetValue(RendererProperty);
        }

        public static void SetRenderer(AvaloniaObject obj, IShapeRenderer value)
        {
            obj.SetValue(RendererProperty, value);
        }

        public static readonly AttachedProperty<ISelection> SelectionProperty =
            AvaloniaProperty.RegisterAttached<RendererOptions, AvaloniaObject, ISelection>("Selection", null, true, BindingMode.TwoWay);

        public static ISelection GetSelection(AvaloniaObject obj)
        {
            return obj.GetValue(SelectionProperty);
        }

        public static void SetSelection(AvaloniaObject obj, ISelection value)
        {
            obj.SetValue(SelectionProperty, value);
        }

        public static readonly AttachedProperty<DataFlow> DataFlowProperty =
            AvaloniaProperty.RegisterAttached<RendererOptions, AvaloniaObject, DataFlow>("DataFlow", null, true, BindingMode.TwoWay);

        public static DataFlow GetDataFlow(AvaloniaObject obj)
        {
            return obj.GetValue(DataFlowProperty);
        }

        public static void SetDataFlow(AvaloniaObject obj, DataFlow value)
        {
            obj.SetValue(DataFlowProperty, value);
        }
    }
}
