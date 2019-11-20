using System.Threading.Tasks;

namespace Plugins.Sample.Extensibility
{
    public interface ISampleService
    {
        Task Print(string[] args);
    }
}
