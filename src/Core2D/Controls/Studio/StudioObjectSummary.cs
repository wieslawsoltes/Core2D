// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;

namespace Core2D.Controls.Studio;

/// <summary>A compact object identity row with a live name and explicit, non-reflected kind label.</summary>
public sealed class StudioObjectSummary : TemplatedControl
{
    /// <summary>Defines the original object described by this row.</summary>
    public static readonly DirectProperty<StudioObjectSummary, ViewModelBase?> SourceProperty =
        AvaloniaProperty.RegisterDirect<StudioObjectSummary, ViewModelBase?>(nameof(Source), x => x.Source, (x, value) => x.Source = value);
    /// <summary>Defines the current display title.</summary>
    public static readonly DirectProperty<StudioObjectSummary, string> TitleProperty =
        AvaloniaProperty.RegisterDirect<StudioObjectSummary, string>(nameof(Title), x => x.Title);
    /// <summary>Defines the explicit object kind.</summary>
    public static readonly DirectProperty<StudioObjectSummary, string> KindProperty =
        AvaloniaProperty.RegisterDirect<StudioObjectSummary, string>(nameof(Kind), x => x.Kind);
    private ViewModelBase? _source;
    private string _title = "Object", _kind = "Object";
    /// <summary>Initializes the row's subscription behavior.</summary>
    public StudioObjectSummary() => Interaction.GetBehaviors(this).Add(new StudioObjectSummaryBehavior());
    /// <summary>Gets or sets the original object.</summary>
    public ViewModelBase? Source { get => _source; set { if (SetAndRaise(SourceProperty, ref _source, value)) Refresh(); } }
    /// <summary>Gets the model name or a useful kind fallback.</summary>
    public string Title => _title;
    /// <summary>Gets the object kind, without reflection.</summary>
    public string Kind => _kind;
    internal void Refresh()
    {
        string kind = Source switch
        {
            DocumentContainerViewModel => "Document", PageContainerViewModel => "Page",
            TemplateContainerViewModel => "Template", LayerContainerViewModel => "Layer",
            BlockShapeViewModel => "Block", InsertShapeViewModel => "Block instance",
            RectangleShapeViewModel => "Rectangle", EllipseShapeViewModel => "Ellipse",
            TextShapeViewModel => "Text", ImageShapeViewModel => "Image", PathShapeViewModel => "Path",
            PointShapeViewModel => "Point", LineShapeViewModel => "Line", WireShapeViewModel => "Wire",
            ArcShapeViewModel => "Arc", CubicBezierShapeViewModel => "Cubic Bézier", QuadraticBezierShapeViewModel => "Quadratic Bézier",
            PathFigureViewModel => "Path figure", ArcSegmentViewModel => "Arc segment",
            CubicBezierSegmentViewModel => "Cubic segment", QuadraticBezierSegmentViewModel => "Quadratic segment",
            LineSegmentViewModel => "Line segment", _ => "Object"
        };
        SetAndRaise(KindProperty, ref _kind, kind);
        SetAndRaise(TitleProperty, ref _title, string.IsNullOrWhiteSpace(Source?.Name) ? kind : Source.Name);
    }
}
