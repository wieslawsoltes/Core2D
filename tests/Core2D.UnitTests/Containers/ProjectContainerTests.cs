// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;
using Xunit;

namespace Core2D.UnitTests
{
    public class ProjectContainerTests
    {
        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Inherits_From_Selectable()
        {
            var target = new ProjectContainer();
            Assert.True(target is SelectableObject);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Options_Not_Null()
        {
            var target = new ProjectContainer();
            Assert.NotNull(target.Options);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void StyleLibraries_Not_Null()
        {
            var target = new ProjectContainer();
            Assert.False(target.StyleLibraries.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void GroupLibraries_Not_Null()
        {
            var target = new ProjectContainer();
            Assert.False(target.GroupLibraries.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Databases_Not_Null()
        {
            var target = new ProjectContainer();
            Assert.False(target.Databases.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Templates_Not_Null()
        {
            var target = new ProjectContainer();
            Assert.False(target.Templates.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Documents_Not_Null()
        {
            var target = new ProjectContainer();
            Assert.False(target.Documents.IsDefault);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetCurrentDocument_Sets_CurrentDocument()
        {
            var target = new ProjectContainer();

            var document = DocumentContainer.Create();
            target.Documents = target.Documents.Add(document);

            target.SetCurrentDocument(document);

            Assert.Equal(document, target.CurrentDocument);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetCurrentContainer_Sets_CurrentContainer_And_Selected()
        {
            var target = new ProjectContainer();

            var document = DocumentContainer.Create();
            target.Documents = target.Documents.Add(document);

            var page = PageContainer.CreatePage();
            document.Pages = document.Pages.Add(page);

            target.SetCurrentContainer(page);

            Assert.Equal(page, target.CurrentContainer);
            Assert.Equal(page, target.Selected);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetCurrentTemplate_Sets_CurrentTemplate()
        {
            var target = new ProjectContainer();

            var template = PageContainer.CreateTemplate();
            target.Templates = target.Templates.Add(template);

            target.SetCurrentTemplate(template);

            Assert.Equal(template, target.CurrentTemplate);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetCurrentDatabase_Sets_CurrentDatabase()
        {
            var target = new ProjectContainer();

            var db = Database.Create("Db");
            target.Databases = target.Databases.Add(db);

            target.SetCurrentDatabase(db);

            Assert.Equal(db, target.CurrentDatabase);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetCurrentGroupLibrary_Sets_CurrentGroupLibrary()
        {
            var target = new ProjectContainer();

            var library = Library<GroupShape>.Create("Library1");
            target.GroupLibraries = target.GroupLibraries.Add(library);

            target.SetCurrentGroupLibrary(library);

            Assert.Equal(library, target.CurrentGroupLibrary);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetCurrentStyleLibrary_Sets_CurrentStyleLibrary()
        {
            var target = new ProjectContainer();

            var library = Library<ShapeStyle>.Create("Library1");
            target.StyleLibraries = target.StyleLibraries.Add(library);

            target.SetCurrentStyleLibrary(library);

            Assert.Equal(library, target.CurrentStyleLibrary);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetSelected_Layer()
        {
            var target = new ProjectContainer();

            var page = new PageContainer();
            var layer = LayerContainer.Create("Layer1", page);

            target.SetSelected(layer);

            Assert.Equal(layer, page.CurrentLayer);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetSelected_Container()
        {
            var target = new ProjectContainer();

            var document = DocumentContainer.Create();
            target.Documents = target.Documents.Add(document);

            var page = PageContainer.CreatePage();
            document.Pages = document.Pages.Add(page);

            var layer = LayerContainer.Create("Layer1", page);
            page.Layers = page.Layers.Add(layer);

            bool raised = false;

            layer.InvalidateLayer += (sender, e) =>
            {
                raised = true;
            };

            target.SetSelected(page);

            Assert.Equal(document, target.CurrentDocument);
            Assert.Equal(page, target.CurrentContainer);
            Assert.True(raised);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void SetSelected_Document()
        {
            var target = new ProjectContainer();

            var document = DocumentContainer.Create();
            target.Documents = target.Documents.Add(document);

            target.SetSelected(document);

            Assert.Equal(document, target.CurrentDocument);
        }

        [Fact]
        [Trait("Core2D.Containers", "Project")]
        public void Setting_Selected_Should_Call_SetSelected()
        {
            var target = new ProjectContainer();

            var document = DocumentContainer.Create();
            target.Documents = target.Documents.Add(document);

            target.Selected = document;

            Assert.Equal(document, target.CurrentDocument);
        }
    }
}
