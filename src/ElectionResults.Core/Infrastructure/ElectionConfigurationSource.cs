using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;
using ElectionResults.Core.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ElectionResults.Core.Infrastructure
{
    public class ElectionConfigurationSource : IElectionConfigurationSource
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppConfig _config;
        private readonly AmazonSimpleSystemsManagementClient _amazonSettingsClient;
        private string _parameterStoreName;

        public ElectionConfigurationSource(IOptions<AppConfig> config, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = config.Value;
            _amazonSettingsClient = new AmazonSimpleSystemsManagementClient();
            _parameterStoreName = Consts.PARAMETER_STORE_NAME;
            if (hostingEnvironment.IsDevelopment())
                _parameterStoreName += "-dev";
        }

        public async Task<Result> UpdateInterval(int seconds)
        {
            var putParameterRequest = new PutParameterRequest
            {
                Name = $"/{_parameterStoreName}/settings/intervalInSeconds",
                Value = seconds.ToString(),
                Type = ParameterType.String,
                Overwrite = true
            };
            var response = await _amazonSettingsClient.PutParameterAsync(putParameterRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
                return Result.Ok();
            return Result.Failure("Couldn't update the job timer");
        }

        public async Task<Result<int>> GetInterval()
        {
            var getParameterRequest = new GetParameterRequest
            {
                Name = $"/{_parameterStoreName}/settings/intervalInSeconds",
            };
            var response = await _amazonSettingsClient.GetParameterAsync(getParameterRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
                return Result.Ok(int.Parse(response.Parameter.Value));
            return Result.Failure<int>("Couldn't retrieve the job timer");
        }

        public async Task<Result> UpdateElectionConfig(ElectionsConfig config)
        {
            var putParameterRequest = new PutParameterRequest
            {
                Name = $"/{_parameterStoreName}/settings/electionsConfig",
                Value = JsonConvert.SerializeObject(config),
                Type = ParameterType.String,
                Overwrite = true
            };
            var response = await _amazonSettingsClient.PutParameterAsync(putParameterRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
                return Result.Ok();
            return Result.Failure("Couldn't update the job timer");
        }

        public async Task<Result<string>> GetConfigAsync()
        {
            var getParameterRequest = new GetParameterRequest
            {
                Name = $"/{_parameterStoreName}/settings/electionsConfig",
            };
            var response = await _amazonSettingsClient.GetParameterAsync(getParameterRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
                return Result.Ok(response.Parameter.Value);
            return Result.Failure<string>("Couldn't update the job timer");
        }

        public List<ElectionResultsFile> GetListOfFilesWithElectionResults()
        {
            var electionsConfig = JsonConvert.DeserializeObject<ElectionsConfig>(_config.ElectionsConfig);
            var files = electionsConfig.Files;
            /*files[0].Name = "national";
            files[0].FileType = FileType.Results;
            files[0].ResultsSource = "national";
            files[1].Name = "diaspora";
            files[1].FileType = FileType.Results;
            files[1].ResultsSource = "diaspora";
            files[2].Name = "mail";
            files[2].FileType = FileType.Results;
            files[2].ResultsSource = "mail";*/
            files[0].Active = true;
            files[1].Active = true;
            files[2].Active = true;
            return files;
        }
    }
}