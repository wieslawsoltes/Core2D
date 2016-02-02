// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using ProtoBuf.Meta;
using Core2D;

namespace Dependencies
{
    public static class ProtoBufModel
    {
        public static RuntimeTypeModel AddCore(this RuntimeTypeModel model)
        {
            var observableObject = model.Add(typeof(ObservableObject), false);
            observableObject.AsReferenceDefault = true;

            var observableResource = model.Add(typeof(ObservableResource), false);
            observableResource.AsReferenceDefault = true;
            observableResource.AddField(1, nameof(ObservableResource.Resources)).AsReference = true;

            return model;
        }

        public static RuntimeTypeModel AddData(this RuntimeTypeModel model)
        {
            var property = model.Add(typeof(Property), false);
            property.AsReferenceDefault = true;
            property.AddField(1, nameof(Property.Name));
            property.AddField(2, nameof(Property.Value)).AsReference = true;
            property.AddField(3, nameof(Property.Owner)).AsReference = true;

            var column = model.Add(typeof(Column), false);
            column.AsReferenceDefault = true;
            column.AddField(1, nameof(Column.Id));
            column.AddField(2, nameof(Column.Name));
            column.AddField(3, nameof(Column.Width));
            column.AddField(4, nameof(Column.IsVisible));
            column.AddField(5, nameof(Column.Owner)).AsReference = true;

            var value = model.Add(typeof(Value), false);
            value.AsReferenceDefault = true;
            value.AddField(1, nameof(Value.Content));

            var record = model.Add(typeof(Record), false);
            record.AsReferenceDefault = true;
            record.AddField(1, nameof(Record.Id));
            record.AddField(2, nameof(Record.Columns)).AsReference = true;
            record.AddField(3, nameof(Record.Values)).AsReference = true;
            record.AddField(4, nameof(Record.Owner)).AsReference = true;

            var data = model.Add(typeof(Data), false);
            data.AsReferenceDefault = true;
            data.AddField(1, nameof(Data.Properties)).AsReference = true;
            data.AddField(2, nameof(Data.Record)).AsReference = true;

            var database = model.Add(typeof(Database), false);
            database.AsReferenceDefault = true;
            database.AddField(1, nameof(Database.Name));
            database.AddField(2, nameof(Database.IdColumnName));
            database.AddField(3, nameof(Database.Columns)).AsReference = true;
            database.AddField(4, nameof(Database.Records)).AsReference = true;
            database.AddField(5, nameof(Database.CurrentRecord)).AsReference = true;

            return model;
        }

