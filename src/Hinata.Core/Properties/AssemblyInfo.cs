using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Hinata.Core")]
[assembly: AssemblyVersion("0.1.0.0")]

#if DEBUG
[assembly: InternalsVisibleTo("Hinata.Core.Test")]
[assembly: InternalsVisibleTo("Hinata.Data.Test")]
[assembly: InternalsVisibleTo("Hinata.Markdown.Test")]
#endif