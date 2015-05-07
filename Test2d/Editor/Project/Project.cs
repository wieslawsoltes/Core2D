// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    public class Project : ObservableObject
    {
        private string _name;
        private IList<ShapeStyleGroup> _styleGroups;
        private ShapeStyleGroup _currentStyleGroup;
        private IList<GroupLibrary> _groupLibraries;
        private GroupLibrary _currentGroupLibrary;
        private IList<Container> _templates;
        private Container _currentTemplate;
        private BaseShape _pointShape;
        private IList<Document> _documents;
        private Document _currentDocument;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    Notify("Name");
                }
            }
        }

        public IList<ShapeStyleGroup> StyleGroups
        {
            get { return _styleGroups; }
            set
            {
                if (value != _styleGroups)
                {
                    _styleGroups = value;
                    Notify("StyleGroups");
                }
            }
        }

        public ShapeStyleGroup CurrentStyleGroup
        {
            get { return _currentStyleGroup; }
            set
            {
                if (value != _currentStyleGroup)
                {
                    _currentStyleGroup = value;
                    Notify("CurrentStyleGroup");
                }
            }
        }

        public IList<GroupLibrary> GroupLibraries
        {
            get { return _groupLibraries; }
            set
            {
                if (value != _groupLibraries)
                {
                    _groupLibraries = value;
                    Notify("GroupLibraries");
                }
            }
        }

        public GroupLibrary CurrentGroupLibrary
        {
            get { return _currentGroupLibrary; }
            set
            {
                if (value != _currentGroupLibrary)
                {
                    _currentGroupLibrary = value;
                    Notify("CurrentGroupLibrary");
                }
            }
        }

        public IList<Container> Templates
        {
            get { return _templates; }
            set
            {
                if (value != _templates)
                {
                    _templates = value;
                    Notify("Templates");
                }
            }
        }

        public Container CurrentTemplate
        {
            get { return _currentTemplate; }
            set
            {
                if (value != _currentTemplate)
                {
                    _currentTemplate = value;
                    Notify("CurrentTemplate");
                }
            }
        }

        public BaseShape PointShape
        {
            get { return _pointShape; }
            set
            {
                if (value != _pointShape)
                {
                    _pointShape = value;
                    Notify("PointShape");
                }
            }
        }

        public IList<Document> Documents
        {
            get { return _documents; }
            set
            {
                if (value != _documents)
                {
                    _documents = value;
                    Notify("Documents");
                }
            }
        }

        public Document CurrentDocument
        {
            get { return _currentDocument; }
            set
            {
                if (value != _currentDocument)
                {
                    _currentDocument = value;
                    Notify("CurrentDocument");
                }
            }
        }

        public static Project Create(string name = "Project")
        {
            var p = new Project()
            {
                Name = name,
                StyleGroups = new ObservableCollection<ShapeStyleGroup>(),
                GroupLibraries = new ObservableCollection<GroupLibrary>(),
                Templates = new ObservableCollection<Container>(),
                Documents = new ObservableCollection<Document>(),
            };

            // default group library
            var gld = GroupLibrary.Create("Default");
            p.GroupLibraries.Add(gld);
            p.CurrentGroupLibrary = p.GroupLibraries.FirstOrDefault();

            // default styles group
            var sgd = ShapeStyleGroup.Create("Default");
            sgd.Styles.Add(ShapeStyle.Create("Black", 255, 0, 0, 0, 255, 0, 0, 0, 2.0));
            sgd.Styles.Add(ShapeStyle.Create("Yellow", 255, 255, 255, 0, 255, 255, 255, 0, 2.0));
            sgd.Styles.Add(ShapeStyle.Create("Red", 255, 255, 0, 0, 255, 255, 0, 0, 2.0));
            sgd.Styles.Add(ShapeStyle.Create("Green", 255, 0, 255, 0, 255, 0, 255, 0, 2.0));
            sgd.Styles.Add(ShapeStyle.Create("Blue", 255, 0, 0, 255, 255, 0, 0, 255, 2.0));
            sgd.CurrentStyle = sgd.Styles.FirstOrDefault();

            p.StyleGroups.Add(sgd);
            p.CurrentStyleGroup = p.StyleGroups.FirstOrDefault();

            // dashed lines styles group
            var sgdl = ShapeStyleGroup.Create("Lines");

            var solid = ShapeStyle.Create("Solid", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            solid.LineStyle.Dashes = null;
            solid.LineStyle.DashOffset = 0.0;
            sgdl.Styles.Add(solid);

            var dash = ShapeStyle.Create("Dash", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dash.LineStyle.Dashes = new double[] { 2, 2 };
            dash.LineStyle.DashOffset = 1.0;
            sgdl.Styles.Add(dash);

            var dot = ShapeStyle.Create("Dot", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dot.LineStyle.Dashes = new double[] { 0, 2 };
            dot.LineStyle.DashOffset = 0.0;
            sgdl.Styles.Add(dot);

            var dashDot = ShapeStyle.Create("DashDot", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dashDot.LineStyle.Dashes = new double[] { 2, 2, 0, 2 };
            dashDot.LineStyle.DashOffset = 1.0;
            sgdl.Styles.Add(dashDot);

            var dashDotDot = ShapeStyle.Create("DashDotDot", 255, 0, 0, 0, 255, 0, 0, 0, 2.0);
            dashDotDot.LineStyle.Dashes = new double[] { 2, 2, 0, 2, 0, 2 };
            dashDotDot.LineStyle.DashOffset = 1.0;
            sgdl.Styles.Add(dashDotDot);

            sgdl.CurrentStyle = sgdl.Styles.FirstOrDefault();
            p.StyleGroups.Add(sgdl);

            // template styles group
            var sgt = ShapeStyleGroup.Create("Template");
            var pss = ShapeStyle.Create("PointShape", 255, 255, 0, 0, 255, 255, 0, 0, 2.0);
            var gs = ShapeStyle.Create("Grid", 255, 172, 172, 172, 255, 172, 172, 172, 1.0);
            sgt.Styles.Add(pss);
            sgt.Styles.Add(gs);
            p.StyleGroups.Add(sgt);
            sgt.CurrentStyle = sgt.Styles.FirstOrDefault();

            CrossPointShape(p, pss);

            return p;
        }

        public static void EllipsePointShape(Project p, ShapeStyle pss)
        {
            p.PointShape = XEllipse.Create(-4, -4, 4, 4, pss, null, false);
        }

        public static void FilledEllipsePointShape(Project p, ShapeStyle pss)
        {
            p.PointShape = XEllipse.Create(-3, -3, 3, 3, pss, null, true);
        }

        public static void RectanglePointShape(Project p, ShapeStyle pss)
        {
            p.PointShape = XRectangle.Create(-4, -4, 4, 4, pss, null, false);
        }

        public static void FilledRectanglePointShape(Project p, ShapeStyle pss)
        {
            p.PointShape = XRectangle.Create(-3, -3, 3, 3, pss, null, true);
        }

        public static void CrossPointShape(Project p, ShapeStyle pss)
        {
            var g = XGroup.Create("PointShape");
            g.Shapes.Add(XLine.Create(-4, 0, 4, 0, pss, null));
            g.Shapes.Add(XLine.Create(0, -4, 0, 4, pss, null));
            p.PointShape = g;
        }
    }
}
