using Avalonia;
using Avalonia.Data;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.UI.Renderer
{
    /// <summary>
    /// Shape renderer avalonia attached properties.
    /// </summary>
    public class RendererOptions
    {
        /// <summary>
        /// Renderer options attached property.
        /// </summary>
        public static readonly AttachedProperty<IShapeRenderer> RendererProperty =
            AvaloniaProperty.RegisterAttached<RendererOptions, AvaloniaObject, IShapeRenderer>("Renderer", null, true, BindingMode.TwoWay);

        /// <summary>
        /// Gets renderer attached property.
        /// </summary>
        /// <param name="obj">The avalonia object.</param>
        /// <returns>The shape renderer property.</returns>
        public static IShapeRenderer GetRenderer(AvaloniaObject obj)
        {
            return obj.GetValue(RendererProperty);
        }

        /// <summary>
        /// Sets renderer attached property.
        /// </summary>
        /// <param name="obj">The avalonia object.</param>
        /// <param name="value">The shape render value.</param>
        public static void SetRenderer(AvaloniaObject obj, IShapeRenderer value)
        {
            obj.SetValue(RendererProperty, value);
        }

        /// <summary>
        /// DataFlow attached property.
        /// </summary>
        public static readonly AttachedProperty<DataFlow> DataFlowProperty =
            AvaloniaProperty.RegisterAttached<RendererOptions, AvaloniaObject, DataFlow>("DataFlow", null, true, BindingMode.TwoWay);

        /// <summary>
        /// Gets data flow attached property.
        /// </summary>
        /// <param name="obj">The avalonia object.</param>
        /// <returns>The data flow property.</returns>
        public static DataFlow GetDataFlow(AvaloniaObject obj)
        {
            return obj.GetValue(DataFlowProperty);
        }

        /// <summary>
        /// Sets data flow attached property.
        /// </summary>
        /// <param name="obj">The avalonia object.</param>
        /// <param name="value">The data flow value.</param>
        public static void SetDataFlow(AvaloniaObject obj, DataFlow value)
        {
            obj.SetValue(DataFlowProperty, value);
        }
    }
}
