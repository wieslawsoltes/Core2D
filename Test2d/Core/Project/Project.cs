// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class Project : ObservableObject
    {
        private string _name;
        private Options _options;
        private ImmutableArray<Database> _databases;
        private Database _currentDatabase;
        private ImmutableArray<ShapeStyleGroup> _styleGroups;
        private ShapeStyleGroup _currentStyleGroup;
        private ImmutableArray<GroupLibrary> _groupLibraries;
        private GroupLibrary _currentGroupLibrary;
        private ImmutableArray<Container> _templates;
        private Container _currentTemplate;
        private ImmutableArray<Document> _documents;
        private Document _currentDocument;
        private Container _currentContainer;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Options Options
        {
            get { return _options; }
            set { Update(ref _options, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Database CurrentDatabase
        {
            get { return _currentDatabase; }
            set { Update(ref _currentDatabase, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Database> Databases
        {
            get { return _databases; }
            set { Update(ref _databases, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<ShapeStyleGroup> StyleGroups
        {
            get { return _styleGroups; }
            set { Update(ref _styleGroups, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShapeStyleGroup CurrentStyleGroup
        {
            get { return _currentStyleGroup; }
            set { Update(ref _currentStyleGroup, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<GroupLibrary> GroupLibraries
        {
            get { return _groupLibraries; }
            set { Update(ref _groupLibraries, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public GroupLibrary CurrentGroupLibrary
        {
            get { return _currentGroupLibrary; }
            set { Update(ref _currentGroupLibrary, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Container> Templates
        {
            get { return _templates; }
            set { Update(ref _templates, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Container CurrentTemplate
        {
            get { return _currentTemplate; }
            set { Update(ref _currentTemplate, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableArray<Document> Documents
        {
            get { return _documents; }
            set { Update(ref _documents, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Document CurrentDocument
        {
            get { return _currentDocument; }
            set { Update(ref _currentDocument, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Container CurrentContainer
        {
            get { return _currentContainer; }
            set { Update(ref _currentContainer, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ShapeStyleGroup DefaultStyleGroup()
        {
            var sgd = ShapeStyleGroup.Create("Default");

            var builder = sgd.Styles.ToBuilder();
            builder.Add(ShapeStyle.Create("Black", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
            builder.Add(ShapeStyle.Create("Yellow", 255, 255, 255, 0, 255, 255, 255, 0, 2.0));
            builder.Add(ShapeStyle.Create("Red", 255, 255, 0, 0, 255, 255, 0, 0, 2.0));
            builder.Add(ShapeStyle.Create("Green", 255, 0, 255, 0, 255, 0, 255, 0, 2.0));
            builder.Add(ShapeStyle.Create("Blue", 255, 0, 0, 255, 255, 0, 0, 255, 2.0));
            sgd.Styles = builder.ToImmutable();

            sgd.CurrentStyle = sgd.Styles.FirstOrDefault();

            return sgd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ShapeStyleGroup LinesStyleGroup()
        {
            var sgdl = ShapeStyleGroup.Create("Lines");

            var solid = ShapeStyle.Create("Solid", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            solid.LineStyle.Dashes = default(double[]);
            solid.LineStyle.DashOffset = 0.0;

            var dash = ShapeStyle.Create("Dash", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dash.LineStyle.Dashes = new double[] { 2, 2 };
            dash.LineStyle.DashOffset = 1.0;

            var dot = ShapeStyle.Create("Dot", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dot.LineStyle.Dashes = new double[] { 0, 2 };
            dot.LineStyle.DashOffset = 0.0;

            var dashDot = ShapeStyle.Create("DashDot", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dashDot.LineStyle.Dashes = new double[] { 2, 2, 0, 2 };
            dashDot.LineStyle.DashOffset = 1.0;

            var dashDotDot = ShapeStyle.Create("DashDotDot", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dashDotDot.LineStyle.Dashes = new double[] { 2, 2, 0, 2, 0, 2 };
            dashDotDot.LineStyle.DashOffset = 1.0;

            var builder = sgdl.Styles.ToBuilder();
            builder.Add(solid);
            builder.Add(dash);
            builder.Add(dot);
            builder.Add(dashDot);
            builder.Add(dashDotDot);
            sgdl.Styles = builder.ToImmutable();

            sgdl.CurrentStyle = sgdl.Styles.FirstOrDefault();

            return sgdl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ShapeStyleGroup TemplateStyleGroup()
        {
            var sgt = ShapeStyleGroup.Create("Template");
            var gs = ShapeStyle.Create("Grid", 255, 172, 172, 172, 255, 172, 172, 172, 1.0);

            var builder = sgt.Styles.ToBuilder();
            builder.Add(gs);
            sgt.Styles = builder.ToImmutable();

            sgt.CurrentStyle = sgt.Styles.FirstOrDefault();

            return sgt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Project Create(string name = "Project")
        {
            var p = new Project()
            {
                Name = name,
                Options = Options.Create(),
                Databases = ImmutableArray.Create<Database>(),
                StyleGroups = ImmutableArray.Create<ShapeStyleGroup>(),
                GroupLibraries = ImmutableArray.Create<GroupLibrary>(),
                Templates = ImmutableArray.Create<Container>(),
                Documents = ImmutableArray.Create<Document>(),
            };

            var glBuilder = p.GroupLibraries.ToBuilder();
            glBuilder.Add(GroupLibrary.Create("Default"));
            p.GroupLibraries = glBuilder.ToImmutable();

            var sgBuilder = p.StyleGroups.ToBuilder();
            sgBuilder.Add(DefaultStyleGroup());
            sgBuilder.Add(LinesStyleGroup());
            sgBuilder.Add(TemplateStyleGroup());
            p.StyleGroups = sgBuilder.ToImmutable();

            p.CurrentGroupLibrary = p.GroupLibraries.FirstOrDefault();
            p.CurrentStyleGroup = p.StyleGroups.FirstOrDefault();

            return p;
        }
    }
}
