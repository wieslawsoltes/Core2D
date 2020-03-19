// Draw Line

#r "Core2D"

using Core2D.Editor;
using Core2D.Editor.Input;

Editor.OnToolLine();

var p0 = new InputArgs(30, 30, ModifierFlags.None);
Editor.CurrentTool.LeftDown(p0);
Editor.CurrentTool.LeftUp(p0);

var p1 = new InputArgs(300, 30, ModifierFlags.None);
Editor.CurrentTool.LeftDown(p1);
Editor.CurrentTool.LeftUp(p1);
