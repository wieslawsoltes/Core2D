// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;
using Core2D.Shapes;

namespace Core2D.Data
{
    /// <summary>
    /// Defines data flow contract.
    /// </summary>
    public interface IDataFlow
    {
        /// <summary>
        /// Binds data to <see cref="IProjectContainer"/> using data context.
        /// </summary>
        /// <param name="project">The <see cref="IProjectContainer"/> object.</param>
        void Bind(IProjectContainer project);

        /// <summary>
        /// Binds data to <see cref="IDocumentContainer"/> using data context.
        /// </summary>
        /// <param name="document">The <see cref="IDocumentContainer"/> object.</param>
        void Bind(IDocumentContainer document);

        /// <summary>
        /// Binds data to <see cref="IPageContainer"/> using data context.
        /// </summary>
        /// <param name="container">The <see cref="IPageContainer"/> object.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(IPageContainer container, object db, object r);

        /// <summary>
        /// Binds data to <see cref="ILayerContainer"/> using data context.
        /// </summary>
        /// <param name="layer">The <see cref="ILayerContainer"/> object.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(ILayerContainer layer, object db, object r);

        /// <summary>
        /// Binds data to <see cref="ILayerContainer"/> using data context.
        /// </summary>
        /// <param name="line">The <see cref="ILineShape"/> shape.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(ILineShape line, object db, object r);

        /// <summary>
        /// Binds data to <see cref="IRectangleShape"/> using data context.
        /// </summary>
        /// <param name="rectangle">The <see cref="IRectangleShape"/> shape.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(IRectangleShape rectangle, object db, object r);

        /// <summary>
        /// Binds data to <see cref="IEllipseShape"/> using data context.
        /// </summary>
        /// <param name="ellipse">The <see cref="IEllipseShape"/> shape.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(IEllipseShape ellipse, object db, object r);

        /// <summary>
        /// Draws a <see cref="IArcShape"/> shape using drawing context.
        /// </summary>
        /// <param name="arc">The <see cref="IArcShape"/> shape.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(IArcShape arc, object db, object r);

        /// <summary>
        /// Binds data to <see cref="ICubicBezierShape"/> using data context.
        /// </summary>
        /// <param name="cubicBezier">The <see cref="ICubicBezierShape"/> shape.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(ICubicBezierShape cubicBezier, object db, object r);

        /// <summary>
        /// Binds data to <see cref="ILayerContainer"/> using data context.
        /// </summary>
        /// <param name="quadraticBezier">The <see cref="IQuadraticBezierShape"/> shape.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(IQuadraticBezierShape quadraticBezier, object db, object r);

        /// <summary>
        /// Binds data to <see cref="ITextShape"/> using data context.
        /// </summary>
        /// <param name="text">The <see cref="ITextShape"/> shape.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(ITextShape text, object db, object r);

        /// <summary>
        /// Binds data to <see cref="IImageShape"/> using data context.
        /// </summary>
        /// <param name="image">The <see cref="IImageShape"/> shape.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(IImageShape image, object db, object r);

        /// <summary>
        /// Binds data to <see cref="IPathShape"/> using data context.
        /// </summary>
        /// <param name="path">The <see cref="IPathShape"/> shape.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The data record.</param>
        void Bind(IPathShape path, object db, object r);
    }
}
