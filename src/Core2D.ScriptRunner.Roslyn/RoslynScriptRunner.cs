// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Editor;
using Core2D.Interfaces;

namespace Core2D.ScriptRunner.Roslyn
{
    /// <inheritdoc/>
    public class RoslynScriptRunner : IScriptRunner
    {
        private readonly IServiceProvider _serviceProvider;

        private static string HostClassHeader =
            @"
            using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Diagnostics;
            using System.IO;
            using System.Linq;
            using System.Reflection;
            using System.Text;
            using System.Threading.Tasks;

            using Spatial;
            using Spatial.Arc;
            using Spatial.ConvexHull;
            using Spatial.Sat;

            using Core2D;
            using Core2D.Attributes;
            using Core2D.Containers;
            using Core2D.Data;
            using Core2D.History;
            using Core2D.Interfaces;
            using Core2D.Path;
            using Core2D.Path.Segments;
            using Core2D.Renderer;
            using Core2D.Renderer.Presenters;
            using Core2D.Shape;
            using Core2D.Shapes;
            using Core2D.Shapes.Interfaces;
            using Core2D.Style;

            using Core2D.Editor;
            using Core2D.Editor.Bounds;
            using Core2D.Editor.Designer;
            using Core2D.Editor.Factories;
            using Core2D.Editor.Input;
            using Core2D.Editor.Recent;
            using Core2D.Editor.Path;
            using Core2D.Editor.Tools;
            using Core2D.Editor.Tools.Path;
            using Core2D.Editor.Tools.Path.Settings;
            using Core2D.Editor.Tools.Path.Shapes;
            using Core2D.Editor.Tools.Selection;
            using Core2D.Editor.Tools.Settings;
            using Core2D.Editor.Views;
            using Core2D.Editor.Views.Interfaces;

            public class ScriptHost : ScriptBase 
            { 
                public override void Run() 
                {
            ";

        private static string HostClassFooter =
            @"
                } 
            }";

        /// <summary>
        /// Initialize new instance of <see cref="RoslynScriptRunner"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public RoslynScriptRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public void Execute(string code)
        {
            string host = string.Concat(HostClassHeader, code, HostClassFooter);
            IEnumerable<ScriptBase> scripts = ScriptImporter.Import<ScriptBase>(host);
            if (scripts != null)
            {
                foreach (var script in scripts)
                {
                    script.Editor = _serviceProvider.GetService<ProjectEditor>();
                    script.Run();
                }
            }
        }
    }
}
