using System;

namespace Hinata
{
    public class Tag
    {
        public string Name { get; internal set; }

        public int AllItemCount { get; internal set; }

        public int PublicItemCount { get; internal set; }

        public int PrivateItemCount { get; internal set; }

        internal Tag()
        {
        }

        public Tag(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("name is null or whitespace");
            Name = name;
        }
    }
}
