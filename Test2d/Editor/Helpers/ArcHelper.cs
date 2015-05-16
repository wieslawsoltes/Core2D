// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ArcHelper
    {
        private Project _project;
        private ShapeStyle _arcHelperStyle;
        private XLine _arcStartHelperLine;
        private XLine _arcEndHelperLine;
        private XEllipse _arcHelperEllipse;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        public ArcHelper(Project project)
        {
            _project = project;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ToStateOne()
        {
            _arcHelperStyle = ShapeStyle.Create(
                "ArcHelper",
                0xFF, 0xA9, 0xA9, 0xA9,
                0xFF, 0xA9, 0xA9, 0xA9,
                _project.CurrentStyleGroup.CurrentStyle.Thickness);
            _arcHelperStyle.LineStyle.Dashes = new double[] { 2, 2 };
            _arcHelperStyle.LineStyle.DashOffset = 1.0;

            _arcHelperEllipse = XEllipse.Create(0, 0, _arcHelperStyle, null);
            _project.CurrentContainer.WorkingLayer.Shapes.Add(_arcHelperEllipse);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ToStateTwo()
        {
            _arcStartHelperLine = XLine.Create(0, 0, _arcHelperStyle, null);
            _project.CurrentContainer.WorkingLayer.Shapes.Add(_arcStartHelperLine);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ToStateThree()
        {
            _arcEndHelperLine = XLine.Create(0, 0, _arcHelperStyle, null);
            _project.CurrentContainer.WorkingLayer.Shapes.Add(_arcEndHelperLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        public void Move(XArc arc)
        {
            var a = WpfArc.FromXArc(arc, 0, 0);

            if (_arcHelperEllipse != null)
            {
                _arcHelperEllipse.TopLeft.X = a.P1.X;
                _arcHelperEllipse.TopLeft.Y = a.P1.Y;
                _arcHelperEllipse.BottomRight.X = a.P2.X;
                _arcHelperEllipse.BottomRight.Y = a.P2.Y;
            }

            if (_arcStartHelperLine != null)
            {
                _arcStartHelperLine.Start.X = a.Center.X;
                _arcStartHelperLine.Start.Y = a.Center.Y;
                _arcStartHelperLine.End.X = a.Start.X;
                _arcStartHelperLine.End.Y = a.Start.Y;
            }

            if (_arcEndHelperLine != null)
            {
                _arcEndHelperLine.Start.X = a.Center.X;
                _arcEndHelperLine.Start.Y = a.Center.Y;
                _arcEndHelperLine.End.X = a.End.X;
                _arcEndHelperLine.End.Y = a.End.Y;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        public void Finalize(XArc arc)
        {
            var a = WpfArc.FromXArc(arc, 0, 0);
            arc.Point3.X = a.Start.X;
            arc.Point3.Y = a.Start.Y;
            arc.Point4.X = a.End.X;
            arc.Point4.Y = a.End.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Remove()
        {
            if (_arcHelperEllipse != null)
            {
                _project.CurrentContainer.WorkingLayer.Shapes.Remove(_arcHelperEllipse);
                _arcHelperEllipse = null;
            }

            if (_arcStartHelperLine != null)
            {
                _project.CurrentContainer.WorkingLayer.Shapes.Remove(_arcStartHelperLine);
                _arcStartHelperLine = null;
            }

            if (_arcEndHelperLine != null)
            {
                _project.CurrentContainer.WorkingLayer.Shapes.Remove(_arcEndHelperLine);
                _arcEndHelperLine = null;
            }

            _arcHelperStyle = null;
        }
    }
}
