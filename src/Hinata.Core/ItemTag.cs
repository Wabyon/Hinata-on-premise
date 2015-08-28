namespace Hinata
{
    public sealed class ItemTag
    {
        public string Name { get; private set; }

        public string Version { get; private set; }

        public ItemTag(string name, string version)
        {
            Name = name;
            Version = version;
        }
    }
}
