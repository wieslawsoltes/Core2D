// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Core2D.Attributes;

namespace Core2D.Path
{
    /// <summary>
    /// Path geometry.
    /// </summary>
    public class PathGeometry : ObservableObject, IPathGeometry
    {
        private ImmutableArray<IPathFigure> _figures;
        private FillRule _fillRule;

        /// <inheritdoc/>
        [Content]
        public ImmutableArray<IPathFigure> Figures
        {
            get => _figures;
            set => Update(ref _figures, value);
        }

        /// <inheritdoc/>
        public FillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometry"/> class.
        /// </summary>
        public PathGeometry() => Figures = ImmutableArray.Create<IPathFigure>();

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="PathGeometry"/> instance.
        /// </summary>
        /// <param name="figures">The figures collection.</param>
        /// <param name="fillRule">The fill rule.</param>
        /// <returns>The new instance of the <see cref="PathGeometry"/> class.</returns>
        public static IPathGeometry Create(ImmutableArray<IPathFigure> figures, FillRule fillRule)
        {
            return new PathGeometry()
            {
                Figures = figures,
                FillRule = fillRule
            };
        }

        /// <summary>
        /// Creates a string representation of figures collection.
        /// </summary>
        /// <param name="figures">The figures collection.</param>
        /// <returns>A string representation of figures collection.</returns>
        public string ToString(ImmutableArray<IPathFigure> figures)
        {
            if (figures.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < figures.Length; i++)
            {
                sb.Append(figures[i]);
                if (i != figures.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string figuresString = string.Empty;

            if (Figures.Length > 0)
            {
                figuresString = ToString(Figures);
            }

            if (FillRule == FillRule.Nonzero)
            {
                return "F1" + figuresString;
            }

            return figuresString;
        }

        /// <summary>
        /// Check whether the <see cref="Figures"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFigures() => _figures.IsEmpty == false;

        /// <summary>
        /// Check whether the <see cref="FillRule"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFillRule() => _fillRule != default;
    }
}
