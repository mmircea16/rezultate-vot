using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Repositories;
using ElectionResults.Core.Services;
using ElectionResults.Core.Services.BlobContainer;
using ElectionResults.Core.Services.CsvDownload;
using ElectionResults.Core.Services.CsvProcessing;
using ElectionResults.Core.Storage;
using ElectionResults.WebApi.Scheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ElectionResults.WebApi
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppConfig>(options => Configuration.GetSection("settings").Bind(options));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<IResultsRepository, ResultsRepository>();
            services.AddTransient<IResultsAggregator, ResultsAggregator>();
            services.AddTransient<ICsvDownloaderJob, CsvDownloaderJob>();
            services.AddTransient<IBucketUploader, BucketUploader>();
            services.AddTransient<IElectionConfigurationSource, ElectionConfigurationSource>();
            services.AddTransient<IFileProcessor, FileProcessor>();
            services.AddTransient<IStatisticsAggregator, StatisticsAggregator>();
            services.AddTransient<IBucketRepository, BucketRepository>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IVoterTurnoutAggregator, VoterTurnoutAggregator>();

            if (_environment.IsDevelopment())
            {
                services.AddSingleton<IAmazonDynamoDB>(sp =>
                {
                    var clientConfig = new AmazonDynamoDBConfig { ServiceURL = Consts.DynamoDbServiceUrl, UseHttp = true };
                    return new AmazonDynamoDBClient(clientConfig);
                });
            }
            else
            {
                services.AddAWSService<IAmazonDynamoDB>();
            }

            if (_environment.IsDevelopment())
            {
                var amazonS3 = new AmazonS3Client(new AmazonS3Config
                {
                    ServiceURL = Consts.S3ServiceUrl,
                    ForcePathStyle = true,
                    UseHttp = true
                });

                services.AddTransient(typeof(IAmazonS3), provider => amazonS3);
            }
            else
            {
                services.AddAWSService<IAmazonS3>(new AWSOptions
                {
                    Profile = "default",
                    Region = RegionEndpoint.EUCentral1
                });
                services.AddDefaultAWSOptions(new AWSOptions
                {
                    Region = RegionEndpoint.EUCentral1
                });
            }
            services.AddLazyCache();
            services.AddSingleton<IHostedService, ScheduleTask>();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            services.AddLogging(builder =>
            {
                var config = Configuration.GetSection("AWS.Logging");
                builder
                    .AddConfiguration(config)
                    .AddConsole()
                    .AddDebug()
                    .AddAWSProvider(Configuration.GetAWSLoggingConfigSection().Config);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("origins",
                    builder =>
                    {
                        builder.WithOrigins("*");
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseHsts();
            Log.SetLogger(loggerFactory.CreateLogger<Startup>());

            app.UseCors("origins");
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(exceptionHandlerPathFeature.Error));

                });
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
