// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using System;
using Test2d;

namespace TestEDITOR
{
    /// <summary>
    /// 
    /// </summary>
    internal struct RoslynScriptCache
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly BaseShape Shape;

        /// <summary>
        /// 
        /// </summary>
        public readonly Script Script;

        /// <summary>
        /// 
        /// </summary>
        public readonly RoslynShapeGlobals Globals;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="script"></param>
        /// <param name="context"></param>
        public RoslynScriptCache(BaseShape shape, Script script, object context)
        {
            Shape = shape;
            Script = script;
            Globals = new RoslynShapeGlobals()
            {
                Context = context as EditorContext,
                Shape = shape
            };
        }
    }
}
