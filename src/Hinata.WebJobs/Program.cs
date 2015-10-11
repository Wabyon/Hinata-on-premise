using Topshelf;

namespace Hinata
{
    class Program
    {
        static void Main()
        {
            var host = HostFactory.New(config =>
            {
                config.Service<WebJob>(s =>
                {
                    s.ConstructUsing(name => new WebJob());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                config.RunAsLocalSystem();
                config.SetDescription("Hinata Web Jobs");
                config.SetDisplayName("HinataWebJobs");
                config.SetServiceName("HinataWebJobs");
            });

            host.Run();
        }
    }
}
