// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    public class LayerContainer : ObservableObject, ILayerContainer, ICopyable
    {
        private ShapeStyle _style;
        private MatrixObject _transform;
        private double _width;
        private double _height;
        private ArgbColor _printBackground;
        private ArgbColor _workBackground;
        private ArgbColor _inputBackground;
        private ObservableCollection<LineShape> _guides;
        private ObservableCollection<BaseShape> _shapes;
        private ObservableCollection<ShapeStyle> _styles;

        public ShapeStyle Style
        {
            get => _style;
            set => Update(ref _style, value);
        }

        public MatrixObject Transform
        {
            get => _transform;
            set => Update(ref _transform, value);
        }

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

        public ObservableCollection<ShapeStyle> Styles
        {
            get => _styles;
            set => Update(ref _styles, value);
        }

        public LayerContainer()
        {
            _guides = new ObservableCollection<LineShape>();
            _shapes = new ObservableCollection<BaseShape>();
            _styles = new ObservableCollection<ShapeStyle>();
        }

        public virtual IEnumerable<PointShape> GetPoints()
        {
            foreach (var shape in Shapes)
            {
                foreach (var point in shape.GetPoints())
                {
                    yield return point;
                }
            }
        }

        public virtual object BeginTransform(object dc, ShapeRenderer renderer)
        {
            if (Transform != null)
            {
                return renderer.PushMatrix(dc, Transform);
            }
            return null;
        }

        public virtual void EndTransform(object dc, ShapeRenderer renderer, object state)
        {
            if (Transform != null)
            {
                renderer.PopMatrix(dc, state);
            }
        }

        public virtual void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var state = BeginTransform(dc, renderer);

            if (Guides != null)
            {
                foreach (var shape in Guides)
                {
                    shape.Draw(dc, renderer, dx, dy, db, r);
                }
            }

            foreach (var shape in Shapes)
            {
                shape.Draw(dc, renderer, dx, dy, db, r);
            }

            EndTransform(dc, renderer, state);
        }

        public virtual bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            bool result = false;

            var points = GetPoints();

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

            foreach (var point in points)
            {
                point.IsDirty = false;
            }

            return result;
        }

        public virtual object Copy(IDictionary<object, object> shared)
        {
            var copy = new LayerContainer()
            {
                Name = this.Name,
                Width = this.Width,
                Height = this.Height
            };

            if (shared != null)
            {
                foreach (var guide in this.Guides)
                {
                    copy.Guides.Add((LineShape)guide.Copy(shared));
                }

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
