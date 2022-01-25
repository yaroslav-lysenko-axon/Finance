
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Test1
{
    public sealed class Program
    {
        private static void Main(string[] args)
        {
            var webHost = BuildWebHost(args);

            webHost.Run();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder.ClearProviders();
                })
                .UseStartup<Startup>()
                .SuppressStatusMessages(true)
                .Build();
    }
}
