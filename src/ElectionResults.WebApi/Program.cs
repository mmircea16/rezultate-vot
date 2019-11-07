using System;
using Amazon.Extensions.NETCore.Setup;
using ElectionResults.Core.Storage;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElectionResults.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.json");
                    builder.AddSystemsManager($"/{Consts.PARAMETER_STORE_NAME}");
                })
                .UseStartup<Startup>();
    }
}
