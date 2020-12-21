#nullable disable
using System.Threading.Tasks;

namespace Core2D.Model
{
    public interface IScriptRunner
    {
        Task<object> Execute(string code, object state);
    }
}
