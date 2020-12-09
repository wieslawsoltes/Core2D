using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core2D.Containers;
using Core2D.History;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    public class StyleEditorViewModel : ViewModelBase
    {
        private const NumberStyles _numberStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
        private readonly IServiceProvider _serviceProvider;
        private ShapeStyleViewModel _shapeStyleViewModelCopy;
        private BaseColorViewModel _strokeCopy;
        private BaseColorViewModel _fillCopy;
        private TextStyleViewModel _textStyleViewModelCopy;
        private ArrowStyleViewModel _endArrowStyleViewModelCopy;
        private ArrowStyleViewModel _startArrowStyleViewModelCopy;

        public StyleEditorViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public void OnCopyStyle()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            if (editor.PageStateViewModel?.SelectedShapes != null)
            {
                var style = editor.PageStateViewModel?.SelectedShapes.FirstOrDefault()?.StyleViewModel;
                _shapeStyleViewModelCopy = (ShapeStyleViewModel)style?.Copy(null);
            }
        }

        public void OnPasteStyle()
        {
            if (_shapeStyleViewModelCopy == null)
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            foreach (var shape in GetShapes(editor))
            {
                var previous = shape.StyleViewModel;
                var next = (ShapeStyleViewModel)_shapeStyleViewModelCopy?.Copy(null);
                editor.Project?.History?.Snapshot(previous, next, (p) => shape.StyleViewModel = p);
                shape.StyleViewModel = next;
            }
        }

        public void OnCopyStroke()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            if (editor.PageStateViewModel?.SelectedShapes != null)
            {
                var stroke = editor.PageStateViewModel?.SelectedShapes.FirstOrDefault()?.StyleViewModel?.Stroke?.ColorViewModel;
                _strokeCopy = (BaseColorViewModel)stroke?.Copy(null);
            }
        }

        public void OnPasteStroke()
        {
            if (_strokeCopy == null)
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            foreach (var shape in GetShapes(editor))
            {
                var style = shape.StyleViewModel;

                var previous = style.Stroke.ColorViewModel;
                var next = (BaseColorViewModel)_strokeCopy?.Copy(null);
                editor.Project?.History?.Snapshot(previous, next, (p) => style.Stroke.ColorViewModel = p);
                style.Stroke.ColorViewModel = next;
            }
        }

        public void OnCopyFill()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            if (editor.PageStateViewModel?.SelectedShapes != null)
            {
                var fill = editor.PageStateViewModel?.SelectedShapes.FirstOrDefault()?.StyleViewModel?.Fill?.ColorViewModel;
                _fillCopy = (BaseColorViewModel)fill?.Copy(null);
            }
        }

        public void OnPasteFill()
        {
            if (_fillCopy == null)
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            foreach (var shape in GetShapes(editor))
            {
                var style = shape.StyleViewModel;

                var previous = style.Fill.ColorViewModel;
                var next = (BaseColorViewModel)_fillCopy?.Copy(null);
                editor.Project?.History?.Snapshot(previous, next, (p) => style.Fill.ColorViewModel = p);
                style.Fill.ColorViewModel = next;
            }
        }

        public void OnCopyStartArrowStyle()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            if (editor.PageStateViewModel?.SelectedShapes != null)
            {
                var startArrowStyle = editor.PageStateViewModel?.SelectedShapes.FirstOrDefault()?.StyleViewModel?.Stroke?.StartArrowStyleViewModel;
                _startArrowStyleViewModelCopy = (ArrowStyleViewModel)startArrowStyle?.Copy(null);
            }
        }

        public void OnPasteStartArrowStyle()
        {
            if (_startArrowStyleViewModelCopy == null)
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            foreach (var shape in GetShapes(editor))
            {
                var style = shape.StyleViewModel;

                var previous = style.Stroke.StartArrowStyleViewModel;
                var next = (ArrowStyleViewModel)_startArrowStyleViewModelCopy?.Copy(null);
                editor.Project?.History?.Snapshot(previous, next, (p) => style.Stroke.StartArrowStyleViewModel = p);
                style.Stroke.StartArrowStyleViewModel = next;
            }
        }

        public void OnCopyEndArrowStyle()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            if (editor.PageStateViewModel?.SelectedShapes != null)
            {
                var endArrowStyle = editor.PageStateViewModel?.SelectedShapes.FirstOrDefault()?.StyleViewModel?.Stroke?.EndArrowStyleViewModel;
                _endArrowStyleViewModelCopy = (ArrowStyleViewModel)endArrowStyle?.Copy(null);
            }
        }

        public void OnPasteEndArrowStyle()
        {
            if (_endArrowStyleViewModelCopy == null)
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            foreach (var shape in GetShapes(editor))
            {
                var style = shape.StyleViewModel;

                var previous = style.Stroke.EndArrowStyleViewModel;
                var next = (ArrowStyleViewModel)_endArrowStyleViewModelCopy?.Copy(null);
                editor.Project?.History?.Snapshot(previous, next, (p) => style.Stroke.EndArrowStyleViewModel = p);
                style.Stroke.EndArrowStyleViewModel = next;
            }
        }

        public void OnCopyTextStyle()
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            if (editor.PageStateViewModel?.SelectedShapes != null)
            {
                var textStyle = editor.PageStateViewModel?.SelectedShapes.FirstOrDefault()?.StyleViewModel?.TextStyleViewModel;
                _textStyleViewModelCopy = (TextStyleViewModel)textStyle?.Copy(null);
            }
        }

        public void OnPasteTextStyle()
        {
            if (_textStyleViewModelCopy == null)
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();

            foreach (var shape in GetShapes(editor))
            {
                var style = shape.StyleViewModel;

                var previous = style.TextStyleViewModel;
                var next = (TextStyleViewModel)_textStyleViewModelCopy?.Copy(null);
                editor.Project?.History?.Snapshot(previous, next, (p) => style.TextStyleViewModel = p);
                style.TextStyleViewModel = next;
            }
        }

        private void SetThickness(BaseShapeViewModel shapeViewModel, double value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style != null)
            {
                var previous = style.Stroke.Thickness;
                var next = value;
                history?.Snapshot(previous, next, (p) => style.Stroke.Thickness = p);
                style.Stroke.Thickness = next;
            }
        }

        private void SetLineCap(BaseShapeViewModel shapeViewModel, LineCap value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style != null)
            {
                var previous = style.Stroke.LineCap;
                var next = value;
                history?.Snapshot(previous, next, (p) => style.Stroke.LineCap = p);
                style.Stroke.LineCap = next;
            }
        }

        private void SetDashes(BaseShapeViewModel shapeViewModel, string value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style != null)
            {
                var previous = style.Stroke.Dashes;
                var next = value;
                history?.Snapshot(previous, next, (p) => style.Stroke.Dashes = p);
                style.Stroke.Dashes = next;
            }
        }

        private void SetDashOffset(BaseShapeViewModel shapeViewModel, double value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style != null)
            {
                var previous = style.Stroke.DashOffset;
                var next = value;
                history?.Snapshot(previous, next, (p) => style.Stroke.DashOffset = p);
                style.Stroke.DashOffset = next;
            }
        }

        private void SetStroke(BaseShapeViewModel shapeViewModel, BaseColorViewModel value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style != null)
            {
                var previous = style.Stroke.ColorViewModel;
                var next = (BaseColorViewModel)value.Copy(null);
                history?.Snapshot(previous, next, (p) => style.Stroke.ColorViewModel = p);
                style.Stroke.ColorViewModel = next;
            }
        }

        private void SetStrokeTransparency(BaseShapeViewModel shapeViewModel, byte value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.Stroke?.ColorViewModel is ArgbColorViewModelViewModel argbColor)
            {
                var previous = argbColor.Value;
                var next = ArgbColorViewModelViewModel.ToUint32(value, argbColor.R, argbColor.G, argbColor.B);
                history?.Snapshot(previous, next, (p) => argbColor.Value = p);
                argbColor.Value = next;
            }
        }

        private void SetFill(BaseShapeViewModel shapeViewModel, BaseColorViewModel value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style != null)
            {
                var previous = style.Fill.ColorViewModel;
                var next = (BaseColorViewModel)value.Copy(null);
                history?.Snapshot(previous, next, (p) => style.Fill.ColorViewModel = p);
                style.Fill.ColorViewModel = next;
            }
        }

        private void SetFillTransparency(BaseShapeViewModel shapeViewModel, byte value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.Fill?.ColorViewModel is ArgbColorViewModelViewModel argbColor)
            {
                var previous = argbColor.Value;
                var next = ArgbColorViewModelViewModel.ToUint32(value, argbColor.R, argbColor.G, argbColor.B);
                history?.Snapshot(previous, next, (p) => argbColor.Value = p);
                argbColor.Value = next;
            }
        }

        private void SetFontName(BaseShapeViewModel shapeViewModel, string value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.TextStyleViewModel != null)
            {
                var textStyle = style.TextStyleViewModel;

                var previous = textStyle.FontName;
                var next = value;
                history?.Snapshot(previous, next, (p) => textStyle.FontName = p);
                textStyle.FontName = next;
            }
        }

        private void SetFontStyle(BaseShapeViewModel shapeViewModel, FontStyleFlags value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.TextStyleViewModel?.FontStyle != null)
            {
                var textStyle = style.TextStyleViewModel;

                var previous = textStyle.FontStyle;
                var next = textStyle.FontStyle ^ value;
                history?.Snapshot(previous, next, (p) => textStyle.FontStyle = p);
                textStyle.FontStyle = next;
            }
        }

        private void SetFontSize(BaseShapeViewModel shapeViewModel, double value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.TextStyleViewModel != null)
            {
                var textStyle = style.TextStyleViewModel;

                var previous = textStyle.FontSize;
                var next = value;
                history?.Snapshot(previous, next, (p) => textStyle.FontSize = p);
                textStyle.FontSize = next;
            }
        }

        private void SetTextHAlignment(BaseShapeViewModel shapeViewModel, TextHAlignment value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.TextStyleViewModel != null)
            {
                var textStyle = style.TextStyleViewModel;

                var previous = textStyle.TextHAlignment;
                var next = value;
                history?.Snapshot(previous, next, (p) => textStyle.TextHAlignment = p);
                textStyle.TextHAlignment = next;
            }
        }

        private void SetTextVAlignment(BaseShapeViewModel shapeViewModel, TextVAlignment value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.TextStyleViewModel != null)
            {
                var textStyle = style.TextStyleViewModel;

                var previous = textStyle.TextVAlignment;
                var next = value;
                history?.Snapshot(previous, next, (p) => textStyle.TextVAlignment = p);
                textStyle.TextVAlignment = next;
            }
        }

        private void SetStartArrowType(BaseShapeViewModel shapeViewModel, ArrowType value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.Stroke.StartArrowStyleViewModel != null)
            {
                var startArrowStyle = style.Stroke.StartArrowStyleViewModel;

                var previous = startArrowStyle.ArrowType;
                var next = value;
                history?.Snapshot(previous, next, (p) => startArrowStyle.ArrowType = p);
                startArrowStyle.ArrowType = next;
            }
        }

        private void SetStartArrowRadiusX(BaseShapeViewModel shapeViewModel, double value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.Stroke.StartArrowStyleViewModel != null)
            {
                var startArrowStyle = style.Stroke.StartArrowStyleViewModel;

                var previous = startArrowStyle.RadiusX;
                var next = value;
                history?.Snapshot(previous, next, (p) => startArrowStyle.RadiusX = p);
                startArrowStyle.RadiusX = next;
            }
        }

        private void SetStartArrowRadiusY(BaseShapeViewModel shapeViewModel, double value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.Stroke.StartArrowStyleViewModel != null)
            {
                var startArrowStyle = style.Stroke.StartArrowStyleViewModel;

                var previous = startArrowStyle.RadiusY;
                var next = value;
                history?.Snapshot(previous, next, (p) => startArrowStyle.RadiusY = p);
                startArrowStyle.RadiusY = next;
            }
        }

        private void SetEndArrowType(BaseShapeViewModel shapeViewModel, ArrowType value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.Stroke.EndArrowStyleViewModel != null)
            {
                var endArrowStyle = style.Stroke.EndArrowStyleViewModel;

                var previous = endArrowStyle.ArrowType;
                var next = value;
                history?.Snapshot(previous, next, (p) => endArrowStyle.ArrowType = p);
                endArrowStyle.ArrowType = next;
            }
        }

        private void SetEndArrowRadiusX(BaseShapeViewModel shapeViewModel, double value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.Stroke.EndArrowStyleViewModel != null)
            {
                var endArrowStyle = style.Stroke.EndArrowStyleViewModel;

                var previous = endArrowStyle.RadiusX;
                var next = value;
                history?.Snapshot(previous, next, (p) => endArrowStyle.RadiusX = p);
                endArrowStyle.RadiusX = next;
            }
        }

        private void SetEndArrowRadiusY(BaseShapeViewModel shapeViewModel, double value, IHistory history)
        {
            var style = shapeViewModel.StyleViewModel;
            if (style?.Stroke.EndArrowStyleViewModel != null)
            {
                var endArrowStyle = style.Stroke.EndArrowStyleViewModel;

                var previous = endArrowStyle.RadiusY;
                var next = value;
                history?.Snapshot(previous, next, (p) => endArrowStyle.RadiusY = p);
                endArrowStyle.RadiusY = next;
            }
        }

        private IEnumerable<BaseShapeViewModel> GetShapes(ProjectEditorViewModel editorViewModel)
        {
            if (editorViewModel.PageStateViewModel?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in editorViewModel.PageStateViewModel.SelectedShapes)
                {
                    if (shape is GroupShapeViewModel group)
                    {
                        var groupShapes = ProjectContainerViewModel.GetAllShapes(group.Shapes);
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

        public void OnStyleSetThickness(string thickness)
        {
            if (!double.TryParse(thickness, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetThickness(shape, value, history);
            }

            editor.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }

        public void OnStyleSetLineCap(string lineCap)
        {
            if (!Enum.TryParse<LineCap>(lineCap, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetLineCap(shape, value, history);
            }

            editor.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }

        public void OnStyleSetDashes(string dashes)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetDashes(shape, dashes, history);
            }

            editor.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }

        public void OnStyleSetDashOffset(string dashOffset)
        {
            if (!double.TryParse(dashOffset, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetDashOffset(shape, value, history);
            }

            editor.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }

        public void OnStyleSetStroke(string color)
        {
            BaseColorViewModel value;
            try
            {
                value = ArgbColorViewModelViewModel.Parse(color);
            }
            catch
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetStroke(shape, value, history);
            }

            editor.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }

        public void OnStyleSetStrokeTransparency(string alpha)
        {
            if (!byte.TryParse(alpha, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetStrokeTransparency(shape, value, history);
            }

            editor.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }

        public void OnStyleSetFill(string color)
        {
            BaseColorViewModel value;
            try
            {
                value = ArgbColorViewModelViewModel.Parse(color);
            }
            catch
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetFill(shape, value, history);
            }
        }

        public void OnStyleSetFillTransparency(string alpha)
        {
            if (!byte.TryParse(alpha, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetFillTransparency(shape, value, history);
            }

            editor.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }

        public void OnStyleSetFontName(string fontName)
        {
            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetFontName(shape, fontName, history);
            }
        }

        public void OnStyleSetFontSize(string fontSize)
        {
            if (!double.TryParse(fontSize, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetFontSize(shape, value, history);
            }
        }

        public void OnStyleSetFontStyle(string fontStyle)
        {
            if (!Enum.TryParse<FontStyleFlags>(fontStyle, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetFontStyle(shape, value, history);
            }
        }

        public void OnStyleSetTextHAlignment(string alignment)
        {
            if (!Enum.TryParse<TextHAlignment>(alignment, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetTextHAlignment(shape, value, history);
            }
        }

        public void OnStyleSetTextVAlignment(string alignment)
        {
            if (!Enum.TryParse<TextVAlignment>(alignment, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetTextVAlignment(shape, value, history);
            }
        }

        public void OnStyleSetStartArrowType(string type)
        {
            if (!Enum.TryParse<ArrowType>(type, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetStartArrowType(shape, value, history);
            }
        }

        public void OnStyleSetStartArrowRadiusX(string radius)
        {
            if (!double.TryParse(radius, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetStartArrowRadiusX(shape, value, history);
            }
        }

        public void OnStyleSetStartArrowRadiusY(string radius)
        {
            if (!double.TryParse(radius, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetStartArrowRadiusY(shape, value, history);
            }
        }

        public void OnStyleSetEndArrowType(string type)
        {
            if (!Enum.TryParse<ArrowType>(type, true, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetEndArrowType(shape, value, history);
            }

            editor.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }

        public void OnStyleSetEndArrowRadiusX(string radius)
        {
            if (!double.TryParse(radius, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetEndArrowRadiusX(shape, value, history);
            }

            editor.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }

        public void OnStyleSetEndArrowRadiusY(string radius)
        {
            if (!double.TryParse(radius, _numberStyles, CultureInfo.InvariantCulture, out var value))
            {
                return;
            }

            var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
            var history = editor.Project?.History;

            foreach (var shape in GetShapes(editor))
            {
                SetEndArrowRadiusY(shape, value, history);
            }

            editor.Project?.CurrentContainerViewModel?.InvalidateLayer();
        }
    }
}
