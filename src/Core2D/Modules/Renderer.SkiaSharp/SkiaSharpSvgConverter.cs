using System;
using System.Collections.Generic;
using Core2D.Editor;
using Core2D.Interfaces;
using Core2D.Shapes;
using Svg.Skia;

namespace Core2D.Renderer.SkiaSharp
{
    /// <summary>
    /// Svg converter.
    /// </summary>
    public class SkiaSharpSvgConverter : ISvgConverter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaSharpSvgConverter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SkiaSharpSvgConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private void FromDrawablePath(DrawablePath drawablePath, IList<IBaseShape> shapes)
        {
            var path = drawablePath.Path;
            if (path == null)
            {
                return;
            }
            var stroke = drawablePath.Stroke;
            var fill = drawablePath.Fill;
            var factory = _serviceProvider.GetService<IFactory>();
            var style = factory.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var geometry = PathGeometryConverter.ToPathGeometry(path, 0.0, 0.0, factory);
            var pathShape = factory.CreatePathShape(
                "Path",
                style,
                geometry,
                stroke != null,
                fill != null);
            shapes.Add(pathShape);
        }

        private void FromDrawableContainer(DrawableContainer drawableContainer, IList<IBaseShape> shapes)
        {
            var factory = _serviceProvider.GetService<IFactory>();
            var group = factory.CreateGroupShape(ProjectEditorConfiguration.DefaulGroupName);
            var groupShapes = new List<IBaseShape>();
            foreach (var child in drawableContainer.ChildrenDrawables)
            {
                ToShape(child, groupShapes);
            }
            foreach (var groupShape in groupShapes)
            {
                group.AddShape(groupShape);
            }
            shapes.Add(group);
        }

        private void ToShape(Drawable drawable, IList<IBaseShape> shapes)
        {
            switch (drawable)
            {
                case AnchorDrawable anchorDrawable:
                    {
                        FromDrawableContainer(anchorDrawable, shapes);
                    }
                    break;
                case FragmentDrawable fragmentDrawable:
                    {
                        FromDrawableContainer(fragmentDrawable, shapes);
                    }
                    break;
                case ImageDrawable imageDrawable:
                    {
                        if (imageDrawable.Image != null)
                        {
                            // TODO: imageDrawable.Image
                        }
                        if (imageDrawable.FragmentDrawable != null)
                        {
                            ToShape(imageDrawable.FragmentDrawable, shapes);
                        }
                    }
                    break;
                case SwitchDrawable switchDrawable:
                    {
                        if (switchDrawable.FirstChild != null)
                        {
                            ToShape(switchDrawable.FirstChild, shapes);
                        }
                    }
                    break;
                case UseDrawable useDrawable:
                    {
                        if (useDrawable.ReferencedDrawable != null)
                        {
                            ToShape(useDrawable.ReferencedDrawable, shapes);
                        }
                    }
                    break;
                case CircleDrawable circleDrawable:
                    {
                        FromDrawablePath(circleDrawable, shapes);
                    }
                    break;
                case EllipseDrawable ellipseDrawable:
                    {
                        FromDrawablePath(ellipseDrawable, shapes);
                    }
                    break;
                case RectangleDrawable rectangleDrawable:
                    {
                        FromDrawablePath(rectangleDrawable, shapes);
                    }
                    break;
                case GroupDrawable groupDrawable:
                    {
                        FromDrawableContainer(groupDrawable, shapes);
                    }
                    break;
                case LineDrawable lineDrawable:
                    {
                        FromDrawablePath(lineDrawable, shapes);
                    }
                    break;
                case PathDrawable pathDrawable:
                    {
                        FromDrawablePath(pathDrawable, shapes);
                    }
                    break;
                case PolylineDrawable polylineDrawable:
                    {
                        FromDrawablePath(polylineDrawable, shapes);
                    }
                    break;
                case PolygonDrawable polygonDrawable:
                    {
                        FromDrawablePath(polygonDrawable, shapes);
                    }
                    break;
                case TextDrawable textDrawable:
                    {
                        // TODO:
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public IList<IBaseShape> Convert(string path)
        {
            var document = SKSvg.Open(path);
            if (document == null)
            {
                return null;
            }

            using var drawable = SKSvg.ToDrawable(document);
            if (drawable == null)
            {
                return null;
            }

            var shapes = new List<IBaseShape>();

            ToShape(drawable, shapes);

            return shapes;
        }
    }
}
