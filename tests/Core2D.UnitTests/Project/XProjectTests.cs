// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data.Database;
using Core2D.Project;
using Core2D.Shapes;
using Core2D.Style;
using Xunit;

namespace Core2D.UnitTests
{
    public class XProjectTests
    {
        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Inherits_From_Selectable()
        {
            var target = new XProject();
            Assert.True(target is XSelectable);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Options_Not_Null()
        {
            var target = new XProject();
            Assert.NotNull(target.Options);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void StyleLibraries_Not_Null()
        {
            var target = new XProject();
            Assert.NotNull(target.StyleLibraries);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void GroupLibraries_Not_Null()
        {
            var target = new XProject();
            Assert.NotNull(target.GroupLibraries);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Databases_Not_Null()
        {
            var target = new XProject();
            Assert.NotNull(target.Databases);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Templates_Not_Null()
        {
            var target = new XProject();
            Assert.NotNull(target.Templates);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Documents_Not_Null()
        {
            var target = new XProject();
            Assert.NotNull(target.Documents);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void SetCurrentDocument_Sets_CurrentDocument()
        {
            var target = new XProject();

            var document = XDocument.Create();
            target.Documents = target.Documents.Add(document);

            target.SetCurrentDocument(document);

            Assert.Equal(document, target.CurrentDocument);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void SetCurrentContainer_Sets_CurrentContainer_And_Selected()
        {
            var target = new XProject();

            var document = XDocument.Create();
            target.Documents = target.Documents.Add(document);

            var page = XContainer.CreatePage();
            document.Pages = document.Pages.Add(page);

            target.SetCurrentContainer(page);

            Assert.Equal(page, target.CurrentContainer);
            Assert.Equal(page, target.Selected);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void SetCurrentTemplate_Sets_CurrentTemplate()
        {
            var target = new XProject();

            var template = XContainer.CreateTemplate();
            target.Templates = target.Templates.Add(template);

            target.SetCurrentTemplate(template);

            Assert.Equal(template, target.CurrentTemplate);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void SetCurrentDatabase_Sets_CurrentDatabase()
        {
            var target = new XProject();

            var db = XDatabase.Create("Db");
            target.Databases = target.Databases.Add(db);

            target.SetCurrentDatabase(db);

            Assert.Equal(db, target.CurrentDatabase);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void SetCurrentGroupLibrary_Sets_CurrentGroupLibrary()
        {
            var target = new XProject();

            var library = XLibrary<GroupShape>.Create("Library1");
            target.GroupLibraries = target.GroupLibraries.Add(library);

            target.SetCurrentGroupLibrary(library);

            Assert.Equal(library, target.CurrentGroupLibrary);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void SetCurrentStyleLibrary_Sets_CurrentStyleLibrary()
        {
            var target = new XProject();

            var library = XLibrary<ShapeStyle>.Create("Library1");
            target.StyleLibraries = target.StyleLibraries.Add(library);

            target.SetCurrentStyleLibrary(library);

            Assert.Equal(library, target.CurrentStyleLibrary);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void SetSelected_Layer()
        {
            var target = new XProject();

            var page = new XContainer();
            var layer = XLayer.Create("Layer1", page);

            target.SetSelected(layer);

            Assert.Equal(layer, page.CurrentLayer);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void SetSelected_Container()
        {
            var target = new XProject();

            var document = XDocument.Create();
            target.Documents = target.Documents.Add(document);

            var page = XContainer.CreatePage();
            document.Pages = document.Pages.Add(page);

            var layer = XLayer.Create("Layer1", page);
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
        [Trait("Core2D.Project", "Project")]
        public void SetSelected_Document()
        {
            var target = new XProject();

            var document = XDocument.Create();
            target.Documents = target.Documents.Add(document);

            target.SetSelected(document);

            Assert.Equal(document, target.CurrentDocument);
        }

        [Fact]
        [Trait("Core2D.Project", "Project")]
        public void Setting_Selected_Should_Call_SetSelected()
        {
            var target = new XProject();

            var document = XDocument.Create();
            target.Documents = target.Documents.Add(document);

            target.Selected = document;

            Assert.Equal(document, target.CurrentDocument);
        }
    }
}
