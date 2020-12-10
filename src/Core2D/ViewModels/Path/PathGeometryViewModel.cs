using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Core2D.Model.Path;

namespace Core2D.ViewModels.Path
{
    public partial class PathGeometryViewModel : ViewModelBase
    {
        public static FillRule[] FillRuleValues { get; } = (FillRule[])Enum.GetValues(typeof(FillRule));

        [AutoNotify] private ImmutableArray<PathFigureViewModel> _figures;
        [AutoNotify] private FillRule _fillRule;

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            foreach (var figure in Figures)
            {
                isDirty |= figure.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var figure in Figures)
            {
                figure.Invalidate();
            }
        }

        public string ToXamlString(ImmutableArray<PathFigureViewModel> figures)
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

        public string ToSvgString(ImmutableArray<PathFigureViewModel> figures)
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

        public string ToSvgString()
        {
            if (Figures.Length > 0)
            {
                return ToSvgString(Figures);
            }
            return string.Empty;
        }
    }
}
