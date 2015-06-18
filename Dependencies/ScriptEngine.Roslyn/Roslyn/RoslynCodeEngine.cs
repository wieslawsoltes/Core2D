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
            _shapes = shapes.Where(s => s.Code != null && s.Code.IsExecutable).ToArray();

            _globals = new RoslynCodeGlobals()
            {
                Context = context as EditorContext,
                Shapes = _shapes,
                Runners = new CodeRunner[_shapes.Length]
            };

            var sw = Stopwatch.StartNew();

            // merge all shapes code as one big script
            var sb = new StringBuilder();

            for (int i = 0; i < _shapes.Length; i++)
            {
                // wrap shape Code in class object:
                //
                // public class Runner_{guid} : CodeRunner
                // {
                //     {Code.Definitions}
                //
                //     public Runner_{guid}(int id, BaseShape[] shapes, EditorContext context)
                //     {
                //         var shape = shapes[id] as {type};
                //         {Code.Initialization}
                //     }
                //
                //     public override void Run(int id, BaseShape[] shapes, EditorContext context)
                //     {
                //         var shape = shapes[id] as {type};
                //         {Code.Script}
                //     }
                // }
                
                // class name
                var name = string.Concat("Runner_", Guid.NewGuid().ToString("N").ToUpper());
                var type = _shapes[i].GetType().Name;

                // begin class
                sb.AppendLine(string.Concat("public class ", name, " : CodeRunner"));
                sb.AppendLine("{");

                // script definitions
                if (!string.IsNullOrWhiteSpace(_shapes[i].Code.Definitions))
                {
                    sb.AppendLine(_shapes[i].Code.Definitions);
                }

                // script initialization
                sb.AppendLine(string.Concat("public ", name, "(int id, BaseShape[] shapes, EditorContext context)"));
                sb.AppendLine("{");
                if (!string.IsNullOrWhiteSpace(_shapes[i].Code.Initialization))
                {
                    // define shape variable with proper type in constructor
                    sb.AppendLine(string.Concat("var shape = shapes[id] as ", type, ";"));
                    sb.AppendLine(_shapes[i].Code.Initialization);
                }
                sb.AppendLine("}");

                // begin run method
                sb.AppendLine("public override void Run(int id, BaseShape[] shapes, EditorContext context)");
                sb.AppendLine("{");

                // define shape variable with proper type in Run method
                sb.AppendLine(string.Concat("var shape = shapes[id] as ", type, ";"));

                // script code
                if (!string.IsNullOrWhiteSpace(_shapes[i].Code.Script))
                {
                    sb.AppendLine(_shapes[i].Code.Script);
                }

                // end run method
                sb.AppendLine("}");

                // end class
                sb.AppendLine("}");

                // create runner
                sb.AppendLine(string.Concat("Runners[", i, "] = ", "new ", name, "(", i, ", Shapes, Context);"));
            }

            // compile runners
            var runners = sb.ToString();
            //Debug.Print(runners);
            CSharpScript.Eval(runners, _options, _globals);

            // create main runner script
            _code = "for (int i = 0; i < Runners.Length; i++) if (Shapes[i].Code.IsExecutable) Runners[i].Run(i, Shapes, Context);";
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
