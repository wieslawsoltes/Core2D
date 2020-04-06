using System;
using System.Collections.Generic;
using System.IO;
using Core2D.Shapes;
using Svg.Skia;

namespace Core2D.Renderer.SkiaSharp
{
    /// <summary>
    /// Svg converter.
    /// </summary>
    public class SvgConverter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgConverter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SvgConverter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Converts svg file to shapes.
        /// </summary>
        /// <param name="path">The svg path.</param>
        /// <returns>The converted shapes.</returns>
        public IList<IBaseShape> Convert(string path)
        {
            var document = SKSvg.Open(path);

            if (document == null)
            {
                return null;
            }

            var drawable = SKSvg.ToDrawable(document);
            if (drawable == null)
            {
                return null;
            }

            var shapes = new List<IBaseShape>();

            switch (drawable)
            {
                case AnchorDrawable anchorDrawable:
                    {

                    }
                    break;
                case FragmentDrawable fragmentDrawable:
                    {

                    }
                    break;
                case ImageDrawable imageDrawable:
                    {

                    }
                    break;
                case SwitchDrawable switchDrawable:
                    {

                    }
                    break;
                case UseDrawable useDrawable:
                    {

                    }
                    break;
                case CircleDrawable circleDrawable:
                    {

                    }
                    break;
                case EllipseDrawable ellipseDrawable:
                    {

                    }
                    break;
                case RectangleDrawable rectangleDrawable:
                    {

                    }
                    break;
                case GroupDrawable groupDrawable:
                    {

                    }
                    break;
                case LineDrawable lineDrawable:
                    {

                    }
                    break;
                case PathDrawable pathDrawable:
                    {

                    }
                    break;
                case PolylineDrawable polylineDrawable:
                    {

                    }
                    break;
                case PolygonDrawable polygonDrawable:
                    {

                    }
                    break;
                case TextDrawable textDrawable:
                    {

                    }
                    break;
            }

            return shapes;
        }
    }
}
