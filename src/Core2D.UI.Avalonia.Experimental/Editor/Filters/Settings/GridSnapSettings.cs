// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;

namespace Core2D.Editor.Filters
{
    public class GridSnapSettings : SettingsBase
    {
        private bool _isEnabled;
        private bool _enableGuides;
        private GridSnapMode _mode;
        private double _gridSizeX;
        private double _gridSizeY;
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

        public GridSnapMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public double GridSizeX
        {
            get => _gridSizeX;
            set => Update(ref _gridSizeX, value);
        }

        public double GridSizeY
        {
            get => _gridSizeY;
            set => Update(ref _gridSizeY, value);
        }

        public ShapeStyle GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }
}
