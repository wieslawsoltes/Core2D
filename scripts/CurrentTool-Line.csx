#r "Core2D"
using Core2D.Editor;
using Core2D.Editor.Input;

OnToolLine();

var p0 = new InputArgs(30, 30, ModifierFlags.None);
CurrentTool.LeftDown(p0);
CurrentTool.LeftUp(p0);

var p1 = new InputArgs(300, 30, ModifierFlags.None);
CurrentTool.LeftDown(p1);
CurrentTool.LeftUp(p1);
