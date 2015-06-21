// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Test2d;

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
        private bool _haveRunners;
        private ScriptRunner _runners;
        private IDictionary<ShapeCode, int> _ids;

        private void FormatRunner(BaseShape shape, int id, StringBuilder sb)
        {
            // wrap shape Code into class object:

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
            var type = shape.GetType().Name;
            var code = shape.Code;

            // begin class
            sb.AppendLine(string.Concat("public class ", name, " : CodeRunner"));
            sb.AppendLine("{");

            // script definitions
            if (!string.IsNullOrWhiteSpace(code.Definitions))
            {
                sb.AppendLine(code.Definitions);
            }

            // script initialization
            sb.AppendLine(string.Concat("public ", name, "(int id, BaseShape[] shapes, EditorContext context)"));
            sb.AppendLine("{");
            if (!string.IsNullOrWhiteSpace(code.Initialization))
            {
                // define shape variable with proper type in constructor
                sb.AppendLine(string.Concat("var shape = shapes[id] as ", type, ";"));
                sb.AppendLine(code.Initialization);
            }
            sb.AppendLine("}");

            // begin run method
            sb.AppendLine("public override void Run(int id, BaseShape[] shapes, EditorContext context)");
            sb.AppendLine("{");

            // define shape variable with proper type in Run method
            sb.AppendLine(string.Concat("var shape = shapes[id] as ", type, ";"));

            // script code
            if (!string.IsNullOrWhiteSpace(code.Script))
            {
                sb.AppendLine(code.Script);
            }

            // end run method
            sb.AppendLine("}");

            // end class
            sb.AppendLine("}");

            // create runner
            sb.AppendLine(string.Concat("Runners[", id, "] = ", "new ", name, "(", id, ", Shapes, Context);"));
        }

        private void BuildRunner(ShapeCode code)
        {
            var sb = new StringBuilder();
            var id = _ids[code];
            var shape = _shapes[id];

            FormatRunner(shape, id, sb);

            var runner = sb.ToString();
            CSharpScript.Eval(runner, _options, _globals);
        }

        private void CodeObserver(object sender, PropertyChangedEventArgs e)
        {
            var code = sender as ShapeCode;
            if (code != null)
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    BuildRunner(code);

                    sw.Stop();
                    Debug.Print("Build: " + sw.Elapsed.TotalMilliseconds + "ms");
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }
        }

        private void StartObservingCode(BaseShape shape)
        {
            shape.Code.PropertyChanged += CodeObserver;
        }

        private void StopObservingCode(BaseShape shape)
        {
            shape.Code.PropertyChanged -= CodeObserver;
        }

        private void BuildRunners()
        {
            // merge all shapes code as one big script
            var sb = new StringBuilder();

            for (int i = 0; i < _shapes.Length; i++)
            {
                _ids.Add(_shapes[i].Code, i);
                FormatRunner(_shapes[i], i, sb);
            }

            // compile runners
            var runners = sb.ToString();
            CSharpScript.Eval(runners, _options, _globals);

            // create main runner script
            _code = "for (int i = 0; i < Runners.Length; i++) if (Shapes[i].Code.IsExecutable) Runners[i].Run(i, Shapes, Context);";
            _runners = CSharpScript.Create(_code, _options)
                .WithGlobalsType(typeof(RoslynCodeGlobals))
                .CreateDelegate();

            for (int i = 0; i < _shapes.Length; i++)
            {
                StartObservingCode(_shapes[i]);
            }
        }

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

            _ids = new Dictionary<ShapeCode, int>();

            var sw = Stopwatch.StartNew();

            BuildRunners();

            sw.Stop();
            Debug.Print("Build: " + sw.Elapsed.TotalMilliseconds + "ms");

            _haveRunners = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Run()
        {
            if (!_haveRunners)
                return;

            var sw = Stopwatch.StartNew();

            _runners(_globals);

            sw.Stop();
            Debug.Print("Run: " + sw.Elapsed.TotalMilliseconds + "ms");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            if (_shapes != null)
            {
                for (int i = 0; i < _shapes.Length; i++)
                {
                    StopObservingCode(_shapes[i]);
                }
            }

            _haveRunners = false;
            _options = null;
            _shapes = null;
            _globals = null;
            _code = null;
            _runners = null;
            _ids = null;
        }
    }
}
