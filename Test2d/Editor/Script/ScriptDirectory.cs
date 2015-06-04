// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ScriptDirectory : ObservableObject
    {
        private string _name;
        private string _path;
        private ImmutableArray<ScriptFile> _scripts;

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
        public ImmutableArray<ScriptFile> Scripts
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
                Scripts = ImmutableArray.Create<ScriptFile>()
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
            var builder = ImmutableArray.CreateBuilder<ScriptFile>();

            foreach (var file in files)
            {
                var sf = ScriptFile.Create(
                    System.IO.Path.GetFileNameWithoutExtension(file),
                    file);

                builder.Add(sf);
            }

            sd.Scripts = builder.ToImmutable();

            return sd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ImmutableArray<ScriptDirectory> CreateScriptDirectories(string path)
        {
            var builder = ImmutableArray.CreateBuilder<ScriptDirectory>();

            var root = CreateScriptDirectory(path);
            if (root != null)
            {
                builder.Add(root);
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
                    builder.Add(sub);
                }
            }

            return builder.ToImmutable();
        }
    }
}
