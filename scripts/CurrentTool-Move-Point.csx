#r "Core2D"
using Core2D.Editor;
using Core2D.Editor.Input;

OnToolSelection();

var p0 = new InputArgs(30, 30, ModifierFlags.None);
var p1 = new InputArgs(60, 30, ModifierFlags.None);
CurrentTool.LeftDown(p0);
CurrentTool.Move(p1);
CurrentTool.LeftUp(p1);
