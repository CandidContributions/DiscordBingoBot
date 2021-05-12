using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingoCore.Constants;
using BingoCore.Core;
using BingoCore.Extensions;
using BingoCore.Factories;
using BingoCore.Models;
using BingoCore.Models.BingoConfiguration;
using BingoCore.Outcomes;
using BingoCore.WinConditions;

namespace BingoCore.Services
{
    public class BingoGame
    {
        private readonly ICsvReader _csvReader;
        private readonly ILogger _logger;
        private readonly IConfigurationService _configurationService;

        public BingoGame(ICsvReader csvReader, ILogger logger, IConfigurationService configurationService)
        {
            _csvReader = csvReader;
            _logger = logger;
            _configurationService = configurationService;
        }

        private List<Player> _players;
        private List<string> _roundItems;
        private List<IWinCondition> _winConditions = new List<IWinCondition> { new OneRowWinCondition(), new FullCardWinCondition() };
        private BingoConfiguration _configuration;
        private int _winners = 0;
        public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

        public bool Verbose { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsRoundActive { get; private set; }

        public async Task<Outcome<string>> Start()
        {
            if (IsActive)
            {
                return Outcome<string>.Fail(await _configurationService.GetPhrase(PhraseKeys.Start.AlreadyActive).ConfigureAwait(false));
            }

            await GameReset().ConfigureAwait(false);

            return Outcome<string>.Success();
        }

        public async Task<Outcome<string>> Register(string mention, string nickName)
        {
            if (mention.Trim().Length < 1)
            {
                return Outcome<string>.Fail(await _configurationService.GetPhrase(PhraseKeys.Register.InvalidName).ConfigureAwait(false));
            }
            if (IsActive == false)
            {
                return Outcome<string>.Fail(await _configurationService.GetPhrase(PhraseKeys.Register.NoActiveGame).ConfigureAwait(false));
            }
            if (IsRoundActive)
            {
                return Outcome<string>.Fail(await _configurationService.GetPhrase(PhraseKeys.Register.ActiveRound).ConfigureAwait(false));
            }
            if (_players.Any(p => p.Name == mention))
            {
                return Outcome<string>.Fail((await _configurationService.GetPhrase(PhraseKeys.Register.AlreadyRegistered).ConfigureAwait(false)).Replace("{0}",mention));
            }

            _players.Add(new Player(mention,nickName));
            return Outcome<string>.Success();
        }

        public async Task<Outcome<string>> DeRegister(string name)
        {
            if (name.Trim().Length < 1)
            {
                return Outcome<string>.Fail(await _configurationService.GetPhrase(PhraseKeys.DeRegister.InvalidName).ConfigureAwait(false));
            }

            var player = _players.FirstOrDefault(p => p.Name == name);
            if (player == null)
            {
                return Outcome<string>.Fail((await _configurationService.GetPhrase(PhraseKeys.DeRegister.NotRegistered).ConfigureAwait(false)).Replace("{0}", name));
            }

            _players.Remove(player);
            return Outcome<string>.Success();
        }

        public async Task<Outcome<StartRoundOutcome>> StartRound(bool verbose)
        {
            Verbose = verbose;
            if (IsActive == false)
            {
                return Outcome<StartRoundOutcome>.Fail(new StartRoundOutcome{Error = await _configurationService.GetPhrase(PhraseKeys.StartRound.NoActiveGame).ConfigureAwait(false)});
            }
            if (IsRoundActive)
            {
                return Outcome<StartRoundOutcome>.Fail(new StartRoundOutcome { Error = await _configurationService.GetPhrase(PhraseKeys.StartRound.ActiveRound).ConfigureAwait(false) });
            }

            if (_players.Count < 1)
            {
                return Outcome<StartRoundOutcome>.Fail(new StartRoundOutcome { Error = await _configurationService.GetPhrase(PhraseKeys.StartRound.NotEnoughPlayers).ConfigureAwait(false) });
            }

            RoundReset();
            IsRoundActive = true;
            return Outcome<StartRoundOutcome>.Success(new StartRoundOutcome{FirstWinCondition = _winConditions.First().Description,NumberOfWinConditions = _winConditions.Count});
        }

        public async Task<Outcome<string>> NextItem()
        {
            if (IsActive == false)
            {
                return Outcome<string>.Fail(await _configurationService.GetPhrase(PhraseKeys.Next.NoActiveGame).ConfigureAwait(false));
            }
            if (IsRoundActive == false)
            {
                return Outcome<string>.Fail(await _configurationService.GetPhrase(PhraseKeys.Next.NoActiveRound).ConfigureAwait(false));
            }

            var item = _roundItems.FirstOrDefault();
            if (item == null)
            {
                return Outcome<string>.Fail(await _configurationService.GetPhrase(PhraseKeys.Next.NoRemainingItems).ConfigureAwait(false));
            }
            _roundItems.RemoveAt(0);
            MarkItemOnPlayerCards(item);
            return Outcome<string>.Success(item);
        }

        public async Task<Outcome<CheckBingoOutcome>> CheckBingo(string playerName)
        {
            var player = _players.FirstOrDefault(p => p.Name == playerName);
            if (player == null)
            {
                return Outcome<CheckBingoOutcome>.Fail(new CheckBingoOutcome { Error = await _configurationService.GetPhrase(PhraseKeys.Bingo.NotRegistered).ConfigureAwait(false) });
            }

            var winningOutcome = Outcome<CheckBingoOutcome>.Success(new CheckBingoOutcome());
            if (_winConditions[_winners].ConditionMet(GetPlayerGrid(playerName)))
            {
                _winners++;
                if (_winners >= _winConditions.Count)
                {
                    IsRoundActive = false;
                    winningOutcome.Info.RoundHasEnded = true;
                }
                else
                {
                    winningOutcome.Info.NextWinCondition = _winConditions[_winners].Description;
                }
                return winningOutcome;
            }
            return Outcome<CheckBingoOutcome>.Fail(new CheckBingoOutcome { Error = await _configurationService.GetPhrase(PhraseKeys.Bingo.Mistake).ConfigureAwait(false) });
        }

        public async Task<Outcome<string>> Stop()
        {
            if (IsActive == false)
            {
                return Outcome<string>.Fail(await _configurationService.GetPhrase(PhraseKeys.Stop.NoActive).ConfigureAwait(false));
            }

            IsActive = false;
            IsRoundActive = false;
            return Outcome<string>.Success();
        }

        public Grid GetPlayerGrid(string playerName)
        {
            return _players.First(p => p.Name == playerName).Grid;
        }

        public async Task LoadConfiguration(bool force = false)
        {
            if (force || _configuration == null)
            {
                _configuration = await _configurationService.GetConfiguration(force).ConfigureAwait(false);
            }
        }

        private async Task GameReset()
        {
            await LoadConfiguration().ConfigureAwait(false);
            IsActive = true;
            IsRoundActive = false;
            _players = new List<Player>();
        }

        private void RoundReset()
        {
            AssingGridsToPlayers();

            // randomize the pool
            _roundItems = new List<string>(_configuration.Words);
            var roundGuid = Guid.NewGuid();
            var random = RandomFactory.FromGuid(roundGuid);
            _roundItems.Shuffle(random);
            _winners = 0;
        }

        private void AssingGridsToPlayers()
        {
            foreach (var player in _players)
            {
                player.Grid = new Grid(Guid.NewGuid());
                player.Grid.Populate(_configuration.Words);
            }
        }

        private void MarkItemOnPlayerCards(string item)
        {
            foreach (var player in _players)
            {
                player.Grid.Mark(item);
            }
        }
    }
}
