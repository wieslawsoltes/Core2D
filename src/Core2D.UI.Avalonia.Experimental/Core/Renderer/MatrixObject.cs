// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Core2D.Renderer
{
    public class MatrixObject : ObservableObject, ICopyable
    {
        private double _m11;
        private double _m12;
        private double _m21;
        private double _m22;
        private double _offsetX;
        private double _offsetY;

        public double M11
        {
            get => _m11;
            set => Update(ref _m11, value);
        }

        public double M12
        {
            get => _m12;
            set => Update(ref _m12, value);
        }

        public double M21
        {
            get => _m21;
            set => Update(ref _m21, value);
        }

        public double M22
        {
            get => _m22;
            set => Update(ref _m22, value);
        }

        public double OffsetX
        {
            get => _offsetX;
            set => Update(ref _offsetX, value);
        }

        public double OffsetY
        {
            get => _offsetY;
            set => Update(ref _offsetY, value);
        }

        public static MatrixObject Identity => new MatrixObject(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);

        public MatrixObject()
            : base()
        {
        }

        public MatrixObject(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
            : base()
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M21 = m21;
            this.M22 = m22;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public virtual bool Invalidate(ShapeRenderer r)
        {
            if (this.IsDirty == true)
            {
                r.InvalidateCache(this);
                this.IsDirty = false;
                return true;
            }
            return false;
        }

        public object Copy(IDictionary<object, object> shared)
        {
            return new MatrixObject()
            {
                M11 = this.M11,
                M12 = this.M12,
                M21 = this.M21,
                M22 = this.M22,
                OffsetX = this.OffsetX,
                OffsetY = this.OffsetY
            };
        }
    }
}
