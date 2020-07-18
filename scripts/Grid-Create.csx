
foreach (var document in Project.Documents)
{
    foreach (var page in document.Pages)
    {
        var template = page.Template;
        template.IsGridEnabled = true;
        template.IsBorderEnabled = true;
        template.GridOffsetLeft = 30.0;
        template.GridOffsetTop = 30.0;
        template.GridOffsetRight = -30.0;
        template.GridOffsetBottom = -30.0;
        template.GridCellWidth = 30.0;
        template.GridCellHeight = 30.0;
        template.GridStrokeColor = Factory.CreateArgbColor(0xFF, 0xDE, 0xDE, 0xDE);
        template.GridStrokeThickness = 1.0;
    }
}
