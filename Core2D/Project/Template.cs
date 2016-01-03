// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class Template : Container
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class.
        /// </summary>
        public Template()
            : base()
        {
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
                Name = name
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
