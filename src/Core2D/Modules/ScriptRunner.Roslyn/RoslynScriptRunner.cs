using System;
using System.Threading.Tasks;
using Core2D.Model;
using Core2D.ViewModels.Editor;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Core2D.ScriptRunner.Roslyn
{
    public class RoslynScriptRunner : IScriptRunner
    {
        private readonly IServiceProvider _serviceProvider;

        public RoslynScriptRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<object> Execute(string code, object state)
        {
            try
            {
                if (state is ScriptState<object> previousState)
                {
                    return await previousState.ContinueWithAsync(code);
                }
                var options = ScriptOptions.Default.WithImports("System");
                var editor = _serviceProvider.GetService<ProjectEditorViewModel>();
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
