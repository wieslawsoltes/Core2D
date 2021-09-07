#nullable disable
using System.Linq;
using Core2D.Model.Editor;

namespace Core2D.ViewModels.Editor
{
    public partial class ProjectEditorViewModel
    {
        public void OnCancel()
        {
            OnDeselectAll();
            OnResetTool();
        }

        public void OnToolNone()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "None");
        }

        public void OnToolSelection()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Selection");
        }

        public void OnToolPoint()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Point");
        }

        public void OnToolLine()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Line")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Line");
            }
            else
            {
                CurrentTool?.Reset();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "Line");
            }
        }

        public void OnToolArc()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Arc")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Arc");
            }
            else
            {
                CurrentTool?.Reset();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "Arc");
            }
        }

        public void OnToolCubicBezier()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "CubicBezier")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "CubicBezier");
            }
            else
            {
                CurrentTool?.Reset();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "CubicBezier");
            }
        }

        public void OnToolQuadraticBezier()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "QuadraticBezier")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "QuadraticBezier");
            }
            else
            {
                CurrentTool?.Reset();
                CurrentTool = Tools.FirstOrDefault(t => t.Title == "QuadraticBezier");
            }
        }

        public void OnToolPath()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Path");
        }

        public void OnToolRectangle()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Rectangle");
        }

        public void OnToolEllipse()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Ellipse");
        }

        public void OnToolText()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Text");
        }

        public void OnToolImage()
        {
            CurrentTool?.Reset();
            CurrentTool = Tools.FirstOrDefault(t => t.Title == "Image");
        }

        public void OnToolMove()
        {
            if (CurrentTool.Title == "Path" && CurrentPathTool.Title != "Move")
            {
                CurrentPathTool?.Reset();
                CurrentPathTool = PathTools.FirstOrDefault(t => t.Title == "Move");
            }
        }

        public void OnResetTool()
        {
            CurrentTool?.Reset();
        }
    }
}
