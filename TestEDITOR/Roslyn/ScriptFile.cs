// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Test2d;

namespace TestEDITOR
{
    public class ScriptFile : ObservableObject
    {
        private string _name;
        private string _path;

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

        public string Path
        {
            get { return _path; }
            set
            {
                if (value != _path)
                {
                    _path = value;
                    Notify("Path");
                }
            }
        }

        public static ScriptFile Create(string name, string path)
        {
            return new ScriptFile()
            {
                Name = name,
                Path = path
            };
        }
    }
}
