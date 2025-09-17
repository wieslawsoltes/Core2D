#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.ViewModels.Editor.Tools.Selection;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools;

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

    private async Task NextPoint(InputArgs args)
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

        var (sx, sy) = selection.TryToSnap(args);
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

                var style = editor.Project.CurrentStyleLibrary?.Selected is { }
                    ? editor.Project.CurrentStyleLibrary.Selected
                    : viewModelFactory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);

                selection.ClearConnectionPoints();

                _image = factory.CreateImageShape(
                    (double) sx, (double) sy,
                    (ShapeStyleViewModel) style.Copy(null),
                    key);

                editor.SetShapeName(_image);

                var result = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                if (result is { })
                {
                    _image.TopLeft = result;
                }

                if (editor.Project.CurrentContainer?.WorkingLayer is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes =
                        editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_image);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }

                ToStateBottomRight();
                Move(_image);
                _currentState = State.BottomRight;
                break;
            }
            case State.BottomRight:
            {
                if (_image is { })
                {
                    if (_image.BottomRight is { })
                    {
                        _image.BottomRight.X = (double)sx;
                        _image.BottomRight.Y = (double)sy;
                    }

                    var result = selection.TryToGetConnectionPoint((double) sx, (double) sy);
                    if (result is { })
                    {
                        _image.BottomRight = result;
                    }

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes =
                            editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_image);
                    }

                    Finalize(_image);

                    if (editor.Project.CurrentContainer?.WorkingLayer is { })
                    {
                        editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _image);
                    }

                    Reset();
                }

                break;
            }
        }
    }

    public async void BeginDown(InputArgs args)
    {
        await NextPoint(args);
    }

    public async void BeginUp(InputArgs args)
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor?.Project is null)
        {
            return;
        }

        if (editor.Project.Options?.SinglePressMode ?? true)
        {
            if (_currentState != State.TopLeft)
            {
                await NextPoint(args);
            }
        }
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
        if (editor?.Project?.Options is null || selection is null)
        {
            return;
        }
        var (sx, sy) = selection.TryToSnap(args);
        switch (_currentState)
        {
            case State.TopLeft:
            {
                if (editor.Project.Options.TryToConnect)
                {
                    selection.TryToHoverShape((double)sx, (double)sy);
                }
                break;
            }
            case State.BottomRight:
            {
                if (_image is { })
                {
                    if (editor.Project.Options.TryToConnect)
                    {
                        selection.TryToHoverShape((double)sx, (double)sy);
                    }
                    if (_image.BottomRight is { })
                    {
                        _image.BottomRight.X = (double)sx;
                        _image.BottomRight.Y = (double)sy;
                    }
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                    Move(_image);
                }
                break;
            }
        }
    }

    public void ToStateBottomRight()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is { }
            && editor.Project?.CurrentContainer?.HelperLayer is { }
            && editor.PageState?.HelperStyle is { }
            && _image is { })
        {
            _selection = new ImageSelection(
                ServiceProvider,
                editor.Project.CurrentContainer.HelperLayer,
                _image,
                editor.PageState.HelperStyle);

            _selection.ToStateBottomRight();
        }
    }

    public void Move(BaseShapeViewModel? shape)
    {
        _selection?.Move();
    }

    public void Finalize(BaseShapeViewModel? shape)
    {
    }

    public void Reset()
    {
        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor?.Project is null)
        {
            return;
        }

        switch (_currentState)
        {
            case State.TopLeft:
                break;
            case State.BottomRight:
            {
                if (editor.Project.CurrentContainer?.WorkingLayer is { } && _image is { })
                {
                    editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_image);
                    editor.Project.CurrentContainer?.WorkingLayer?.RaiseInvalidateLayer();
                }
                break;
            }
        }

        _currentState = State.TopLeft;

        if (_selection is { })
        {
            _selection.Reset();
            _selection = null;
        }

        var selection = ServiceProvider.GetService<ISelectionService>();
        selection?.ClearConnectionPoints();

        editor.IsToolIdle = true;
    }
}
