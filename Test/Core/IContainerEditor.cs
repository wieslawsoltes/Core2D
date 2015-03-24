using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public interface IContainerEditor
    {
        IContainer Container { get; set; }
        IRenderer Renderer { get; set; }
        Tool CurrentTool { get; set; }
        State CurrentState { get; set; }
        bool DefaultIsFilled { get; set; }
        bool SnapToGrid { get; set; }
        double SnapX { get; set; }
        double SnapY { get; set; }
        double Snap(double value, double snap);
        void Left(double x, double y);
        void Right(double x, double y);
        void Move(double x, double y);
    }
}
