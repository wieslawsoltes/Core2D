using System.Threading.Tasks;

namespace Core2D
{
    public interface IScriptRunner
    {
        Task<object> Execute(string code, object state);
    }
}
