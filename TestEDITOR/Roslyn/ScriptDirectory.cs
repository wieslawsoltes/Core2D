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
    public class ScriptDirectory : ObservableObject
    {
        private string _name;
        private string _path;
        private IList<ScriptFile> _scripts;

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

        public IList<ScriptFile> Scripts
        {
            get { return _scripts; }
            set
            {
                if (value != _scripts)
                {
                    _scripts = value;
                    Notify("Scripts");
                }
            }
        }

        public static ScriptDirectory Create(string name, string path)
        {
            return new ScriptDirectory()
            {
                Name = name,
                Path = path,
                Scripts = new ObservableCollection<ScriptFile>()
            };
        }

        public static ScriptDirectory CreateScriptDirectory(string path)
        {
            var files = System.IO.Directory.EnumerateFiles(
                path,
                "*.cs",
                System.IO.SearchOption.TopDirectoryOnly);

            if (files.Count() <= 0)
            {
                return null;
            }

            var sd = ScriptDirectory.Create(System.IO.Path.GetFileName(path), path);

            foreach (var file in files)
            {
                var sf = ScriptFile.Create(
                    System.IO.Path.GetFileNameWithoutExtension(file),
                    file);

                sd.Scripts.Add(sf);
            }

            return sd;
        }

        public static IList<ScriptDirectory> CreateScriptDirectories(string path)
        {
            var sds = new ObservableCollection<ScriptDirectory>();

            var root = CreateScriptDirectory(path);
            if (root != null)
            {
                sds.Add(root);
            }

            var dirs = System.IO.Directory.EnumerateDirectories(
                path,
                "*",
                System.IO.SearchOption.TopDirectoryOnly);

            foreach (var dir in dirs)
            {
                var sub = CreateScriptDirectory(dir);
                if (sub != null)
                {
                    sds.Add(sub);
                }
            }

            return sds;
        }
    }
}
