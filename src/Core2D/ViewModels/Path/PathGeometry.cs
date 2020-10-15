using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Core2D.Path
{
    /// <summary>
    /// Path geometry.
    /// </summary>
    public class PathGeometry : ObservableObject
    {
        private ImmutableArray<PathFigure> _figures;
        private FillRule _fillRule;

        /// <inheritdoc/>
        public ImmutableArray<PathFigure> Figures
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

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var figure in Figures)
            {
                isDirty |= figure.IsDirty();
            }

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var figure in Figures)
            {
                figure.Invalidate();
            }
        }

        public string ToXamlString(ImmutableArray<PathFigure> figures)
        {
            if (figures.Length == 0)
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < figures.Length; i++)
            {
                sb.Append(figures[i].ToXamlString());
                if (i != figures.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        public string ToSvgString(ImmutableArray<PathFigure> figures)
        {
            if (figures.Length == 0)
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < figures.Length; i++)
            {
                sb.Append(figures[i].ToSvgString());
                if (i != figures.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToXamlString()
        {
            string figuresString = string.Empty;

            if (Figures.Length > 0)
            {
                figuresString = ToXamlString(Figures);
            }

            if (FillRule == FillRule.Nonzero)
            {
                return "F1" + figuresString;
            }

            return figuresString;
        }

        /// <inheritdoc/>
        public string ToSvgString()
        {
            if (Figures.Length > 0)
            {
                return ToSvgString(Figures);
            }
            return string.Empty;
        }

        /// <summary>
        /// Check whether the <see cref="Figures"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFigures() => true;

        /// <summary>
        /// Check whether the <see cref="FillRule"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeFillRule() => _fillRule != default;
    }
}