        public static RuntimeTypeModel AddStyle(this RuntimeTypeModel model)
        {
            var argbColor = model.Add(typeof(ArgbColor), false);
            argbColor.AsReferenceDefault = true;
            argbColor.AddField(1, nameof(ArgbColor.A));
            argbColor.AddField(2, nameof(ArgbColor.R));
            argbColor.AddField(3, nameof(ArgbColor.G));
            argbColor.AddField(4, nameof(ArgbColor.B));

            var baseStyle = model.Add(typeof(BaseStyle), false);
            baseStyle.AsReferenceDefault = true;
            baseStyle.AddField(1, nameof(BaseStyle.Name));
            baseStyle.AddField(2, nameof(BaseStyle.Stroke)).AsReference = true;
            baseStyle.AddField(3, nameof(BaseStyle.Fill)).AsReference = true;
            baseStyle.AddField(4, nameof(BaseStyle.Thickness));
            baseStyle.AddField(5, nameof(BaseStyle.LineCap));
            baseStyle.AddField(6, nameof(BaseStyle.Dashes));
            baseStyle.AddField(7, nameof(BaseStyle.DashOffset));
            baseStyle.AddSubType(101, typeof(ArrowStyle));
            baseStyle.AddSubType(102, typeof(ShapeStyle));

            // ArrowStyle : BaseStyle
            model[typeof(ArrowStyle)].AddField(1, nameof(ArrowStyle.ArrowType));
            model[typeof(ArrowStyle)].AddField(2, nameof(ArrowStyle.IsStroked));
            model[typeof(ArrowStyle)].AddField(3, nameof(ArrowStyle.IsFilled));
            model[typeof(ArrowStyle)].AddField(4, nameof(ArrowStyle.RadiusX));
            model[typeof(ArrowStyle)].AddField(5, nameof(ArrowStyle.RadiusY));

            // ShapeStyle : BaseStyle
            model[typeof(ShapeStyle)].AddField(1, nameof(ShapeStyle.LineStyle)).AsReference = true;
            model[typeof(ShapeStyle)].AddField(2, nameof(ShapeStyle.StartArrowStyle)).AsReference = true;
            model[typeof(ShapeStyle)].AddField(3, nameof(ShapeStyle.EndArrowStyle)).AsReference = true;
            model[typeof(ShapeStyle)].AddField(4, nameof(ShapeStyle.TextStyle)).AsReference = true;

            var lineFixedLength = model.Add(typeof(LineFixedLength), false);
            lineFixedLength.AsReferenceDefault = true;
            lineFixedLength.AddField(1, nameof(LineFixedLength.Flags));
            lineFixedLength.AddField(2, nameof(LineFixedLength.StartTrigger)).AsReference = true;
            lineFixedLength.AddField(3, nameof(LineFixedLength.EndTrigger)).AsReference = true;

            var lineStyle = model.Add(typeof(LineStyle), false);
            lineStyle.AsReferenceDefault = true;
            lineStyle.AddField(1, nameof(LineStyle.Name));
            lineStyle.AddField(2, nameof(LineStyle.FixedLength)).AsReference = true;

            var fontStyle = model.Add(typeof(FontStyle), false);
            fontStyle.AsReferenceDefault = true;
            fontStyle.AddField(1, nameof(FontStyle.Flags));

            var textStyle = model.Add(typeof(TextStyle), false);
            textStyle.AsReferenceDefault = true;
            textStyle.AddField(1, nameof(TextStyle.Name));
            textStyle.AddField(2, nameof(TextStyle.FontName));
            textStyle.AddField(3, nameof(TextStyle.FontFile));
            textStyle.AddField(4, nameof(TextStyle.FontSize));
            textStyle.AddField(5, nameof(TextStyle.FontStyle)).AsReference = true;
            textStyle.AddField(6, nameof(TextStyle.TextHAlignment));
            textStyle.AddField(7, nameof(TextStyle.TextVAlignment));

            return model;
        }

