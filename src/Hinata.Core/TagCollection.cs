using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hinata
{
    public class TagCollection : Collection<Tag>
    {
        internal void AddRange(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                Add(tag);
            }
        }
    }
}
