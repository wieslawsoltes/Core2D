var c = Context.Editor.Container;
var sg = ShapeStyleGroup.Create("New");
sg.Styles.Add(ShapeStyle.Create("New", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
sg.CurrentStyle = sg.Styles.FirstOrDefault();
c.StyleGroups.Add(sg);
c.CurrentStyleGroup = sg;