        public static RuntimeTypeModel AddShapes(this RuntimeTypeModel model)
        {
            var state = model.Add(typeof(ShapeState), false);
            state.AsReferenceDefault = true;
            state.AddField(1, nameof(ShapeState.Flags));

            var shape = model.Add(typeof(BaseShape), false);
            shape.AsReferenceDefault = true;
            shape.AddField(1, nameof(BaseShape.Name));
            shape.AddField(2, nameof(BaseShape.Owner)).AsReference = true;
            shape.AddField(3, nameof(BaseShape.State)).AsReference = true;
            shape.AddField(4, nameof(BaseShape.Style)).AsReference = true;
            shape.AddField(5, nameof(BaseShape.IsStroked));
            shape.AddField(6, nameof(BaseShape.IsFilled));
            shape.AddField(7, nameof(BaseShape.Data)).AsReference = true;
            shape.AddSubType(101, typeof(XPoint));
            shape.AddSubType(102, typeof(XLine));
            shape.AddSubType(103, typeof(XBezier));
            shape.AddSubType(104, typeof(XQBezier));
            shape.AddSubType(105, typeof(XArc));
            shape.AddSubType(106, typeof(XPath));
            shape.AddSubType(107, typeof(XText));
            shape.AddSubType(108, typeof(XGroup));

            // XPoint : BaseShape
            model[typeof(XPoint)].AddField(1, nameof(XPoint.X));
            model[typeof(XPoint)].AddField(2, nameof(XPoint.Y));
            model[typeof(XPoint)].AddField(3, nameof(XPoint.Shape)).AsReference = true;

            // XLine : BaseShape
            model[typeof(XLine)].AddField(1, nameof(XLine.Start)).AsReference = true;
            model[typeof(XLine)].AddField(2, nameof(XLine.End)).AsReference = true;

            // XText : BaseShape
            model[typeof(XText)].AddField(1, nameof(XText.TopLeft)).AsReference = true;
            model[typeof(XText)].AddField(2, nameof(XText.BottomRight)).AsReference = true;
            model[typeof(XText)].AddField(3, nameof(XText.Text));
            model[typeof(XText)].AddSubType(101, typeof(XRectangle));
            model[typeof(XText)].AddSubType(102, typeof(XEllipse));
            model[typeof(XText)].AddSubType(103, typeof(XImage));

            // XRectangle : BaseShape
            model[typeof(XRectangle)].AddField(1, nameof(XRectangle.IsGrid));
            model[typeof(XRectangle)].AddField(2, nameof(XRectangle.OffsetX));
            model[typeof(XRectangle)].AddField(3, nameof(XRectangle.OffsetY));
            model[typeof(XRectangle)].AddField(4, nameof(XRectangle.CellWidth));
            model[typeof(XRectangle)].AddField(5, nameof(XRectangle.CellHeight));

            // XEllipse : BaseShape

            // XImage : BaseShape
            model[typeof(XImage)].AddField(1, nameof(XImage.Key));

            // XBezier : BaseShape
            model[typeof(XBezier)].AddField(1, nameof(XBezier.Point1)).AsReference = true;
            model[typeof(XBezier)].AddField(2, nameof(XBezier.Point2)).AsReference = true;
            model[typeof(XBezier)].AddField(3, nameof(XBezier.Point3)).AsReference = true;
            model[typeof(XBezier)].AddField(4, nameof(XBezier.Point4)).AsReference = true;

            // XQBezier : BaseShape
            model[typeof(XQBezier)].AddField(1, nameof(XQBezier.Point1)).AsReference = true;
            model[typeof(XQBezier)].AddField(2, nameof(XQBezier.Point2)).AsReference = true;
            model[typeof(XQBezier)].AddField(3, nameof(XQBezier.Point3)).AsReference = true;

            // XArc : BaseShape
            model[typeof(XArc)].AddField(1, nameof(XArc.Point1)).AsReference = true;
            model[typeof(XArc)].AddField(2, nameof(XArc.Point2)).AsReference = true;
            model[typeof(XArc)].AddField(3, nameof(XArc.Point3)).AsReference = true;
            model[typeof(XArc)].AddField(4, nameof(XArc.Point4)).AsReference = true;

            // XPath : BaseShape
            model[typeof(XPath)].AddField(1, nameof(XPath.Geometry)).AsReference = true;

            // XGroup : BaseShape
            model[typeof(XGroup)].AddField(1, nameof(XGroup.Shapes)).AsReference = true;
            model[typeof(XGroup)].AddField(2, nameof(XGroup.Connectors)).AsReference = true;

            // XPathGeometry
            var geometry = model.Add(typeof(XPathGeometry), false);
            geometry.AsReferenceDefault = true;
            geometry.AddField(1, nameof(XPathGeometry.Figures)).AsReference = true;
            geometry.AddField(2, nameof(XPathGeometry.FillRule));

            // XPathFigure
            var figure = model.Add(typeof(XPathFigure), false);
            figure.AsReferenceDefault = true;
            figure.AddField(1, nameof(XPathFigure.StartPoint)).AsReference = true;
            figure.AddField(2, nameof(XPathFigure.Segments)).AsReference = true;
            figure.AddField(3, nameof(XPathFigure.IsFilled));
            figure.AddField(4, nameof(XPathFigure.IsClosed));

            // XPathSize
            var size = model.Add(typeof(XPathSize), false);
            size.AsReferenceDefault = true;
            size.AddField(1, nameof(XPathSize.Width));
            size.AddField(2, nameof(XPathSize.Height));

            // XPathSegment
            var segment = model.Add(typeof(XPathSegment), false);
            segment.AsReferenceDefault = true;
            segment.AddField(1, nameof(XPathSegment.IsStroked));
            segment.AddField(2, nameof(XPathSegment.IsSmoothJoin));
            segment.AddSubType(101, typeof(XLineSegment));
            segment.AddSubType(102, typeof(XBezierSegment));
            segment.AddSubType(103, typeof(XQuadraticBezierSegment));
            segment.AddSubType(104, typeof(XArcSegment));
            segment.AddSubType(105, typeof(XPolyLineSegment));
            segment.AddSubType(106, typeof(XPolyCubicBezierSegment));
            segment.AddSubType(107, typeof(XPolyQuadraticBezierSegment));

            // XLineSegment : XPathSegment
            model[typeof(XLineSegment)].AddField(1, nameof(XLineSegment.Point)).AsReference = true;

            // XBezierSegment : XPathSegment
            model[typeof(XBezierSegment)].AddField(1, nameof(XBezierSegment.Point1)).AsReference = true;
            model[typeof(XBezierSegment)].AddField(2, nameof(XBezierSegment.Point2)).AsReference = true;
            model[typeof(XBezierSegment)].AddField(3, nameof(XBezierSegment.Point3)).AsReference = true;

            // XQuadraticBezierSegment : XPathSegment
            model[typeof(XQuadraticBezierSegment)].AddField(1, nameof(XQuadraticBezierSegment.Point1)).AsReference = true;
            model[typeof(XQuadraticBezierSegment)].AddField(2, nameof(XQuadraticBezierSegment.Point2)).AsReference = true;

            // XArcSegment : XPathSegment
            model[typeof(XArcSegment)].AddField(1, nameof(XArcSegment.Point)).AsReference = true;
            model[typeof(XArcSegment)].AddField(2, nameof(XArcSegment.Size)).AsReference = true;
            model[typeof(XArcSegment)].AddField(3, nameof(XArcSegment.RotationAngle));
            model[typeof(XArcSegment)].AddField(4, nameof(XArcSegment.IsLargeArc));
            model[typeof(XArcSegment)].AddField(5, nameof(XArcSegment.SweepDirection));

            // XPolyLineSegment : XPathSegment
            model[typeof(XPolyLineSegment)].AddField(1, nameof(XPolyLineSegment.Points)).AsReference = true;

            // XPolyCubicBezierSegment : XPathSegment
            model[typeof(XPolyCubicBezierSegment)].AddField(1, nameof(XPolyCubicBezierSegment.Points)).AsReference = true;

            // XPolyQuadraticBezierSegment : XPathSegment
            model[typeof(XPolyQuadraticBezierSegment)].AddField(1, nameof(XPolyQuadraticBezierSegment.Points)).AsReference = true;

            return model;
        }

