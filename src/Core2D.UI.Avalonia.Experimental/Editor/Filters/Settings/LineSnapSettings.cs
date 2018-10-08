// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;

namespace Core2D.Editor.Filters
{
    public class LineSnapSettings : SettingsBase
    {
        private bool _isEnabled;
        private bool _enableGuides;
        private LineSnapMode _mode;
        private LineSnapTarget _target;
        private double _threshold;
        private ShapeStyle _guideStyle;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }

        public bool EnableGuides
        {
            get => _enableGuides;
            set => Update(ref _enableGuides, value);
        }

        public LineSnapMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public LineSnapTarget Target
        {
            get => _target;
            set => Update(ref _target, value);
        }

        public double Threshold
        {
            get => _threshold;
            set => Update(ref _threshold, value);
        }

        public ShapeStyle GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }
}
