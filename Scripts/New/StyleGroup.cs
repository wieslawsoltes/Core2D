
var p = Context.Editor.Project;
var sg = ShapeStyleGroup.Create("New");

sg.Styles.Add(ShapeStyle.Create("New", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
sg.CurrentStyle = sg.Styles.FirstOrDefault();

p.StyleGroups.Add(sg);
p.CurrentStyleGroup = sg;