        public static RuntimeTypeModel AddHistory(this RuntimeTypeModel model)
        {
            var history = model.Add(typeof(IHistory), false);
            history.AsReferenceDefault = true;
            history.AddSubType(101, typeof(History));

            return model;
        }

        public static RuntimeTypeModel AddProject(this RuntimeTypeModel model)
        {
            var options = model.Add(typeof(Options), false);
            options.AsReferenceDefault = true;
            options.AddField(1, nameof(Options.SnapToGrid));
            options.AddField(2, nameof(Options.SnapX));
            options.AddField(3, nameof(Options.SnapY));
            options.AddField(4, nameof(Options.HitThreshold));
            options.AddField(5, nameof(Options.MoveMode));
            options.AddField(6, nameof(Options.DefaultIsStroked));
            options.AddField(7, nameof(Options.DefaultIsFilled));
            options.AddField(8, nameof(Options.DefaultIsClosed));
            options.AddField(9, nameof(Options.DefaultIsSmoothJoin));
            options.AddField(10, nameof(Options.DefaultFillRule));
            options.AddField(11, nameof(Options.TryToConnect));
            options.AddField(12, nameof(Options.PointShape)).AsReference = true;
            options.AddField(13, nameof(Options.PointStyle)).AsReference = true;
            options.AddField(14, nameof(Options.SelectionStyle)).AsReference = true;
            options.AddField(15, nameof(Options.HelperStyle)).AsReference = true;

            var shapeStyleLibrary = model.Add(typeof(Library<ShapeStyle>), false);
            shapeStyleLibrary.AsReferenceDefault = true;
            shapeStyleLibrary.AddField(1, nameof(Library<ShapeStyle>.Name));
            shapeStyleLibrary.AddField(2, nameof(Library<ShapeStyle>.Items)).AsReference = true;
            shapeStyleLibrary.AddField(3, nameof(Library<ShapeStyle>.Selected)).AsReference = true;

            var groupLibrary = model.Add(typeof(Library<XGroup>), false);
            groupLibrary.AsReferenceDefault = true;
            groupLibrary.AddField(1, nameof(Library<XGroup>.Name));
            groupLibrary.AddField(2, nameof(Library<XGroup>.Items)).AsReference = true;
            groupLibrary.AddField(3, nameof(Library<XGroup>.Selected)).AsReference = true;

            var selectable = model.Add(typeof(Selectable), false);
            selectable.AsReferenceDefault = true;
            selectable.AddSubType(101, typeof(Layer));
            selectable.AddSubType(102, typeof(Container));
            selectable.AddSubType(103, typeof(Document));
            selectable.AddSubType(104, typeof(Project));

            // Layer : Selectable
            model[typeof(Layer)].AddField(1, nameof(Layer.Name));
            model[typeof(Layer)].AddField(2, nameof(Layer.Owner)).AsReference = true;
            model[typeof(Layer)].AddField(3, nameof(Layer.IsVisible));
            model[typeof(Layer)].AddField(4, nameof(Layer.Shapes)).AsReference = true;

            // Container : Selectable
            model[typeof(Container)].AddField(1, nameof(Container.Name));
            model[typeof(Container)].AddField(2, nameof(Container.Width));
            model[typeof(Container)].AddField(3, nameof(Container.Height));
            model[typeof(Container)].AddField(4, nameof(Container.Background)).AsReference = true;
            model[typeof(Container)].AddField(5, nameof(Container.Layers)).AsReference = true;
            model[typeof(Container)].AddField(6, nameof(Container.CurrentLayer)).AsReference = true;
            model[typeof(Container)].AddField(7, nameof(Container.WorkingLayer)).AsReference = true;
            model[typeof(Container)].AddField(8, nameof(Container.HelperLayer)).AsReference = true;
            model[typeof(Container)].AddField(9, nameof(Container.CurrentShape)).AsReference = true;
            model[typeof(Container)].AddField(10, nameof(Container.Template)).AsReference = true;
            model[typeof(Container)].AddSubType(101, typeof(Template));
            model[typeof(Container)].AddSubType(102, typeof(Page));

            // Template : Container

            // Page : Container
            model[typeof(Page)].AddField(1, nameof(Page.Data)).AsReference = true;
            model[typeof(Page)].AddField(2, nameof(Page.IsExpanded));

            // Document : Selectable
            model[typeof(Document)].AddField(1, nameof(Document.Name));
            model[typeof(Document)].AddField(2, nameof(Document.IsExpanded));
            model[typeof(Document)].AddField(3, nameof(Document.Pages)).AsReference = true;

            // Project : Selectable
            model[typeof(Project)].AddField(1, nameof(Project.Name));
            model[typeof(Project)].AddField(2, nameof(Project.Options)).AsReference = true;
            model[typeof(Project)].AddField(3, nameof(Project.History)).AsReference = true;
            model[typeof(Project)].AddField(4, nameof(Project.StyleLibraries)).AsReference = true;
            model[typeof(Project)].AddField(5, nameof(Project.GroupLibraries)).AsReference = true;
            model[typeof(Project)].AddField(6, nameof(Project.Databases)).AsReference = true;
            model[typeof(Project)].AddField(7, nameof(Project.Templates)).AsReference = true;
            model[typeof(Project)].AddField(8, nameof(Project.Documents)).AsReference = true;
            model[typeof(Project)].AddField(9, nameof(Project.CurrentStyleLibrary)).AsReference = true;
            model[typeof(Project)].AddField(10, nameof(Project.CurrentGroupLibrary)).AsReference = true;
            model[typeof(Project)].AddField(11, nameof(Project.CurrentDatabase)).AsReference = true;
            model[typeof(Project)].AddField(12, nameof(Project.CurrentTemplate)).AsReference = true;
            model[typeof(Project)].AddField(13, nameof(Project.CurrentDocument)).AsReference = true;
            model[typeof(Project)].AddField(14, nameof(Project.CurrentContainer)).AsReference = true;
            model[typeof(Project)].AddField(15, nameof(Project.Selected)).AsReference = true;

            return model;
        }

        public static RuntimeTypeModel Empty()
        {
            return TypeModel.Create();
        }

        public static RuntimeTypeModel ForProject()
        {
            return Empty()
                .AddCore()
                .AddData()
                .AddStyle()
                .AddShapes()
                .AddHistory()
                .AddProject();
        }
    }
}
