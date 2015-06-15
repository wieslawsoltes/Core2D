// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
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
        private RoslynCodeGlobals<object> _globals;
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
            _shapes = shapes.Where(s => s.IsExecutable && !string.IsNullOrEmpty(s.Code)).ToArray();

            _globals = new RoslynCodeGlobals<object>()
            {
                Context = context as EditorContext,
                Shapes = _shapes,
                States = new object[_shapes.Length]
            };

            // merge all shapes code as one big script
            var sb = new StringBuilder();
            for (int i = 0; i < _shapes.Length; i++)
            {
                // wrap shape Code in a block and define Shape variable as its own type
                sb.AppendLine("{");
                sb.AppendLine(string.Concat("var Id = ", i, ";"));
                sb.AppendLine(string.Concat("var State = States[", i, "];"));
                sb.AppendLine(string.Concat("var Shape = Shapes[", i, "] as ", _shapes[i].GetType().Name, ";"));
                sb.AppendLine(_shapes[i].Code);
                sb.AppendLine("}");
            }
            _code = sb.ToString();

            _runner = CSharpScript.Create(_code, _options).WithGlobalsType(typeof(RoslynCodeGlobals<object>)).CreateDelegate();
            _haveRunner = true;
        }

        public void Run()
        {
            if (!_haveRunner)
                return;

            _runner(_globals);
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
