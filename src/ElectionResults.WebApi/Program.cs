using System;
using System.Net.Http;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using ElectionResults.Core.Storage;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ElectionResults.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    Console.WriteLine($"Env: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
                    var env = hostingContext.HostingEnvironment;
                    builder.AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true,
                            reloadOnChange: true);
                    if (env.EnvironmentName == "Development")
                    {
                        builder.AddSystemsManager($"/{Consts.ParameterStoreName}-dev", new AWSOptions
                        {
                            DefaultClientConfig =
                                    {
                                        ServiceURL = Consts.SSMServiceUrl,
                                        UseHttp = true
                                    },
                            Credentials = new BasicAWSCredentials("abc", "def")
                        }, TimeSpan.FromSeconds(30));
                    }
                    else
                    {
                        builder.AddSystemsManager($"/{Consts.ParameterStoreName}", new AWSOptions(),
                            TimeSpan.FromSeconds(30));
                    }
                }).UseStartup<Startup>();
        }
    }
}
