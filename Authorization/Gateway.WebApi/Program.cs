using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.WebApi
{
    public static class Program
    {
        private static void Main()
        {
            IWebHostBuilder builder = new WebHostBuilder();
            builder.ConfigureServices(s =>
            {
                s.AddSingleton(builder);
            });
            builder.UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .ConfigureAppConfiguration(config =>
                    config.AddJsonFile($"authConfiguration.json")
                        .AddJsonFile($"fileConfiguration.json"));
            var host = builder.Build();
            host.Run();
        }
    }
}
