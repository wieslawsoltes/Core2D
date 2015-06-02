// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Test2d;

namespace Test
{
    /// <summary>
    /// 
    /// </summary>
    public class TransformGroupHelper
    {
        private readonly TransformGroup _group;
        private readonly ScaleTransform _scale;
        private readonly SkewTransform _skew;
        private readonly RotateTransform _rotate;
        private readonly TranslateTransform _translate;

        /// <summary>
        /// 
        /// </summary>
        public TransformGroup Group
        {
            get { return _group; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public TransformGroupHelper(XTransform t)
        {
            _scale = new ScaleTransform(t.ScaleX, t.ScaleY, t.CenterX, t.CenterY);
            _skew = new SkewTransform(t.SkewAngleX, t.SkewAngleY, t.CenterX, t.CenterY);
            _rotate = new RotateTransform(t.RotateAngle, t.CenterX, t.CenterY);
            _translate = new TranslateTransform(t.OffsetX, t.OffsetY);
            _group = new TransformGroup();
            _group.Children.Add(_scale);
            _group.Children.Add(_skew);
            _group.Children.Add(_rotate);
            _group.Children.Add(_translate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void Update(XTransform t)
        {
            _scale.ScaleX = t.ScaleX;
            _scale.ScaleY = t.ScaleY;
            _scale.CenterX = t.CenterX;
            _scale.CenterY = t.CenterY;
            _skew.AngleX = t.SkewAngleX;
            _skew.AngleY = t.SkewAngleY;
            _skew.CenterX = t.CenterX;
            _skew.CenterY = t.CenterY;
            _rotate.Angle = t.RotateAngle;
            _rotate.CenterX = t.CenterX;
            _rotate.CenterY = t.CenterY;
            _translate.X = t.OffsetX;
            _translate.Y = t.OffsetY;
        }
    }
}
