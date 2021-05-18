using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BingoCore.Constants;
using BingoCore.Models.BingoConfiguration;
using BingoCore.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DiscordBingoBot.Services
{
    public class ConfigurationService : ConfigurationServiceBase, IConfigurationService
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _config;

        public ConfigurationService(ILogger logger, IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;
        }

        private HttpClient _httpClient;
        private HttpClient HttpClient
        {
            get {
                if (_httpClient == null)
                {
                    _httpClient = CreateClient();
                }

                return _httpClient;
            }
        }

        public override async Task<BingoConfiguration> GetConfiguration(bool refresh = false)
        {
            if (refresh == false && _configuration != null)
            {
                return _configuration;
            }

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, _config["DiscordBotApiBaseUrl"] + "/Token")
            {
                Content = new StringContent(JsonConvert.SerializeObject(_config["DiscordBotApiSecret"]), Encoding.UTF8, "application/json")
            };

            // get a token
            var tokenResult = await HttpClient.SendAsync(tokenRequest);
            if (tokenResult.IsSuccessStatusCode == false)
            {
                await _logger.Warn("Failed to get a token for configuration api");
                return null;
            }

            var token = JsonConvert.DeserializeObject<string>(await tokenResult.Content.ReadAsStringAsync().ConfigureAwait(false));
            // if token is issued, get configuration
            var configRequest = new HttpRequestMessage(HttpMethod.Get,
                _config["DiscordBotApiBaseUrl"] + "/BingoConfiguration?Token=" + token + "&wordCount=0");
            var configResult = await HttpClient.SendAsync(configRequest);
            if (configResult.IsSuccessStatusCode == false)
            {
                await _logger.Warn("Failed to get a token for configuration api");
                return null;
            }

            var jsonString = await configResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            _configuration = JsonConvert.DeserializeObject<BingoConfiguration>(jsonString);
            return _configuration;
        }


        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }
    }
}
