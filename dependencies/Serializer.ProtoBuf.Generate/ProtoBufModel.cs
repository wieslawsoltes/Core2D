// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.History;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using ProtoBuf.Meta;

namespace Serializer.ProtoBuf
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
            var property = model.Add(typeof(XProperty), false);
            property.AsReferenceDefault = true;
            property.AddField(1, nameof(XProperty.Name));
            property.AddField(2, nameof(XProperty.Value)).AsReference = true;
            property.AddField(3, nameof(XProperty.Owner)).AsReference = true;

            var column = model.Add(typeof(XColumn), false);
            column.AsReferenceDefault = true;
            column.AddField(1, nameof(XColumn.Id));
            column.AddField(2, nameof(XColumn.Name));
            column.AddField(3, nameof(XColumn.Width));
            column.AddField(4, nameof(XColumn.IsVisible));
            column.AddField(5, nameof(XColumn.Owner)).AsReference = true;

            var value = model.Add(typeof(XValue), false);
            value.AsReferenceDefault = true;
            value.AddField(1, nameof(XValue.Content));

            var record = model.Add(typeof(XRecord), false);
            record.AsReferenceDefault = true;
            record.AddField(1, nameof(XRecord.Id));
            record.AddField(2, nameof(XRecord.Columns)).AsReference = true;
            record.AddField(3, nameof(XRecord.Values)).AsReference = true;
            record.AddField(4, nameof(XRecord.Owner)).AsReference = true;

            var data = model.Add(typeof(XContext), false);
            data.AsReferenceDefault = true;
            data.AddField(1, nameof(XContext.Properties)).AsReference = true;
            data.AddField(2, nameof(XContext.Record)).AsReference = true;

            var database = model.Add(typeof(XDatabase), false);
            database.AsReferenceDefault = true;
            database.AddField(1, nameof(XDatabase.Name));
            database.AddField(2, nameof(XDatabase.IdColumnName));
            database.AddField(3, nameof(XDatabase.Columns)).AsReference = true;
            database.AddField(4, nameof(XDatabase.Records)).AsReference = true;
            database.AddField(5, nameof(XDatabase.CurrentRecord)).AsReference = true;

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
            lineFixedLength.AddField(4, nameof(LineFixedLength.Length));

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
            shape.AddSubType(103, typeof(XCubicBezier));
            shape.AddSubType(104, typeof(XQuadraticBezier));
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

            // XCubicBezier : BaseShape
            model[typeof(XCubicBezier)].AddField(1, nameof(XCubicBezier.Point1)).AsReference = true;
            model[typeof(XCubicBezier)].AddField(2, nameof(XCubicBezier.Point2)).AsReference = true;
            model[typeof(XCubicBezier)].AddField(3, nameof(XCubicBezier.Point3)).AsReference = true;
            model[typeof(XCubicBezier)].AddField(4, nameof(XCubicBezier.Point4)).AsReference = true;

            // XQuadraticBezier : BaseShape
            model[typeof(XQuadraticBezier)].AddField(1, nameof(XQuadraticBezier.Point1)).AsReference = true;
            model[typeof(XQuadraticBezier)].AddField(2, nameof(XQuadraticBezier.Point2)).AsReference = true;
            model[typeof(XQuadraticBezier)].AddField(3, nameof(XQuadraticBezier.Point3)).AsReference = true;

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
            segment.AddSubType(102, typeof(XCubicBezierSegment));
            segment.AddSubType(103, typeof(XQuadraticBezierSegment));
            segment.AddSubType(104, typeof(XArcSegment));
            segment.AddSubType(105, typeof(XPolyLineSegment));
            segment.AddSubType(106, typeof(XPolyCubicBezierSegment));
            segment.AddSubType(107, typeof(XPolyQuadraticBezierSegment));

            // XLineSegment : XPathSegment
            model[typeof(XLineSegment)].AddField(1, nameof(XLineSegment.Point)).AsReference = true;

            // XCubicBezierSegment : XPathSegment
            model[typeof(XCubicBezierSegment)].AddField(1, nameof(XCubicBezierSegment.Point1)).AsReference = true;
            model[typeof(XCubicBezierSegment)].AddField(2, nameof(XCubicBezierSegment.Point2)).AsReference = true;
            model[typeof(XCubicBezierSegment)].AddField(3, nameof(XCubicBezierSegment.Point3)).AsReference = true;

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
            history.AddSubType(101, typeof(StackHistory));

            return model;
        }

        public static RuntimeTypeModel AddProject(this RuntimeTypeModel model)
        {
            var options = model.Add(typeof(XOptions), false);
            options.AsReferenceDefault = true;
            options.AddField(1, nameof(XOptions.SnapToGrid));
            options.AddField(2, nameof(XOptions.SnapX));
            options.AddField(3, nameof(XOptions.SnapY));
            options.AddField(4, nameof(XOptions.HitThreshold));
            options.AddField(5, nameof(XOptions.MoveMode));
            options.AddField(6, nameof(XOptions.DefaultIsStroked));
            options.AddField(7, nameof(XOptions.DefaultIsFilled));
            options.AddField(8, nameof(XOptions.DefaultIsClosed));
            options.AddField(9, nameof(XOptions.DefaultIsSmoothJoin));
            options.AddField(10, nameof(XOptions.DefaultFillRule));
            options.AddField(11, nameof(XOptions.TryToConnect));
            options.AddField(12, nameof(XOptions.PointShape)).AsReference = true;
            options.AddField(13, nameof(XOptions.PointStyle)).AsReference = true;
            options.AddField(14, nameof(XOptions.SelectionStyle)).AsReference = true;
            options.AddField(15, nameof(XOptions.HelperStyle)).AsReference = true;

            var shapeStyleLibrary = model.Add(typeof(XLibrary<ShapeStyle>), false);
            shapeStyleLibrary.AsReferenceDefault = true;
            shapeStyleLibrary.AddField(1, nameof(XLibrary<ShapeStyle>.Name));
            shapeStyleLibrary.AddField(2, nameof(XLibrary<ShapeStyle>.Items)).AsReference = true;
            shapeStyleLibrary.AddField(3, nameof(XLibrary<ShapeStyle>.Selected)).AsReference = true;

            var groupLibrary = model.Add(typeof(XLibrary<XGroup>), false);
            groupLibrary.AsReferenceDefault = true;
            groupLibrary.AddField(1, nameof(XLibrary<XGroup>.Name));
            groupLibrary.AddField(2, nameof(XLibrary<XGroup>.Items)).AsReference = true;
            groupLibrary.AddField(3, nameof(XLibrary<XGroup>.Selected)).AsReference = true;

            var selectable = model.Add(typeof(XSelectable), false);
            selectable.AsReferenceDefault = true;
            selectable.AddSubType(101, typeof(XLayer));
            selectable.AddSubType(102, typeof(XContainer));
            selectable.AddSubType(103, typeof(XDocument));
            selectable.AddSubType(104, typeof(XProject));

            // Layer : Selectable
            model[typeof(XLayer)].AddField(1, nameof(XLayer.Name));
            model[typeof(XLayer)].AddField(2, nameof(XLayer.Owner)).AsReference = true;
            model[typeof(XLayer)].AddField(3, nameof(XLayer.IsVisible));
            model[typeof(XLayer)].AddField(4, nameof(XLayer.Shapes)).AsReference = true;

            // Container : Selectable
            model[typeof(XContainer)].AddField(1, nameof(XContainer.Name));
            model[typeof(XContainer)].AddField(2, nameof(XContainer.Width));
            model[typeof(XContainer)].AddField(3, nameof(XContainer.Height));
            model[typeof(XContainer)].AddField(4, nameof(XContainer.Background)).AsReference = true;
            model[typeof(XContainer)].AddField(5, nameof(XContainer.Layers)).AsReference = true;
            model[typeof(XContainer)].AddField(6, nameof(XContainer.CurrentLayer)).AsReference = true;
            model[typeof(XContainer)].AddField(7, nameof(XContainer.WorkingLayer)).AsReference = true;
            model[typeof(XContainer)].AddField(8, nameof(XContainer.HelperLayer)).AsReference = true;
            model[typeof(XContainer)].AddField(9, nameof(XContainer.CurrentShape)).AsReference = true;
            model[typeof(XContainer)].AddField(10, nameof(XContainer.Template)).AsReference = true;
            model[typeof(XContainer)].AddSubType(101, typeof(XTemplate));
            model[typeof(XContainer)].AddSubType(102, typeof(XPage));

            // Template : Container

            // Page : Container
            model[typeof(XPage)].AddField(1, nameof(XPage.Data)).AsReference = true;
            model[typeof(XPage)].AddField(2, nameof(XPage.IsExpanded));

            // Document : Selectable
            model[typeof(XDocument)].AddField(1, nameof(XDocument.Name));
            model[typeof(XDocument)].AddField(2, nameof(XDocument.IsExpanded));
            model[typeof(XDocument)].AddField(3, nameof(XDocument.Pages)).AsReference = true;

            // Project : Selectable
            model[typeof(XProject)].AddField(1, nameof(XProject.Name));
            model[typeof(XProject)].AddField(2, nameof(XProject.Options)).AsReference = true;
            model[typeof(XProject)].AddField(3, nameof(XProject.History)).AsReference = true;
            model[typeof(XProject)].AddField(4, nameof(XProject.StyleLibraries)).AsReference = true;
            model[typeof(XProject)].AddField(5, nameof(XProject.GroupLibraries)).AsReference = true;
            model[typeof(XProject)].AddField(6, nameof(XProject.Databases)).AsReference = true;
            model[typeof(XProject)].AddField(7, nameof(XProject.Templates)).AsReference = true;
            model[typeof(XProject)].AddField(8, nameof(XProject.Documents)).AsReference = true;
            model[typeof(XProject)].AddField(9, nameof(XProject.CurrentStyleLibrary)).AsReference = true;
            model[typeof(XProject)].AddField(10, nameof(XProject.CurrentGroupLibrary)).AsReference = true;
            model[typeof(XProject)].AddField(11, nameof(XProject.CurrentDatabase)).AsReference = true;
            model[typeof(XProject)].AddField(12, nameof(XProject.CurrentTemplate)).AsReference = true;
            model[typeof(XProject)].AddField(13, nameof(XProject.CurrentDocument)).AsReference = true;
            model[typeof(XProject)].AddField(14, nameof(XProject.CurrentContainer)).AsReference = true;
            model[typeof(XProject)].AddField(15, nameof(XProject.Selected)).AsReference = true;

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
