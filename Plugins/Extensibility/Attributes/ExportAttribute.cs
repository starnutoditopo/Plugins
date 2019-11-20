using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins.Extensibility.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExportAttribute : Attribute
    {
        public ExportAttribute(Type contract)
            : this(contract, ExportMode.Transient)
        {
        }

        public ExportAttribute(Type contract, ExportMode exportMode)
            : this(new Type[] { contract }, exportMode)
        {
        }

        public ExportAttribute(IEnumerable<Type> contracts, ExportMode exportMode)
        {
            this.Contracts = contracts.ToArray();
            this.Mode = exportMode;
        }

        public ExportMode Mode
        {
            get;
        }

        public Type[] Contracts
        {
            get;
        }
    }
}
