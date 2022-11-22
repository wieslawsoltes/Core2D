﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core2D.Model.History;
using Core2D.Model.Style;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor;

public class StyleEditorViewModel : ViewModelBase
{
    private const NumberStyles NumberStyles = 
        System.Globalization.NumberStyles.AllowLeadingSign 
        | System.Globalization.NumberStyles.AllowDecimalPoint 
        | System.Globalization.NumberStyles.AllowThousands;

    private ShapeStyleViewModel? _shapeStyleCopy;
    private BaseColorViewModel? _strokeCopy;
    private BaseColorViewModel? _fillCopy;
    private TextStyleViewModel? _textStyleCopy;
    private ArrowStyleViewModel? _endArrowStyleCopy;
    private ArrowStyleViewModel? _startArrowStyleCopy;

    public StyleEditorViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    public void OnCopyStyle()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        if (editor.Project?.SelectedShapes is { })
        {
            var style = editor.Project?.SelectedShapes.FirstOrDefault()?.Style;
            _shapeStyleCopy = (ShapeStyleViewModel?)style?.Copy(null);
        }
    }

    public void OnPasteStyle()
    {
        if (_shapeStyleCopy is null)
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        foreach (var shape in GetShapes(editor))
        {
            var previous = shape.Style;
            var next = (ShapeStyleViewModel?)_shapeStyleCopy?.Copy(null);
            editor.Project?.History?.Snapshot(previous, next, (p) => shape.Style = p);
            shape.Style = next;
        }
    }

    public void OnCopyStroke()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        if (editor.Project?.SelectedShapes is { })
        {
            var stroke = editor.Project?.SelectedShapes.FirstOrDefault()?.Style?.Stroke?.Color;
            _strokeCopy = (BaseColorViewModel?)stroke?.Copy(null);
        }
    }

    public void OnPasteStroke()
    {
        if (_strokeCopy is null)
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        foreach (var shape in GetShapes(editor))
        {
            var style = shape.Style;
            if (style?.Stroke is null)
            {
                continue;
            }

            var previous = style.Stroke.Color;
            var next = (BaseColorViewModel?)_strokeCopy?.Copy(null);
            editor.Project?.History?.Snapshot(previous, next, (p) => style.Stroke.Color = p);
            style.Stroke.Color = next;
        }
    }

    public void OnCopyFill()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        if (editor.Project?.SelectedShapes is { })
        {
            var fill = editor.Project?.SelectedShapes.FirstOrDefault()?.Style?.Fill?.Color;
            _fillCopy = (BaseColorViewModel?)fill?.Copy(null);
        }
    }

    public void OnPasteFill()
    {
        if (_fillCopy is null)
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        foreach (var shape in GetShapes(editor))
        {
            var style = shape.Style;
            if (style?.Fill is null)
            {
                continue;
            }

            var previous = style.Fill.Color;
            var next = (BaseColorViewModel?)_fillCopy?.Copy(null);
            editor.Project?.History?.Snapshot(previous, next, (p) => style.Fill.Color = p);
            style.Fill.Color = next;
        }
    }

    public void OnCopyStartArrowStyle()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        if (editor.Project?.SelectedShapes is { })
        {
            var startArrowStyle = editor.Project?.SelectedShapes.FirstOrDefault()?.Style?.Stroke?.StartArrow;
            _startArrowStyleCopy = (ArrowStyleViewModel?)startArrowStyle?.Copy(null);
        }
    }

    public void OnPasteStartArrowStyle()
    {
        if (_startArrowStyleCopy is null)
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        foreach (var shape in GetShapes(editor))
        {
            var style = shape.Style;
            if (style?.Stroke is null)
            {
                continue;
            }

            var previous = style.Stroke.StartArrow;
            var next = (ArrowStyleViewModel?)_startArrowStyleCopy?.Copy(null);
            editor.Project?.History?.Snapshot(previous, next, (p) => style.Stroke.StartArrow = p);
            style.Stroke.StartArrow = next;
        }
    }

    public void OnCopyEndArrowStyle()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        if (editor.Project?.SelectedShapes is { })
        {
            var endArrowStyle = editor.Project?.SelectedShapes.FirstOrDefault()?.Style?.Stroke?.EndArrow;
            _endArrowStyleCopy = (ArrowStyleViewModel?)endArrowStyle?.Copy(null);
        }
    }

    public void OnPasteEndArrowStyle()
    {
        if (_endArrowStyleCopy is null)
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        foreach (var shape in GetShapes(editor))
        {
            var style = shape.Style;
            if (style?.Stroke is null)
            {
                continue;
            }

            var previous = style.Stroke.EndArrow;
            var next = (ArrowStyleViewModel?)_endArrowStyleCopy?.Copy(null);
            editor.Project?.History?.Snapshot(previous, next, (p) => style.Stroke.EndArrow = p);
            style.Stroke.EndArrow = next;
        }
    }

    public void OnCopyTextStyle()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        if (editor.Project?.SelectedShapes is { })
        {
            var textStyle = editor.Project?.SelectedShapes.FirstOrDefault()?.Style?.TextStyle;
            _textStyleCopy = (TextStyleViewModel?)textStyle?.Copy(null);
        }
    }

    public void OnPasteTextStyle()
    {
        if (_textStyleCopy is null)
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }

        foreach (var shape in GetShapes(editor))
        {
            var style = shape.Style;
            if (style?.TextStyle is null)
            {
                continue;
            }

            var previous = style.TextStyle;
            var next = (TextStyleViewModel?)_textStyleCopy?.Copy(null);
            editor.Project?.History?.Snapshot(previous, next, (p) => style.TextStyle = p);
            style.TextStyle = next;
        }
    }

    private void SetThickness(BaseShapeViewModel shape, double value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke is { })
        {
            var previous = style.Stroke.Thickness;
            var next = value;
            history?.Snapshot(previous, next, (p) => style.Stroke.Thickness = p);
            style.Stroke.Thickness = next;
        }
    }

    private void SetLineCap(BaseShapeViewModel shape, LineCap value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke is { })
        {
            var previous = style.Stroke.LineCap;
            var next = value;
            history?.Snapshot(previous, next, (p) => style.Stroke.LineCap = p);
            style.Stroke.LineCap = next;
        }
    }

    private void SetDashes(BaseShapeViewModel shape, string value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke is { })
        {
            var previous = style.Stroke.Dashes;
            var next = value;
            history?.Snapshot(previous, next, (p) => style.Stroke.Dashes = p);
            style.Stroke.Dashes = next;
        }
    }

    private void SetDashOffset(BaseShapeViewModel shape, double value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke is { })
        {
            var previous = style.Stroke.DashOffset;
            var next = value;
            history?.Snapshot(previous, next, (p) => style.Stroke.DashOffset = p);
            style.Stroke.DashOffset = next;
        }
    }

    private void SetStroke(BaseShapeViewModel shape, BaseColorViewModel value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke is { })
        {
            var previous = style.Stroke.Color;
            var next = (BaseColorViewModel)value.Copy(null);
            history?.Snapshot(previous, next, (p) => style.Stroke.Color = p);
            style.Stroke.Color = next;
        }
    }

    private void SetStrokeTransparency(BaseShapeViewModel shape, byte value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke?.Color is ArgbColorViewModel argbColor)
        {
            var previous = argbColor.Value;
            var next = ArgbColorViewModel.ToUint32(value, argbColor.R, argbColor.G, argbColor.B);
            history?.Snapshot(previous, next, (p) => argbColor.Value = p);
            argbColor.Value = next;
        }
    }

    private void SetFill(BaseShapeViewModel shape, BaseColorViewModel value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Fill is { })
        {
            var previous = style.Fill.Color;
            var next = (BaseColorViewModel)value.Copy(null);
            history?.Snapshot(previous, next, (p) => style.Fill.Color = p);
            style.Fill.Color = next;
        }
    }

    private void SetFillTransparency(BaseShapeViewModel shape, byte value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Fill?.Color is ArgbColorViewModel argbColor)
        {
            var previous = argbColor.Value;
            var next = ArgbColorViewModel.ToUint32(value, argbColor.R, argbColor.G, argbColor.B);
            history?.Snapshot(previous, next, (p) => argbColor.Value = p);
            argbColor.Value = next;
        }
    }

    private void SetFontName(BaseShapeViewModel shape, string value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.TextStyle is { })
        {
            var textStyle = style.TextStyle;

            var previous = textStyle.FontName;
            var next = value;
            history?.Snapshot(previous, next, (p) => textStyle.FontName = p);
            textStyle.FontName = next;
        }
    }

    private void SetFontStyle(BaseShapeViewModel shape, FontStyleFlags value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.TextStyle?.FontStyle is { })
        {
            var textStyle = style.TextStyle;

            var previous = textStyle.FontStyle;
            var next = textStyle.FontStyle ^ value;
            history?.Snapshot(previous, next, (p) => textStyle.FontStyle = p);
            textStyle.FontStyle = next;
        }
    }

    private void SetFontSize(BaseShapeViewModel shape, double value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.TextStyle is { })
        {
            var textStyle = style.TextStyle;

            var previous = textStyle.FontSize;
            var next = value;
            history?.Snapshot(previous, next, (p) => textStyle.FontSize = p);
            textStyle.FontSize = next;
        }
    }

    private void SetTextHAlignment(BaseShapeViewModel shape, TextHAlignment value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.TextStyle is { })
        {
            var textStyle = style.TextStyle;

            var previous = textStyle.TextHAlignment;
            var next = value;
            history?.Snapshot(previous, next, (p) => textStyle.TextHAlignment = p);
            textStyle.TextHAlignment = next;
        }
    }

    private void SetTextVAlignment(BaseShapeViewModel shape, TextVAlignment value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.TextStyle is { })
        {
            var textStyle = style.TextStyle;

            var previous = textStyle.TextVAlignment;
            var next = value;
            history?.Snapshot(previous, next, (p) => textStyle.TextVAlignment = p);
            textStyle.TextVAlignment = next;
        }
    }

    private void SetStartArrowType(BaseShapeViewModel shape, ArrowType value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke?.StartArrow is { })
        {
            var startArrowStyle = style.Stroke.StartArrow;

            var previous = startArrowStyle.ArrowType;
            var next = value;
            history?.Snapshot(previous, next, (p) => startArrowStyle.ArrowType = p);
            startArrowStyle.ArrowType = next;
        }
    }

    private void SetStartArrowRadiusX(BaseShapeViewModel shape, double value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke?.StartArrow is { })
        {
            var startArrowStyle = style.Stroke.StartArrow;

            var previous = startArrowStyle.RadiusX;
            var next = value;
            history?.Snapshot(previous, next, (p) => startArrowStyle.RadiusX = p);
            startArrowStyle.RadiusX = next;
        }
    }

    private void SetStartArrowRadiusY(BaseShapeViewModel shape, double value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke?.StartArrow is { })
        {
            var startArrowStyle = style.Stroke.StartArrow;

            var previous = startArrowStyle.RadiusY;
            var next = value;
            history?.Snapshot(previous, next, (p) => startArrowStyle.RadiusY = p);
            startArrowStyle.RadiusY = next;
        }
    }

    private void SetEndArrowType(BaseShapeViewModel shape, ArrowType value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke?.EndArrow is { })
        {
            var endArrowStyle = style.Stroke.EndArrow;

            var previous = endArrowStyle.ArrowType;
            var next = value;
            history?.Snapshot(previous, next, (p) => endArrowStyle.ArrowType = p);
            endArrowStyle.ArrowType = next;
        }
    }

    private void SetEndArrowRadiusX(BaseShapeViewModel shape, double value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke?.EndArrow is { })
        {
            var endArrowStyle = style.Stroke.EndArrow;

            var previous = endArrowStyle.RadiusX;
            var next = value;
            history?.Snapshot(previous, next, (p) => endArrowStyle.RadiusX = p);
            endArrowStyle.RadiusX = next;
        }
    }

    private void SetEndArrowRadiusY(BaseShapeViewModel shape, double value, IHistory? history)
    {
        var style = shape.Style;
        if (style?.Stroke?.EndArrow is { })
        {
            var endArrowStyle = style.Stroke.EndArrow;

            var previous = endArrowStyle.RadiusY;
            var next = value;
            history?.Snapshot(previous, next, (p) => endArrowStyle.RadiusY = p);
            endArrowStyle.RadiusY = next;
        }
    }

    private IEnumerable<BaseShapeViewModel> GetShapes(ProjectEditorViewModel editorViewModel)
    {
        if (editorViewModel.Project?.SelectedShapes?.Count > 0)
        {
            foreach (var shape in editorViewModel.Project.SelectedShapes)
            {
                if (shape is GroupShapeViewModel group)
                {
                    var groupShapes = group.Shapes.GetAllShapes();
                    foreach (var child in groupShapes)
                    {
                        yield return child;
                    }
                }
                else
                {
                    yield return shape;
                }
            }
        }
    }

    public void OnStyleSetThickness(object param)
    {
        if (param is not string thickness)
        {
            return;
        }

        if (!double.TryParse(thickness, NumberStyles, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetThickness(shape, value, history);
        }

        editor.Project?.CurrentContainer?.InvalidateLayer();
    }

    public void OnStyleSetLineCap(object param)
    {
        if (param is not string lineCap)
        {
            return;
        }

        if (!Enum.TryParse<LineCap>(lineCap, true, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetLineCap(shape, value, history);
        }

        editor.Project?.CurrentContainer?.InvalidateLayer();
    }

    public void OnStyleSetDashes(object param)
    {
        if (param is not string dashes)
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetDashes(shape, dashes, history);
        }

        editor.Project?.CurrentContainer?.InvalidateLayer();
    }

    public void OnStyleSetDashOffset(object param)
    {
        if (param is not string dashOffset)
        {
            return;
        }

        if (!double.TryParse(dashOffset, NumberStyles, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetDashOffset(shape, value, history);
        }

        editor.Project?.CurrentContainer?.InvalidateLayer();
    }

    public void OnStyleSetStroke(object param)
    {
        if (param is not string color)
        {
            return;
        }

        BaseColorViewModel value;
        try
        {
            ArgbColorViewModel.Parse(color, out var v);
            value = new ArgbColorViewModel(ServiceProvider)
            {
                Value = v
            };
        }
        catch
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetStroke(shape, value, history);
        }

        editor.Project?.CurrentContainer?.InvalidateLayer();
    }

    public void OnStyleSetStrokeTransparency(object param)
    {
        if (param is not string alpha)
        {
            return;
        }

        if (!byte.TryParse(alpha, NumberStyles, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetStrokeTransparency(shape, value, history);
        }

        editor.Project?.CurrentContainer?.InvalidateLayer();
    }

    public void OnStyleSetFill(object param)
    {
        if (param is not string color)
        {
            return;
        }

        BaseColorViewModel value;
        try
        {
            ArgbColorViewModel.Parse(color, out var v);
            value = new ArgbColorViewModel(ServiceProvider)
            {
                Value = v
            };
        }
        catch
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetFill(shape, value, history);
        }
    }

    public void OnStyleSetFillTransparency(object param)
    {
        if (param is not string alpha)
        {
            return;
        }

        if (!byte.TryParse(alpha, NumberStyles, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetFillTransparency(shape, value, history);
        }

        editor.Project?.CurrentContainer?.InvalidateLayer();
    }

    public void OnStyleSetFontName(object param)
    {
        if (param is not string fontName)
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetFontName(shape, fontName, history);
        }
    }

    public void OnStyleSetFontSize(object param)
    {
        if (param is not string fontSize)
        {
            return;
        }

        if (!double.TryParse(fontSize, NumberStyles, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetFontSize(shape, value, history);
        }
    }

    public void OnStyleSetFontStyle(object param)
    {
        if (param is not string fontStyle)
        {
            return;
        }

        if (!Enum.TryParse<FontStyleFlags>(fontStyle, true, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetFontStyle(shape, value, history);
        }
    }

    public void OnStyleSetTextHAlignment(object param)
    {
        if (param is not string alignment)
        {
            return;
        }

        if (!Enum.TryParse<TextHAlignment>(alignment, true, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
            
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetTextHAlignment(shape, value, history);
        }
    }

    public void OnStyleSetTextVAlignment(object param)
    {
        if (param is not string alignment)
        {
            return;
        }

        if (!Enum.TryParse<TextVAlignment>(alignment, true, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetTextVAlignment(shape, value, history);
        }
    }

    public void OnStyleSetStartArrowType(object param)
    {
        if (param is not string type)
        {
            return;
        }

        if (!Enum.TryParse<ArrowType>(type, true, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetStartArrowType(shape, value, history);
        }
    }

    public void OnStyleSetStartArrowRadiusX(object param)
    {
        if (param is not string radius)
        {
            return;
        }

        if (!double.TryParse(radius, NumberStyles, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetStartArrowRadiusX(shape, value, history);
        }
    }

    public void OnStyleSetStartArrowRadiusY(object param)
    {
        if (param is not string radius)
        {
            return;
        }

        if (!double.TryParse(radius, NumberStyles, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetStartArrowRadiusY(shape, value, history);
        }
    }

    public void OnStyleSetEndArrowType(object param)
    {
        if (param is not string type)
        {
            return;
        }

        if (!Enum.TryParse<ArrowType>(type, true, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetEndArrowType(shape, value, history);
        }

        editor.Project?.CurrentContainer?.InvalidateLayer();
    }

    public void OnStyleSetEndArrowRadiusX(object param)
    {
        if (param is not string radius)
        {
            return;
        }

        if (!double.TryParse(radius, NumberStyles, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetEndArrowRadiusX(shape, value, history);
        }

        editor.Project?.CurrentContainer?.InvalidateLayer();
    }

    public void OnStyleSetEndArrowRadiusY(object param)
    {
        if (param is not string radius)
        {
            return;
        }

        if (!double.TryParse(radius, NumberStyles, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is null)
        {
            return;
        }
        var history = editor.Project?.History;

        foreach (var shape in GetShapes(editor))
        {
            SetEndArrowRadiusY(shape, value, history);
        }

        editor.Project?.CurrentContainer?.InvalidateLayer();
    }
}
