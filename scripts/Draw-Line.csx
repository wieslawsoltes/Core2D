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