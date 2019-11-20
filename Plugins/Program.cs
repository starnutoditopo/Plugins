using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Plugins
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            Startup startup = new Startup(configuration);

            IServiceCollection serviceCollection = new ServiceCollection();

            startup.ConfigureServices(serviceCollection);

            using var serviceProvider = serviceCollection
                .BuildServiceProvider();

            try
            {
                await startup.Run(serviceProvider, args);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
