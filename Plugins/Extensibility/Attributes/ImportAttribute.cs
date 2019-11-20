using System;

namespace Plugins.Extensibility.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ImportAttribute : Attribute
    {
        public ImportAttribute()
        {
        }
    }
}
