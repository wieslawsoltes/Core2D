// Draw Line


#r "Core2D.Editor"
using Core2D.Editor;
using Core2D.Editor.Input;

Editor.OnToolLine();

var p0 = new InputArgs(30, 30, ModifierFlags.None);
Editor.LeftDown(p0);
Editor.LeftUp(p0);

var p1 = new InputArgs(300, 30, ModifierFlags.None);
Editor.LeftDown(p1);
Editor.LeftUp(p1);


// Move Point


#r "Core2D.Editor"
using Core2D.Editor;
using Core2D.Editor.Input;

Editor.OnToolSelection();

var p0 = new InputArgs(30, 30, ModifierFlags.None);
var p1 = new InputArgs(60, 30, ModifierFlags.None);
Editor.LeftDown(p0);
Editor.Move(p1);
Editor.LeftUp(p1);


// CanvasPlatform


Editor.CanvasPlatform.Invalidate();
Editor.CanvasPlatform.ResetZoom();
Editor.CanvasPlatform.AutoFitZoom();


// Undo Redo


Editor.OnUndo();
Editor.OnRedo();


// Execute Code


Editor.OnExecuteCode("Editor.CanvasPlatform.ResetZoom();");


// Copy Page


Editor.OnSelectAll();
Editor.OnCopy(null);
var name = Editor.Project.CurrentContainer.Name;
Editor.OnNewPage(Editor.Project.CurrentContainer);
Editor.OnPaste(null);
Editor.OnDeselectAll();
Editor.Project.CurrentContainer.Name = name + "_copy";


// Random


#r "System.Security.Cryptography.Csp"
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

double ToDouble(byte[] data, int index, double size)
{
    var ul = BitConverter.ToUInt64(data, index) / (1 << 11);
    return size * (ul / (double)(1UL << 53));
}

void RandomLines()
{
    double width = Editor.Project.CurrentContainer.Width;
    double height = Editor.Project.CurrentContainer.Height;

    using (var rng = new RNGCryptoServiceProvider())
    {
        byte[] data = new byte[32];
        for (int i = 0; i < 1000; i++)
        {
            rng.GetBytes(data);
            double x0 = ToDouble(data, 0, width);
            double y0 = ToDouble(data, 8, height);
            double x1 = ToDouble(data, 16, width);
            double y1 = ToDouble(data, 24, height);
            Editor.ShapeFactory.Line(x0, y0, x1, y1, true);
            //Console.WriteLine($"[{x0};{y0}] [{x1};{y1}]");
        }
    }
}

Task.Factory.StartNew(() => RandomLines());

