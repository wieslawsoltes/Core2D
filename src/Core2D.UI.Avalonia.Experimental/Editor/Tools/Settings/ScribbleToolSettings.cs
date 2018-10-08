// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class ScribbleToolSettings : SettingsBase
    {
        private bool _simplify;
        private double _epsilon;
        private PathFillRule _fillRule;
        private bool _isFilled;
        private bool _isClosed;

        public bool Simplify
        {
            get => _simplify;
            set => Update(ref _simplify, value);
        }

        public double Epsilon
        {
            get => _epsilon;
            set => Update(ref _epsilon, value);
        }

        public PathFillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
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
    }
}
