using System.Collections.Generic;
using System.Linq;

namespace Woolies.Api
{
    public static class CollectionHelper
    {
        public static bool IsUnique(this IEnumerable<string> values)
        {
            return values.Count() ==
                   values.Distinct().Count();
        }
    }
}
