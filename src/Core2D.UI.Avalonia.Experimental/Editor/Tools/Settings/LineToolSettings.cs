// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Editor.Tools
{
    public class LineToolSettings : SettingsBase
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private bool _splitIntersections;

        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public bool SplitIntersections
        {
            get => _splitIntersections;
            set => Update(ref _splitIntersections, value);
        }
    }
}
