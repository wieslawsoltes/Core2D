// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Style;

namespace Core2D.Shapes
{
    public class FigureShape : BaseShape, ILayerContainer, ICopyable
    {
        private double _width;
        private double _height;
        private ArgbColor _printBackground;
        private ArgbColor _workBackground;
        private ArgbColor _inputBackground;
        private ObservableCollection<LineShape> _guides;
        private ObservableCollection<BaseShape> _shapes;
        private bool _isFilled;
        private bool _isClosed;

        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => Update(ref _height, value);
        }

        public ArgbColor PrintBackground
        {
            get => _printBackground;
            set => Update(ref _printBackground, value);
        }

        public ArgbColor WorkBackground
        {
            get => _workBackground;
            set => Update(ref _workBackground, value);
        }

        public ArgbColor InputBackground
        {
            get => _inputBackground;
            set => Update(ref _inputBackground, value);
        }

        public ObservableCollection<LineShape> Guides
        {
            get => _guides;
            set => Update(ref _guides, value);
        }

        public ObservableCollection<BaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }

        public FigureShape()
            : base()
        {
            _shapes = new ObservableCollection<BaseShape>();
        }

        public FigureShape(ObservableCollection<BaseShape> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public FigureShape(string name)
            : this()
        {
            this.Name = name;
        }

        public FigureShape(string name, ObservableCollection<BaseShape> shapes)
            : base()
        {
            this.Name = name;
            this.Shapes = shapes;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            foreach (var shape in Shapes)
            {
                foreach (var point in shape.GetPoints())
                {
                    yield return point;
                }
            }
        }

        public override bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            bool result = base.Invalidate(renderer, dx, dy);

            if (Guides != null)
            {
                foreach (var guide in Guides)
                {
                    result |= guide.Invalidate(renderer, dx, dy);
                }
            }

            foreach (var shape in Shapes)
            {
                result |= shape.Invalidate(renderer, dx, dy);
            }

            if (this.IsDirty || result == true)
            {
                renderer.InvalidateCache(this, Style, dx, dy);
                this.IsDirty = false;
                result |= true;
            }

            return result;
        }

        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var state = base.BeginTransform(dc, renderer);

            if (Guides != null)
            {
                foreach (var guide in Guides)
                {
                    guide.Draw(dc, renderer, dx, dy, db, r);
                }
            }

            foreach (var shape in Shapes)
            {
                shape.Draw(dc, renderer, dx, dy, db ,r);
            }

            base.EndTransform(dc, renderer, state);
        }

        public override void Move(ISelection selection, double dx, double dy)
        {
            var points = GetPoints().Distinct();

            foreach (var point in points)
            {
                if (!selection.Selected.Contains(point))
                {
                    point.Move(selection, dx, dy);
                }
            }
        }

        public object Copy(IDictionary<object, object> shared)
        {
            var copy = new FigureShape()
            {
                Style = this.Style,
                Transform = (MatrixObject)this.Transform?.Copy(shared),
                Width = this.Width,
                Height = this.Height,
                IsFilled = this.IsFilled,
                IsClosed = this.IsClosed
            };

            if (shared != null)
            {
                foreach (var shape in this.Shapes)
                {
                    if (shape is ICopyable copyable)
                    {
                        copy.Shapes.Add((BaseShape)copyable.Copy(shared));
                    }
                }
            }

            return copy;
        }
    }
}
