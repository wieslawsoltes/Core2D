#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools
{
    public partial class ImageToolViewModel : ViewModelBase, IEditorTool
    {
        public enum State { TopLeft, BottomRight }
        private State _currentState = State.TopLeft;
        private ImageShapeViewModel? _image;
        private ImageSelection? _selection;

        public string Title => "Image";

        public ImageToolViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }

        public async void BeginDown(InputArgs args)
        {
            var factory = ServiceProvider.GetService<IViewModelFactory>();
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var selection = ServiceProvider.GetService<ISelectionService>();
            var imageImporter = ServiceProvider.GetService<IImageImporter>();
            var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();

            if (factory is null || editor?.Project?.Options is null || selection is null || viewModelFactory is null)
            {
                return;
            }

            (decimal sx, decimal sy) = selection.TryToSnap(args);
            switch (_currentState)
            {
                case State.TopLeft:
                    {
                        editor.IsToolIdle = false;

                        if (imageImporter is null)
                        {
                            editor.IsToolIdle = true;
                            return;
                        }

                        var key = await imageImporter.GetImageKeyAsync();
                        if (key is null || string.IsNullOrEmpty(key))
                        {
                            editor.IsToolIdle = true;
                            return;
                        }

                        var style = editor.Project.CurrentStyleLibrary?.Selected is { } ?
                            editor.Project.CurrentStyleLibrary.Selected :
                            viewModelFactory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
                        _image = factory.CreateImageShape(
                            (double)sx, (double)sy,
                            (ShapeStyleViewModel)style.Copy(null),
                            key,
                            false,
                            false);

                        editor.SetShapeName(_image);

                        var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                        if (result is { })
                        {
                            _image.TopLeft = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_image);
                        editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                        ToStateBottomRight();
                        Move(_image);
                        _currentState = State.BottomRight;
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_image is { })
                        {
                            _image.BottomRight.X = (double)sx;
                            _image.BottomRight.Y = (double)sy;

                            var result = selection.TryToGetConnectionPoint((double)sx, (double)sy);
                            if (result is { })
                            {
                                _image.BottomRight = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_image);
                            Finalize(_image);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _image);

                            Reset();
                        }
                    }
                    break;
            }
        }

        public void BeginUp(InputArgs args)
        {
        }

        public void EndDown(InputArgs args)
        {
            switch (_currentState)
            {
                case State.TopLeft:
                    break;
                case State.BottomRight:
                    Reset();
                    break;
            }
        }

        public void EndUp(InputArgs args)
        {
        }

        public void Move(InputArgs args)
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var selection = ServiceProvider.GetService<ISelectionService>();
            (decimal sx, decimal sy) = selection.TryToSnap(args);
            switch (_currentState)
            {
                case State.TopLeft:
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_image is { })
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                selection.TryToHoverShape((double)sx, (double)sy);
                            }
                            _image.BottomRight.X = (double)sx;
                            _image.BottomRight.Y = (double)sy;
                            editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                            Move(_image);
                        }
                    }
                    break;
            }
        }

        public void ToStateBottomRight()
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            _selection = new ImageSelection(
                ServiceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _image,
                editor.PageState.HelperStyle);

            _selection.ToStateBottomRight();
        }

        public void Move(BaseShapeViewModel shape)
        {
            _selection?.Move();
        }

        public void Finalize(BaseShapeViewModel shape)
        {
        }

        public void Reset()
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();

            if (editor is null)
            {
                return;
            }

            switch (_currentState)
            {
                case State.TopLeft:
                    break;
                case State.BottomRight:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_image);
                        editor.Project.CurrentContainer.WorkingLayer.RaiseInvalidateLayer();
                    }
                    break;
            }

            _currentState = State.TopLeft;

            if (_selection is { })
            {
                _selection.Reset();
                _selection = null;
            }

            editor.IsToolIdle = true;
        }
    }
}
