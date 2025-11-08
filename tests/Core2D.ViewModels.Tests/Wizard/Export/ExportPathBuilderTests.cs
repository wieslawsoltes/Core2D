// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using IOPath = System.IO.Path;
using Core2D.ViewModels.Wizard.Export;
using Core2D.ViewModels.Wizard.Export.Destination;
using Xunit;

namespace Core2D.ViewModels.Tests.Wizard.Export;

public class ExportPathBuilderTests
{
    [Fact]
    public void BuildPath_ComposesDestinationFolderAndTemplates()
    {
        var context = new ExportWizardContext(null)
        {
            DestinationFolder = IOPath.Combine("out", "exports"),
            SubfolderTemplate = "{ProjectName}",
            FileNameTemplate = "{DocumentName}_{PageName}.{ExporterExtension}"
        };

        var scope = ExportTestHelpers.CreateScope("Project Alpha", "Doc Zero", "Page Zero");
        var exporter = ExportTestHelpers.CreateExporter("Vector PDF", ".pdf");

        var path = ExportPathBuilder.BuildPath(context, scope, exporter);

        var expected = IOPath.Combine(context.DestinationFolder, "Project Alpha", "Doc Zero_Page Zero.pdf");
        Assert.Equal(expected, path);
    }

    [Fact]
    public void BuildPath_ReturnsRelativeWhenDestinationMissing()
    {
        var context = new ExportWizardContext(null)
        {
            DestinationFolder = string.Empty,
            SubfolderTemplate = "{Exporter}",
            FileNameTemplate = "{PageName}.{ExporterExtension}"
        };

        var scope = ExportTestHelpers.CreateScope("Project", "Doc", "Page");
        var exporter = ExportTestHelpers.CreateExporter("Image", ".png");

        var path = ExportPathBuilder.BuildPath(context, scope, exporter);

        var expected = IOPath.Combine("Image", "Page.png");
        Assert.Equal(expected, path);
    }

    [Fact]
    public void BuildPath_ReplacesPageNumberToken()
    {
        var context = new ExportWizardContext(null)
        {
            DestinationFolder = "out",
            SubfolderTemplate = string.Empty,
            FileNameTemplate = "{ProjectName}_{PageNumber}.{ExporterExtension}"
        };

        var scope = ExportTestHelpers.CreateScope("Project Alpha", "Doc Zero", "Page Zero");
        var exporter = ExportTestHelpers.CreateExporter("Vector PDF", ".pdf");

        var path = ExportPathBuilder.BuildPath(context, scope, exporter);

        var expected = IOPath.Combine("out", "Project Alpha_1.pdf");
        Assert.Equal(expected, path);
    }
}
