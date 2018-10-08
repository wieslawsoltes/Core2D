// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Renderer;

namespace Core2D.Style
{
    public class ShapeStyle : ObservableObject, ICopyable
    {
        private ArgbColor _stroke;
        private ArgbColor _fill;
        private double _thickness;
        private bool _isStroked;
        private bool _isFilled;

        public ArgbColor Stroke
        {
            get => _stroke;
            set => Update(ref _stroke, value);
        }

        public ArgbColor Fill
        {
            get => _fill;
            set => Update(ref _fill, value);
        }

        public double Thickness
        {
            get => _thickness;
            set => Update(ref _thickness, value);
        }

        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        public ShapeStyle()
        {
        }

        public ShapeStyle(ArgbColor stroke, ArgbColor fill, double thickness, bool isStroked, bool isFilled)
        {
            this.Stroke = stroke;
            this.Fill = fill;
            this.Thickness = thickness;
            this.IsStroked = isStroked;
            this.IsFilled = isFilled;
        }

        public virtual bool Invalidate(ShapeRenderer r)
        {
            if ((this.IsDirty == true)
                || (_stroke?.IsDirty ?? false) 
                || (_fill?.IsDirty ?? false))
            {
                r.InvalidateCache(this);
                this.IsDirty = false;

                if (_stroke != null)
                {
                    _stroke.IsDirty = false;
                }

                if (_fill != null)
                {
                    _fill.IsDirty = false;
                }

                return true;
            }

            return false;
        }

        public object Copy(IDictionary<object, object> shared)
        {
            return new ShapeStyle()
            {
                Name = this.Name,
                Stroke = (ArgbColor)this.Stroke.Copy(shared),
                Fill = (ArgbColor)this.Fill.Copy(shared),
                Thickness = this.Thickness,
                IsStroked = this.IsStroked,
                IsFilled = this.IsFilled
            };
        }
    }
}
