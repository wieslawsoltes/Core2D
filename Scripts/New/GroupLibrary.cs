
var p = Context.Editor.Project;
var gl = GroupLibrary.Create("New");

p.GroupLibraries = p.GroupLibraries.Add(gl);
p.CurrentGroupLibrary = gl;
