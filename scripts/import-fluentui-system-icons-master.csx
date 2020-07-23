// https://github.com/microsoft/fluentui-system-icons

#r "Core2D"
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core2D.Containers;
using Core2D.Editor;
using Core2D.Layout;
using Core2D.Shapes;

var assetsBasePath =  @"D:\fluentui-system-icons-master\assets\";
var projectBasePath = @"D:\";
var projectName = "fluentui-system-icons-master";

var project = await Task.Run(() => ImportIcons(assetsBasePath, projectName));

var projectPath = Path.Combine(projectBasePath, "{projectName}.project");
Factory.SaveProjectContainer(project, projectPath, FileIO, JsonSerializer);

OnUnload();
OnLoad(project, projectPath);

IProjectContainer ImportIcons(string assetsBasePath, string projectName)
{
    var assetsDirectories = Directory.EnumerateDirectories(assetsBasePath);
    var project = Factory.CreateProjectContainer(projectName);

    foreach (var assetDirectory in assetsDirectories)
    {
        var name = new DirectoryInfo(assetDirectory).Name;
        var document = Factory.CreateDocumentContainer(name);

        document.IsExpanded = false;

        project.Documents = project.Documents.Add(document);

        var svgIconPaths = Directory.EnumerateFiles(Path.Combine(assetDirectory, "SVG"), "*.svg");

        foreach (var svgIconPath in svgIconPaths)
        {
            ImportSvg(svgIconPath, document);
        }
    }

    return project;
}

IPageContainer CreateTemplate(double width, double height)
{
    var template = Factory.CreateTemplateContainer(width: width, height: height);

    template.Background = Factory.CreateArgbColor(0x00, 0xFF, 0xFF, 0xFF);
    template.IsGridEnabled = true;
    template.IsBorderEnabled = true;
    template.GridOffsetLeft = 0.0;
    template.GridOffsetTop = 0.0;
    template.GridOffsetRight = 0.0;
    template.GridOffsetBottom = 0.0;
    template.GridCellWidth = 1.0;
    template.GridCellHeight = 1.0;
    template.GridStrokeColor = Factory.CreateArgbColor(0xFF, 0xDE, 0xDE, 0xDE);
    template.GridStrokeThickness = 1.0; 

    return template;
}

void ImportSvg(string svgIconPath, IDocumentContainer document)
{
    var group = SvgConverter.Convert(svgIconPath, out var width, out var height)?.FirstOrDefault() as IGroupShape;
    if (group == null)
    {
        return;
    }

    var name = Path.GetFileNameWithoutExtension(svgIconPath);
    var page = Factory.CreatePageContainer(name);
    page.Template = CreateTemplate((double)width, (double)height);

    var layer = page.CurrentLayer;
    layer.Shapes = layer.Shapes.Add(group);

    document.Pages = document.Pages.Add(page);
}
