// Move Point

#r "Core2D"

using Core2D.Editor;
using Core2D.Editor.Input;

Editor.OnToolSelection();

var p0 = new InputArgs(30, 30, ModifierFlags.None);
var p1 = new InputArgs(60, 30, ModifierFlags.None);
Editor.CurrentTool.LeftDown(p0);
Editor.CurrentTool.Move(p1);
Editor.CurrentTool.LeftUp(p1);
