// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    /// <summary>
    /// Defines page container interface.
    /// </summary>
    public interface IPageContainer : IBaseContainer
    {
        /// <summary>
        /// Gets or sets container width.
        /// </summary>
        /// <remarks>
        /// If template is not null Template.Width property is used.
        /// </remarks>
        double Width { get; set; }

        /// <summary>
        /// Gets or sets container height.
        /// </summary>
        /// <remarks>
        /// If template is not null Template.Height property is used.
        /// </remarks>
        double Height { get; set; }

        /// <summary>
        /// Gets or sets container background color.
        /// </summary>
        /// <remarks>
        /// If template is not null Template.Background property is used.
        /// </remarks>
        ArgbColor Background { get; set; }

        /// <summary>
        /// Gets or sets container layers.
        /// </summary>
        ImmutableArray<ILayerContainer> Layers { get; set; }

        /// <summary>
        /// Gets or sets current container layer.
        /// </summary>
        ILayerContainer CurrentLayer { get; set; }

        /// <summary>
        /// Gets or sets working container layer.
        /// </summary>
        ILayerContainer WorkingLayer { get; set; }

        /// <summary>
        /// Gets or sets helper container layer.
        /// </summary>
        ILayerContainer HelperLayer { get; set; }

        /// <summary>
        /// Gets or sets current container shape.
        /// </summary>
        IBaseShape CurrentShape { get; set; }

        /// <summary>
        /// Gets or sets container template.
        /// </summary>
        IPageContainer Template { get; set; }

        /// <summary>
        /// Gets or sets container data.
        /// </summary>
        Context Data { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether container is expanded.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Invalidate container layers.
        /// </summary>
        void Invalidate();
    }
}
