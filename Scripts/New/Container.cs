var p = Context.Editor.Project;

var t = Container.Create("New");
p.Templates.Add(t);

var c = Container.Create("New");
c.Template = t;

p.CurrentDocument.Containers.Add(c);
p.CurrentContainer = c;