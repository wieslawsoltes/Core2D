// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Core2D.Editor;
using Core2D.Interfaces;

namespace Core2D.ScriptRunner.Roslyn
{
    /// <inheritdoc/>
    public class RoslynScriptRunner : IScriptRunner
    {
        private readonly IServiceProvider _serviceProvider;

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
            ScriptState<object> state = null;

            var options = ScriptOptions.Default
                .WithImports("System");

            var globals = new ScriptBase 
            { 
                Editor = _serviceProvider.GetService<ProjectEditor>() 
            }

            try
            {
                state = CSharpScript.RunAsync(code, options, globals).Result;
            }
            catch (CompilationErrorException e)
            {
                Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
            }
        }
    }
}
