using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Plugins.Extensibility
{
    public interface IRunner
    {
        Task Run(
            ServiceProvider serviceProvider,
            string[] args
            );
    }
}
