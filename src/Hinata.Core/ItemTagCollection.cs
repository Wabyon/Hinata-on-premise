using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hinata
{
    public class ItemTagCollection : Collection<ItemTag>
    {
        internal void AddRange(IEnumerable<ItemTag> tags)
        {
            foreach (var tag in tags)
            {
                Add(tag);
            }
        }
    }
}
