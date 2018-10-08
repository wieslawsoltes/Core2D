// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    public class PathToolSettings : SettingsBase
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private PathFillRule _fillRule;
        private bool _isFilled;
        private bool _isClosed;
        private ObservableCollection<ToolBase> _tools;
        private ToolBase _currentTool;
        private ToolBase _previousTool;

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

        public ObservableCollection<ToolBase> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        public ToolBase CurrentTool
        {
            get => _currentTool;
            set
            {
                PreviousTool = _currentTool;
                Update(ref _currentTool, value);
            }
        }

        public ToolBase PreviousTool
        {
            get => _previousTool;
            set => Update(ref _previousTool, value);
        }
    }
}
