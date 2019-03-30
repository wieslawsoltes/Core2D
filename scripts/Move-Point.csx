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
