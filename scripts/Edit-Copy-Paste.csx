OnSelectAll();
OnCopy(null);

var name = Project.CurrentContainer.Name;

OnNewPage(Project.CurrentContainer);
OnPaste(null);
OnDeselectAll();

Project.CurrentContainer.Name = name + "_copy";
