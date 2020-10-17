using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DiscordBingoBot.Constants;
using DiscordBingoBot.Models;
using DiscordBingoBot.Models.BingoConfiguration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DiscordBingoBot.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _config;
        private BingoConfiguration _configuration;
        private readonly Random _random = new Random();

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

        public async Task<BingoConfiguration> GetConfiguration(bool refresh = false)
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

        public async Task<string> GetPhrase(string key)
        {
            if (_configuration == null)
            {
                await GetConfiguration().ConfigureAwait(false);
            }

            //todo need to implement boosting 
            var phraseList = _configuration?.KeyedPhrases.FirstOrDefault(p => p.Key == key) ??
                             FallBackPhrases.FirstOrDefault(p => p.Key == key);
            if (phraseList == null)
            {
                return "Phrase not found = " + key;
            }

            return phraseList.Phrases[_random.Next(0, phraseList.Phrases.Count)].Text;
        }

        private static readonly List<KeyedPhrases> FallBackPhrases = new List<KeyedPhrases>
        {
            new KeyedPhrases(PhraseKeys.Start.AlreadyActive,new List<Phrase>{new Phrase("There is currently a game active", 0)}),

            new KeyedPhrases(PhraseKeys.Register.InvalidName,new List<Phrase>{new Phrase("Invalid name", 0)}),
            new KeyedPhrases(PhraseKeys.Register.NoActiveGame,new List<Phrase>{new Phrase("No active game found", 0)}),
            new KeyedPhrases(PhraseKeys.Register.ActiveRound,new List<Phrase>{new Phrase("There is an active round, wait for the round to end", 0)}),
            new KeyedPhrases(PhraseKeys.Register.AlreadyRegistered,new List<Phrase>{new Phrase("{0} is already registered", 0)}),

            new KeyedPhrases(PhraseKeys.DeRegister.InvalidName,new List<Phrase>{new Phrase("Invalid name", 0)}),
            new KeyedPhrases(PhraseKeys.DeRegister.NotRegistered,new List<Phrase>{new Phrase("{0} is not registered", 0)}),

            new KeyedPhrases(PhraseKeys.StartRound.NoActiveGame,new List<Phrase>{new Phrase("No active game found", 0)}),
            new KeyedPhrases(PhraseKeys.StartRound.ActiveRound,new List<Phrase>{new Phrase("There is already a round active", 0)}),
            new KeyedPhrases(PhraseKeys.StartRound.NotEnoughPlayers,new List<Phrase>{new Phrase("Need at least 2 players to play", 0)}),

            new KeyedPhrases(PhraseKeys.Next.NoActiveGame,new List<Phrase>{new Phrase("No active game found", 0)}),
            new KeyedPhrases(PhraseKeys.Next.NoActiveRound,new List<Phrase>{new Phrase("No active game found", 0)}),
            new KeyedPhrases(PhraseKeys.Next.NoRemainingItems,new List<Phrase>{new Phrase("No items remaining, everyone must have fallen asleep", 0)}),

            new KeyedPhrases(PhraseKeys.Bingo.NotRegistered,new List<Phrase>{new Phrase("You are not registered", 0)}),
            new KeyedPhrases(PhraseKeys.Bingo.Mistake,new List<Phrase>{new Phrase("We think you might have made a mistake there", 0)}),

            new KeyedPhrases(PhraseKeys.Stop.NoActive,new List<Phrase>{new Phrase("No active game found", 0)}),
        };
    }
}
