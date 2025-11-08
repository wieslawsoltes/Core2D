// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Avalonia;
using System;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Rendering;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;

namespace Core2D.Views.Renderer;

public partial class RenderView : UserControl
{
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

    private IRenderHost[] _renderHosts;

    public RenderView()
    {
        InitializeComponent();
        _renderHosts = CreateHosts();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_renderHosts.Length == 0)
        {
            _renderHosts = CreateHosts();
        }

        var renderer = Renderer ?? GetValue(RendererOptions.RendererProperty);
        if (renderer is null)
        {
            return;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var scaling = topLevel.RenderScaling;
        var pixelSize = PixelSize.FromSize(Bounds.Size, scaling);
        if (pixelSize.Width <= 0 || pixelSize.Height <= 0)
        {
            return;
        }

        var destination = new Rect(Bounds.Size);
        var host = ResolveHost(renderer);
        object renderContext;

        try
        {
            renderContext = host.BeginRender(renderer, context, pixelSize, scaling);
        }
        catch
        {
            host = _renderHosts[^1];
            renderContext = host.BeginRender(renderer, context, pixelSize, scaling);
        }

        var drawState = new RenderState
        {
            Container = Container,
            Renderer = renderer,
            Selection = Selection ?? GetValue(RendererOptions.SelectionProperty),
            DataFlow = DataFlow ?? GetValue(RendererOptions.DataFlowProperty),
            RenderType = RenderType,
        };

        RenderDrawOperation.Draw(drawState, renderContext);
        host.EndRender(renderer, renderContext, context, pixelSize, destination);
    }

    private IRenderHost ResolveHost(IShapeRenderer renderer)
    {
        foreach (var host in _renderHosts)
        {
            if (host.Supports(renderer))
            {
                return host;
            }
        }

        return _renderHosts[^1];
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        foreach (var host in _renderHosts)
        {
            host.Dispose();
        }
        _renderHosts = Array.Empty<IRenderHost>();
    }

    private static IRenderHost[] CreateHosts()
        => new IRenderHost[]
        {
            new SparseStripsRenderHost(),
            new VelloSharpRenderHost(),
            new ImmediateRenderHost()
        };
}
