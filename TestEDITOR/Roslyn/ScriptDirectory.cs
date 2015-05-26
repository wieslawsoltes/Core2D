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
    /// <summary>
    /// 
    /// </summary>
    public class ScriptDirectory : ObservableObject
    {
        private string _name;
        private string _path;
        private IList<ScriptFile> _scripts;

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
        public string Path
        {
            get { return _path; }
            set { Update(ref _path, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<ScriptFile> Scripts
        {
            get { return _scripts; }
            set { Update(ref _scripts, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ScriptDirectory Create(string name, string path)
        {
            return new ScriptDirectory()
            {
                Name = name,
                Path = path,
                Scripts = new ObservableCollection<ScriptFile>()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
