// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.Model.Renderer;
using Core2D.Model.Style;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Designer;

public class DesignerContext
{
    public static ProjectEditorViewModel? Editor { get; private set; }

    public static TemplateContainerViewModel? Template { get; private set; }

    public static PageContainerViewModel? Page { get; private set; }

    public static DocumentContainerViewModel? Document { get; private set; }

    public static LayerContainerViewModel? Layer { get; private set; }

    public static OptionsViewModel? Options { get; private set; }

    public static ScriptViewModel? Script { get; private set; }

    public static ProjectContainerViewModel? Project { get; private set; }

    public static LibraryViewModel? CurrentStyleLibrary { get; private set; }

    public static LibraryViewModel? CurrentGroupLibrary { get; private set; }

    public static ShapeStateFlags State { get; private set; }

    public static DatabaseViewModel? Database { get; private set; }

    public static RecordViewModel? Record { get; private set; }

    public static ArgbColorViewModel? ArgbColor { get; private set; }

    public static ArrowStyleViewModel? ArrowStyle { get; private set; }

    public static FontStyleFlags FontStyle { get; private set; }

    public static ShapeStyleViewModel? ShapeStyle { get; private set; }

    public static StrokeStyleViewModel? StrokeStyle { get; private set; }

    public static FillStyleViewModel? FillStyle { get; private set; }

    public static TextStyleViewModel? TextStyle { get; private set; }

    public static ArcShapeViewModel? Arc { get; private set; }

    public static CubicBezierShapeViewModel? CubicBezier { get; private set; }

    public static EllipseShapeViewModel? Ellipse { get; private set; }

    public static BlockShapeViewModel? Group { get; private set; }

    public static ImageShapeViewModel? Image { get; private set; }

    public static LineShapeViewModel? Line { get; private set; }

    public static PathShapeViewModel? Path { get; private set; }

    public static PointShapeViewModel? Point { get; private set; }

    public static QuadraticBezierShapeViewModel? QuadraticBezier { get; private set; }

    public static RectangleShapeViewModel? Rectangle { get; private set; }

    public static TextShapeViewModel? Text { get; private set; }

    public static ArcSegmentViewModel? ArcSegment { get; private set; }

    public static CubicBezierSegmentViewModel? CubicBezierSegment { get; private set; }

    public static LineSegmentViewModel? LineSegment { get; private set; }

    public static PathFigureViewModel? PathFigure { get; private set; }

    public static PathSizeViewModel? PathSize { get; private set; }

    public static QuadraticBezierSegmentViewModel? QuadraticBezierSegment { get; private set; }

    public static ShapeRendererStateViewModel? ShapeRendererState { get; private set; }

    public static void InitializeContext(IServiceProvider? serviceProvider)
    {
        var factory = serviceProvider.GetService<IViewModelFactory>();
        var containerFactory = serviceProvider.GetService<IContainerFactory>();
        if (factory is null || containerFactory is null)
        {
            return;
        }

        // Editor

        Editor = serviceProvider.GetService<ProjectEditorViewModel>();
        if (Editor is { })
        {
            Editor.CurrentTool = Editor.Tools.FirstOrDefault(x => x.Title == "Selection");
            Editor.CurrentPathTool = Editor.PathTools.FirstOrDefault(x => x.Title == "Line");
        }
        
        // New Project

        Editor?.OnNewProject();

        // Data

        var db = factory.CreateDatabase("Db");
        var fields = new [] { "Column0", "Column1" };
        var columns = ImmutableArray.CreateRange(fields.Select(c => factory.CreateColumn(db, c)));
        db.Columns = columns;
        var values = Enumerable.Repeat("<empty>", db.Columns.Length).Select(c => factory.CreateValue(c));
        var record = factory.CreateRecord(
            db,
            ImmutableArray.CreateRange(values));
        db.Records = db.Records.Add(record);
        db.CurrentRecord = record;

        Database = db;
        Record = record;

        // Project

        Project = containerFactory.GetProject();

        Template = factory.CreateTemplateContainer();

        Page = factory.CreatePageContainer();
        var layer = Page.Layers.FirstOrDefault();
        if (layer is { })
        {
            layer.Shapes = layer.Shapes.Add(factory.CreateLineShape(0, 0, null));
            Page.CurrentLayer = layer;
            Page.CurrentShape = layer.Shapes.FirstOrDefault();
        }
        Page.Template = Template;

        Document = factory.CreateDocumentContainer();
        Layer = factory.CreateLayerContainer();
        Options = factory.CreateOptions();

        if (Project is { })
        {
            CurrentStyleLibrary = Project.CurrentStyleLibrary;
            CurrentGroupLibrary = Project.CurrentGroupLibrary;
        }

        // Scripting

        Script = factory.CreateScript();

        // State

        State = ShapeStateFlags.Default;

        // Style

        ArgbColor = factory.CreateArgbColor(128, 255);
        ArrowStyle = factory.CreateArrowStyle();
        FontStyle = FontStyleFlags.Regular;
        ShapeStyle = factory.CreateShapeStyle("Default");
        StrokeStyle = factory.CreateStrokeStyle();
        FillStyle = factory.CreateFillStyle();
        TextStyle = factory.CreateTextStyle();

        // Shapes

        Arc = factory.CreateArcShape(0, 0, ShapeStyle);
        CubicBezier = factory.CreateCubicBezierShape(0, 0, ShapeStyle);
        Ellipse = factory.CreateEllipseShape(0, 0, ShapeStyle);
        Group = factory.CreateBlockShape("Block");
        Image = factory.CreateImageShape(0, 0, ShapeStyle, "key");
        Line = factory.CreateLineShape(0, 0, ShapeStyle);
        Path = factory.CreatePathShape(ShapeStyle, ImmutableArray.Create<PathFigureViewModel>(), FillRule.EvenOdd);
        Point = factory.CreatePointShape();
        QuadraticBezier = factory.CreateQuadraticBezierShape(0, 0, ShapeStyle);
        Rectangle = factory.CreateRectangleShape(0, 0, ShapeStyle);
        Text = factory.CreateTextShape(0, 0, ShapeStyle, "Text");

        // Path

        ArcSegment = factory.CreateArcSegment(factory.CreatePointShape(), factory.CreatePathSize(), 180, true, SweepDirection.Clockwise);
        CubicBezierSegment = factory.CreateCubicBezierSegment(factory.CreatePointShape(), factory.CreatePointShape(), factory.CreatePointShape());
        LineSegment = factory.CreateLineSegment(factory.CreatePointShape());
        PathFigure = factory.CreatePathFigure(factory.CreatePointShape());
        PathSize = factory.CreatePathSize();
        QuadraticBezierSegment = factory.CreateQuadraticBezierSegment(factory.CreatePointShape(), factory.CreatePointShape());

        // Renderer

        ShapeRendererState = factory.CreateShapeRendererState();
    }
}
