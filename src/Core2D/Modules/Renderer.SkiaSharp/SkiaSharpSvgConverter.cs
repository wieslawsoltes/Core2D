using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Editor;
using Core2D.Shapes;
using SkiaSharp;
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

        // TODO:
        /*
        private void FromDrawablePath(DrawablePath drawablePath, IList<IBaseShape> shapes, IFactory factory)
        {
            var path = drawablePath.Path;
            if (path == null)
            {
                return;
            }
            var stroke = drawablePath.Stroke;
            var fill = drawablePath.Fill;
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

        private void FromDrawableContainer(DrawableContainer drawableContainer, IList<IBaseShape> shapes, IFactory factory)
        {
            var group = factory.CreateGroupShape(ProjectEditorConfiguration.DefaulGroupName);
            var groupShapes = new List<IBaseShape>();
            foreach (var child in drawableContainer.ChildrenDrawables)
            {
                ToShape(child, groupShapes, factory);
            }
            foreach (var groupShape in groupShapes)
            {
                group.AddShape(groupShape);
            }
            shapes.Add(group);
        }

        private void ToShape(Drawable drawable, IList<IBaseShape> shapes, IFactory factory)
        {
            switch (drawable)
            {
                case AnchorDrawable anchorDrawable:
                    {
                        FromDrawableContainer(anchorDrawable, shapes, factory);
                    }
                    break;
                case FragmentDrawable fragmentDrawable:
                    {
                        FromDrawableContainer(fragmentDrawable, shapes, factory);
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
                            ToShape(imageDrawable.FragmentDrawable, shapes, factory);
                        }
                    }
                    break;
                case SwitchDrawable switchDrawable:
                    {
                        if (switchDrawable.FirstChild != null)
                        {
                            ToShape(switchDrawable.FirstChild, shapes, factory);
                        }
                    }
                    break;
                case UseDrawable useDrawable:
                    {
                        if (useDrawable.ReferencedDrawable != null)
                        {
                            ToShape(useDrawable.ReferencedDrawable, shapes, factory);
                        }
                    }
                    break;
                case CircleDrawable circleDrawable:
                    {
                        FromDrawablePath(circleDrawable, shapes, factory);
                    }
                    break;
                case EllipseDrawable ellipseDrawable:
                    {
                        FromDrawablePath(ellipseDrawable, shapes, factory);
                    }
                    break;
                case RectangleDrawable rectangleDrawable:
                    {
                        FromDrawablePath(rectangleDrawable, shapes, factory);
                    }
                    break;
                case GroupDrawable groupDrawable:
                    {
                        FromDrawableContainer(groupDrawable, shapes, factory);
                    }
                    break;
                case LineDrawable lineDrawable:
                    {
                        FromDrawablePath(lineDrawable, shapes, factory);
                    }
                    break;
                case PathDrawable pathDrawable:
                    {
                        FromDrawablePath(pathDrawable, shapes, factory);
                    }
                    break;
                case PolylineDrawable polylineDrawable:
                    {
                        FromDrawablePath(polylineDrawable, shapes, factory);
                    }
                    break;
                case PolygonDrawable polygonDrawable:
                    {
                        FromDrawablePath(polygonDrawable, shapes, factory);
                    }
                    break;
                case TextDrawable textDrawable:
                    {
                        // TODO:
                    }
                    break;
            }
        }
        */

        /// <inheritdoc/>
        public IList<IBaseShape> Convert(string path)
        {
            var document = SKSvg.Open(path);
            if (document == null)
            {
                return null;
            }

            // TODO:
            /*
            using var drawable = SKSvg.ToDrawable(document);
            if (drawable == null)
            {
                return null;
            }

            var shapes = new List<IBaseShape>();
            var factory = _serviceProvider.GetService<IFactory>();

            ToShape(drawable, shapes, factory);

            return shapes;
            */

            return null;
        }
    }
}
