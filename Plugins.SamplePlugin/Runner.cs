using Plugins.Extensibility;
using Plugins.Sample.Extensibility;
using Plugins.Extensibility.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Plugins.Sample
{
    [Export(typeof(IRunner), ExportMode.Singleton)]
    public class Runner: IRunner
    {
        public async Task Run(
            ServiceProvider serviceProvider,
            string[] args
            )
        {
            var sampleServices = serviceProvider.GetServices<ISampleService>();
            foreach (var sampleService in sampleServices)
            {
                await sampleService.Print(args);
                return;
            }
            throw new InvalidOperationException("No service found");
        }
    }
}
