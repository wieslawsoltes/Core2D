using System;
using System.Threading.Tasks;
using Core2D;
using Core2D.Editor;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Core2D.ScriptRunner.Roslyn
{
    /// <summary>
    /// Roslyn C# script runner.
    /// </summary>
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
        public async Task<object> Execute(string code, object state)
        {
            try
            {
                if (state is ScriptState<object> previousState)
                {
                    return await previousState.ContinueWithAsync(code);
                }
                var options = ScriptOptions.Default.WithImports("System");
                var editor = _serviceProvider.GetService<IProjectEditor>();
                return await CSharpScript.RunAsync(code, options, editor);
            }
            catch (CompilationErrorException ex)
            {
                var log = _serviceProvider.GetService<ILog>();
                log?.LogException(ex);
                log?.LogError($"{Environment.NewLine}{ex.Diagnostics}");
            }
            return null;
        }
    }
}
