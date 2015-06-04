
var helper = 
    ShapeStyle.Create(
        "Helper",
        0xFF, 0xFF, 0xFF, 0x00,
        0xFF, 0xFF, 0xFF, 0x00,
        1.0,
        LineStyle.Create(
            ArrowStyle.Create(),
            ArrowStyle.Create()));

Context.Editor.Project.Options.HelperStyle = helper;
