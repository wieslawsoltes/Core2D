#nullable enable
using System.Linq;

namespace Core2D.ViewModels.Editor
{
    public partial class ProjectEditorViewModel
    {
        public void OnCancel()
        {
            ServiceProvider.GetService<ISelectionService>()?.OnDeselectAll();
            OnResetTool();
        }

        public void OnToolNone()
        {
            OnResetTool();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "None");
        }

        public void OnToolSelection()
        {
            OnResetTool();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Selection");
        }

        public void OnToolPoint()
        {
            OnResetTool();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Point");
        }

        public void OnToolLine()
        {
            if (CurrentTool?.Title == "Path" && CurrentPathTool?.Title != "Line")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Line");
            }
            else
            {
                OnResetTool();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "Line");
            }
        }

        public void OnToolArc()
        {
            if (CurrentTool?.Title == "Path" && CurrentPathTool?.Title != "Arc")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Arc");
            }
            else
            {
                OnResetTool();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "Arc");
            }
        }

        public void OnToolCubicBezier()
        {
            if (CurrentTool?.Title == "Path" && CurrentPathTool?.Title != "CubicBezier")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "CubicBezier");
            }
            else
            {
                OnResetTool();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "CubicBezier");
            }
        }

        public void OnToolQuadraticBezier()
        {
            if (CurrentTool?.Title == "Path" && CurrentPathTool?.Title != "QuadraticBezier")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "QuadraticBezier");
            }
            else
            {
                OnResetTool();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "QuadraticBezier");
            }
        }

        public void OnToolPath()
        {
            OnResetTool();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Path");
        }

        public void OnToolRectangle()
        {
            OnResetTool();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Rectangle");
        }

        public void OnToolEllipse()
        {
            OnResetTool();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Ellipse");
        }

        public void OnToolText()
        {
            OnResetTool();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Text");
        }

        public void OnToolImage()
        {
            OnResetTool();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Image");
        }

        public void OnToolMove()
        {
            if (CurrentTool?.Title == "Path" && CurrentPathTool?.Title != "Move")
            {
                OnResetPathTool();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Move");
            }
        }

        private void OnResetTool()
        {
            CurrentTool?.Reset();
        }

        private void OnResetPathTool()
        {
            CurrentPathTool?.Reset();
        }
    }
}
