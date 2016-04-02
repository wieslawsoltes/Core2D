// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;
using System.Linq;

namespace Core2D.Project
{
    /// <summary>
    /// Template container.
    /// </summary>
    public class XTemplate : XContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XTemplate"/> class.
        /// </summary>
        public XTemplate()
            : base()
        {
        }

        /// <summary>
        /// Creates a new <see cref="XTemplate"/> instance.
        /// </summary>
        /// <param name="name">The template name.</param>
        /// <param name="width">The template width.</param>
        /// <param name="height">The template height.</param>
        /// <returns>The new instance of the <see cref="XTemplate"/>.</returns>
        public static XTemplate Create(string name = "Template", double width = 840, double height = 600)
        {
            var template = new XTemplate()
            {
                Name = name
            };

            template.Background = ArgbColor.Create(0x00, 0xFF, 0xFF, 0xFF);
            template.Width = width;
            template.Height = height;

            var builder = template.Layers.ToBuilder();
            builder.Add(XLayer.Create("TemplateLayer1", template));
            builder.Add(XLayer.Create("TemplateLayer2", template));
            builder.Add(XLayer.Create("TemplateLayer3", template));
            template.Layers = builder.ToImmutable();

            template.CurrentLayer = template.Layers.FirstOrDefault();
            template.WorkingLayer = XLayer.Create("TemplateWorking", template);
            template.HelperLayer = XLayer.Create("TemplateHelper", template);

            return template;
        }
    }
}
