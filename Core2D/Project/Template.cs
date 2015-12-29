// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class Template : Container
    {
        private double _width;
        private double _height;
        private ArgbColor _background;

        /// <summary>
        /// 
        /// </summary>
        public double Width
        {
            get { return _width; }
            set { Update(ref _width, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Height
        {
            get { return _height; }
            set { Update(ref _height, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ArgbColor Background
        {
            get { return _background; }
            set { Update(ref _background, value); }
        }

        /// <summary>
        /// Creates a new <see cref="Template"/> instance.
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <param name="width">The template width.</param>
        /// <param name="height">The template height.</param>
        /// <returns>The new instance of the <see cref="Template"/>.</returns>
        public static Template Create(string name = "Template", double width = 840, double height = 600)
        {
            var template = new Template()
            {
                Name = name,
                Layers = ImmutableArray.Create<Layer>(),
            };

            template.Background = ArgbColor.Create(0x00, 0xFF, 0xFF, 0xFF);
            template.Width = width;
            template.Height = height;

            var builder = template.Layers.ToBuilder();
            builder.Add(Layer.Create("Layer1", template));
            builder.Add(Layer.Create("Layer2", template));
            builder.Add(Layer.Create("Layer3", template));
            template.Layers = builder.ToImmutable();

            template.CurrentLayer = template.Layers.FirstOrDefault();
            template.WorkingLayer = Layer.Create("Working", template);
            template.HelperLayer = Layer.Create("Helper", template);

            return template;
        }
    }
}
