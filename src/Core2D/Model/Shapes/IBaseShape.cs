using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Defines base shape contract.
    /// </summary>
    public interface IBaseShape : IObservableObject, IBindable, IDrawable, ISelectable
    {
        /// <summary>
        /// Gets shape target type.
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Indicates shape state options.
        /// </summary>
        IShapeState State { get; set; }

        /// <summary>
        /// Gets or sets shape <see cref="IContext"/>.
        /// </summary>
        IContext Data { get; set; }

        /// <summary>
        /// Get all points in the shape.
        /// </summary>
        /// <param name="points">The points list.</param>
        void GetPoints(IList<IPointShape> points);
    }
}
