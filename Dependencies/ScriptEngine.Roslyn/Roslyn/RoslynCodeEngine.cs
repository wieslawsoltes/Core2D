// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Dxf;
using Test2d;
using TestSIM;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class RoslynCodeEngine : ICodeEngine
    {
        private ScriptOptions _options;
        private BaseShape[] _shapes;
        private RoslynCodeGlobals _globals;
        private string _code;
        private bool _haveRunner;
        private ScriptRunner _runner;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="context"></param>
        public void Build(ImmutableArray<BaseShape> shapes, object context)
        {
            _options = Helpers.GetOptions();
            _shapes = shapes.Where(s => s.IsExecutable).ToArray();

            _globals = new RoslynCodeGlobals()
            {
                Context = context as EditorContext,
                Shapes = _shapes,
                Runners = new CodeRunner[_shapes.Length]
            };

            var sw = Stopwatch.StartNew();

            // merge all shapes code as one big script
            
            for (int i = 0; i < _shapes.Length; i++)
            {
                // wrap shape Code and Data in a block and define Shape variable as its own type
                var sb = new StringBuilder();

                // class name
                var name = string.Concat("Runner_", Guid.NewGuid().ToString("N").ToUpper());

                // begin class
                sb.AppendLine(string.Concat("public class ", name, " : CodeRunner"));
                sb.AppendLine("{");

                // shape property
                sb.AppendLine(string.Concat("public ", name, "()"));
                sb.AppendLine("{");
                sb.AppendLine("}");

                // shape data
                if (!string.IsNullOrWhiteSpace(_shapes[i].Data))
                {
                    sb.AppendLine(_shapes[i].Data);
                }

                // begin run method
                sb.AppendLine("public override void Run(int id, BaseShape[] shapes, EditorContext context)");
                sb.AppendLine("{");

                // define local shape variable with proper type
                var type = _shapes[i].GetType().Name;
                sb.AppendLine(string.Concat("var shape = shapes[id] as ", type, ";"));

                // shape code
                if (!string.IsNullOrWhiteSpace(_shapes[i].Code))
                {
                    sb.AppendLine(_shapes[i].Code);
                }

                // end run method
                sb.AppendLine("}");

                // end class
                sb.AppendLine("}");

                // create runner
                sb.AppendLine(string.Concat("new ", name, "();"));

                // evaluate shape code and set runner
                var code = sb.ToString();

                //Debug.Print(code);

                var runner = CSharpScript.Eval(code, _options, _globals);
                _globals.Runners[i] = runner as CodeRunner;
            }

            _code = "for (int i = 0; i < Runners.Length; i++) Runners[i].Run(i, Shapes, Context);";
            _runner = CSharpScript.Create(_code, _options)
                .WithGlobalsType(typeof(RoslynCodeGlobals))
                .CreateDelegate();

            sw.Stop();
            Debug.Print("Build: " + sw.Elapsed.TotalMilliseconds + "ms");

            _haveRunner = true;
        }

        public void Run()
        {
            if (!_haveRunner)
                return;

            var sw = Stopwatch.StartNew();

            _runner(_globals);

            sw.Stop();
            Debug.Print("Run: " + sw.Elapsed.TotalMilliseconds + "ms");
        }

        public void Reset()
        {
            _haveRunner = false;
            _options = null;
            _shapes = null;
            _globals = null;
            _code = null;
        }
    }
}
