using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingoCore.Services;

namespace DiscordBingoBot.Services
{
    public class BingoService : IBingoService
    {
        private readonly ICsvReader _csvReader;
        private readonly ILogger _logger;
        private readonly IConfigurationService _configurationService;
        private readonly Dictionary<string, BingoGame> _bingoGames = new Dictionary<string, BingoGame>();

        public BingoService(ICsvReader csvReader, ILogger logger, IConfigurationService configurationService)
        {
            _csvReader = csvReader;
            _logger = logger;
            _configurationService = configurationService;
        }

        public BingoGame GetGame(string identifier)
        {
            if (_bingoGames.ContainsKey(identifier) == false)
            {
                _bingoGames.Add(identifier, new BingoGame(_csvReader, _logger, _configurationService));
            }
            return _bingoGames[identifier];
        }

        public void RemoveGame(string identifier)
        {
            if (_bingoGames.ContainsKey(identifier) == false)
            {
                return;
            }

            _bingoGames.Remove(identifier);
        }
    }
}
