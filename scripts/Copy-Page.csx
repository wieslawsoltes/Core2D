// Copy Page

Editor.OnSelectAll();
Editor.OnCopy(null);
var name = Editor.Project.CurrentContainer.Name;
Editor.OnNewPage(Editor.Project.CurrentContainer);
Editor.OnPaste(null);
Editor.OnDeselectAll();
Editor.Project.CurrentContainer.Name = name + "_copy";
