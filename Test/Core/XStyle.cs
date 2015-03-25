// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Core
{
    public class XStyle : XObject
    {
        private string _name;
        private XColor _stroke;
        private XColor _fill;
        private double _thickness;

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

        public XColor Stroke
        {
            get { return _stroke; }
            set
            {
                if (value != _stroke)
                {
                    _stroke = value;
                    Notify("Stroke");
                }
            }
        }

        public XColor Fill
        {
            get { return _fill; }
            set
            {
                if (value != _fill)
                {
                    _fill = value;
                    Notify("Fill");
                }
            }
        }

        public double Thickness
        {
            get { return _thickness; }
            set
            {
                if (value != _thickness)
                {
                    _thickness = value;
                    Notify("Thickness");
                }
            }
        }

        public static XStyle Create(
            string name,
            byte sa, byte sr, byte sg, byte sb,
            byte fa, byte fr, byte fg, byte fb,
            double thickness)
        {
            return new XStyle()
            {
                Name = name,
                Stroke = XColor.Create(sa, sr, sg, sb),
                Fill = XColor.Create(fa, fr, fg, fb),
                Thickness = thickness
            };
        }
    }
}
