using System.Collections.Generic;

namespace Plugins.Helpers
{
    public static class CollectionHelper
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach(T item in items)
            {
                collection.Add(item);
            }
        }
    }
}
