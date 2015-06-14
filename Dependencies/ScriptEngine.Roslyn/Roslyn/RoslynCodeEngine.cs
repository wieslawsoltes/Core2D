// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using Dxf;
using Test2d;
using TestSIM;

namespace TestEDITOR
{
    /// <summary>
    /// 
    /// </summary>
    public class RoslynCodeEngine : ICodeEngine
    {
        private bool _haveCache;
        private ImmutableArray<RoslynScriptCache> _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="context"></param>
        public void Build(ImmutableArray<BaseShape> shapes, object context)
        {
            var options = Helpers.GetOptions();
            var cache = ImmutableArray.CreateBuilder<RoslynScriptCache>();

            foreach (var shape in shapes)
            {
                if (!shape.IsExecutable || string.IsNullOrEmpty(shape.Code))
                    continue;

                var script = CSharpScript.Create(shape.Code, options);
                script.Build();

                cache.Add(new RoslynScriptCache(shape, script, context));
            }

            _cache = cache.ToImmutableArray();
            _haveCache = true;
        }

        public void Run()
        {
            if (!_haveCache)
                return;

            foreach (var item in _cache)
            {
                item.Script.Run(item.Globals);
            }
        }

        public void Reset()
        {
            _haveCache = false;
            _cache = ImmutableArray<RoslynScriptCache>.Empty;
        }
    }
}
