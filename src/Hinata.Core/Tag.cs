namespace Hinata
{
    public sealed class Tag
    {
        public string Name { get; private set; }

        public string Version { get; private set; }

        public Tag(string name, string version)
        {
            Name = name;
            Version = version;
        }
    }
}
