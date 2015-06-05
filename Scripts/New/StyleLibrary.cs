
var p = Context.Editor.Project;
var sl = StyleLibrary.Create("New");

sl.Styles = sl.Styles.Add(ShapeStyle.Create("New", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
sl.CurrentStyle = sl.Styles.FirstOrDefault();

p.StyleLibraries = p.StyleLibraries.Add(sl);
p.CurrentStyleLibrary = sl;
