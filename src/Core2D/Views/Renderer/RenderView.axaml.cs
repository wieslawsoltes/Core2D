#nullable enable
using System;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Core2D.Model.Renderer;
using Core2D.Model;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;

namespace Core2D.Views.Renderer;

public partial class RenderView : UserControl
{
    private CompositionCustomVisual? _customVisual;

    public static readonly StyledProperty<FrameContainerViewModel?> ContainerProperty =
        AvaloniaProperty.Register<RenderView, FrameContainerViewModel?>(nameof(Container));

    public static readonly StyledProperty<IShapeRenderer?> RendererProperty =
        AvaloniaProperty.Register<RenderView, IShapeRenderer?>(nameof(Renderer));

    public static readonly StyledProperty<ISelection?> SelectionProperty =
        AvaloniaProperty.Register<RenderView, ISelection?>(nameof(Selection));

    public static readonly StyledProperty<DataFlow?> DataFlowProperty =
        AvaloniaProperty.Register<RenderView, DataFlow?>(nameof(DataFlow));

    public static readonly StyledProperty<RenderType> RenderTypeProperty =
        AvaloniaProperty.Register<RenderView, RenderType>(nameof(RenderType));

    public FrameContainerViewModel? Container
    {
        get => GetValue(ContainerProperty);
        set => SetValue(ContainerProperty, value);
    }

    public IShapeRenderer? Renderer
    {
        get => GetValue(RendererProperty);
        set => SetValue(RendererProperty, value);
    }

    public ISelection? Selection
    {
        get => GetValue(SelectionProperty);
        set => SetValue(SelectionProperty, value);
    }

    public DataFlow? DataFlow
    {
        get => GetValue(DataFlowProperty);
        set => SetValue(DataFlowProperty, value);
    }

    public RenderType RenderType
    {
        get => GetValue(RenderTypeProperty);
        set => SetValue(RenderTypeProperty, value);
    }

    public RenderView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded()
    {
        base.OnLoaded();
        
        var elemVisual = ElementComposition.GetElementVisual(this);
        var compositor = elemVisual?.Compositor;
        if (compositor is null)
        {
            return;
        }
        
        _customVisual = compositor.CreateCustomVisual(new RenderCompositionCustomVisualHandler());
        ElementComposition.SetElementChildVisual(this, _customVisual);

        _customVisual.Size = new Vector2((float)Bounds.Size.Width, (float)Bounds.Size.Height);
        _customVisual.SendHandlerMessage(GetRenderState());

        LayoutUpdated += OnLayoutUpdated;
    }

    protected override void OnUnloaded()
    {
        base.OnUnloaded();

        LayoutUpdated -= OnLayoutUpdated;
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        if (_customVisual is { })
        {
            _customVisual.Size = new Vector2((float)Bounds.Size.Width, (float)Bounds.Size.Height);
            _customVisual.SendHandlerMessage(GetRenderState());
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // var customDrawOperation = new RenderDrawOperation
        // {
        //     RenderState = GetRenderState(),
        //     Bounds = Bounds
        // };
        // context.Custom(customDrawOperation);

        // RenderDrawOperation.Draw(GetRenderState(), context);

        // _customVisual?.SendHandlerMessage(GetRenderState());
    }

    private RenderState GetRenderState()
    {
        return new RenderState
        {
            Container = Container,
            Renderer = Renderer ?? GetValue(RendererOptions.RendererProperty),
            Selection = Selection ?? GetValue(RendererOptions.SelectionProperty),
            DataFlow = DataFlow ?? GetValue(RendererOptions.DataFlowProperty),
            RenderType = RenderType,
        };
    }

    public void Invalidate()
    {
        // InvalidateVisual();
        _customVisual?.SendHandlerMessage(GetRenderState());
    }
}
