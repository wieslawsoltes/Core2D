// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using System;
using System.Collections.Immutable;
using System.Reflection;
using Dxf;
using Test2d;
using TestSIM;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class RoslynScriptEngine : IScriptEngine
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="context"></param>
        public void Eval(string code, object context)
        {
            var options = Helpers.GetOptions();
            CSharpScript.Eval(
                code,
                options,
                new RoslynScriptGlobals(context as EditorContext));
        }
    }
}
