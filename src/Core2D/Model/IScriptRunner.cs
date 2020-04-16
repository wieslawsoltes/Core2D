using System.Threading.Tasks;

namespace Core2D
{
    /// <summary>
    /// Executes code scripts.
    /// </summary>
    public interface IScriptRunner
    {
        /// <summary>
        /// Executes code script and return current script state.
        /// </summary>
        /// <param name="code">The script code.</param>
        /// <param name="state">The script state to continue execution from a previous state.</param>
        /// <returns>The next script state.</returns>
        Task<object> Execute(string code, object state);
    }
}
