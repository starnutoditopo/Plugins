using Plugins.Sample.Extensibility;
using Plugins.Extensibility.Attributes;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Plugins
{
    [Export(typeof(ISampleService), ExportMode.Singleton)]
    public class SampleService : ISampleService
    {
        public async Task Print(string[] args)
        {
            foreach(string s in args)
            {
                Debug.WriteLine(s);
            }
        }
    }
}
