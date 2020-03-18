// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#if !_CORERT
using System;
using Core2D.Editor;
using Core2D.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

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

            var globals = new EditorScript
            {
                Editor = _serviceProvider.GetService<IProjectEditor>()
            };

            try
            {
                state = CSharpScript.RunAsync(code, options, globals).Result;
            }
            catch (CompilationErrorException ex)
            {
                var log = _serviceProvider.GetService<ILog>();
                log?.LogException(ex);
                log?.LogError($"{Environment.NewLine}{ex.Diagnostics}");
            }
        }

        /// <inheritdoc/>
        public object Execute(string code, object state)
        {
            ScriptState<object> next = null;

            if (state is ScriptState<object> previous)
            {
                try
                {
                    next = previous.ContinueWithAsync(code).Result;
                }
                catch (CompilationErrorException ex)
                {
                    var log = _serviceProvider.GetService<ILog>();
                    log?.LogException(ex);
                    log?.LogError($"{Environment.NewLine}{ex.Diagnostics}");
                }
                return next;
            }

            var options = ScriptOptions.Default
                .WithImports("System");

            var globals = new EditorScript
            {
                Editor = _serviceProvider.GetService<IProjectEditor>()
            };

            try
            {
                next = CSharpScript.RunAsync(code, options, globals).Result;
            }
            catch (CompilationErrorException ex)
            {
                var log = _serviceProvider.GetService<ILog>();
                log?.LogException(ex);
                log?.LogError($"{Environment.NewLine}{ex.Diagnostics}");
            }

            return next;
        }
    }
}
#endif
