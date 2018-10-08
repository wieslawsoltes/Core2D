// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    public class GuideToolSettings : SettingsBase
    {
        private ShapeStyle _guideStyle;
        
        public ShapeStyle GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }
}
