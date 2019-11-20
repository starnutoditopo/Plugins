using Plugins.Extensibility;
using Plugins.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plugins
{
    internal class Startup
    {
        public Startup(
            IConfiguration configuration
            )
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(
            IServiceCollection services
            )
        {
            services.AddSingleton(this.Configuration);

            PluginHelper.RegisterPlugins(services);
        }        

        public async Task Run(
            ServiceProvider serviceProvider,
            string[] args
            )
        {
            var runners = serviceProvider.GetServices<IRunner>();
            foreach(var runner in runners)
            {
                await runner.Run(serviceProvider, args);
                return;
            }
            throw new InvalidOperationException("No service found");

        }
    }
}
