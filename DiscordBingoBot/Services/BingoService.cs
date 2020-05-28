using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordBingoBot.Core;
using DiscordBingoBot.Extenstions;
using DiscordBingoBot.Models;
using DiscordBingoBot.Outcomes;
using DiscordBingoBot.WinConditions;

namespace DiscordBingoBot.Services
{
    public class BingoService : IBingoService
    {
        private readonly ICsvReader _csvReader;
        private readonly ILogger _logger;

        public BingoService(ICsvReader csvReader, ILogger logger)
        {
            _csvReader = csvReader;
            _logger = logger;
        }

        private bool _isActive;
        private bool _roundActive;
        private List<string> _items;
        private List<Player> _players;
        private List<string> _roundItems;
        private List<IWinCondition> _winConditions = new List<IWinCondition> { new OneRowWinCondition(), new FullCardWinCondition() };
        private int _winners = 0;
        public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

        public Outcome<string> Start()
        {
            if (_isActive)
            {
                return Outcome<string>.Fail("There is currently a game active");
            }

            GameReset();

            return Outcome<string>.Success();
        }

        public Outcome<string> Register(string name)
        {
            if (name.Trim().Length < 1)
            {
                return Outcome<string>.Fail("Invalid name");
            }
            if (_isActive == false)
            {
                return Outcome<string>.Fail("No active game found");
            }
            if (_roundActive)
            {
                return Outcome<string>.Fail("There is an active round, wait for the round to end");
            }
            if (_players.Any(p => p.Name == name))
            {
                return Outcome<string>.Fail(name + " is already registered");
            }

            _players.Add(new Player(name));
            return Outcome<string>.Success();
        }

        public Outcome<StartRoundOutcome> StartRound()
        {
            if (_isActive == false)
            {
                return Outcome<StartRoundOutcome>.Fail(new StartRoundOutcome{Error = "No active game found"});
            }
            if (_roundActive)
            {
                return Outcome<StartRoundOutcome>.Fail(new StartRoundOutcome { Error = "There is already a round active"});
            }

            if (_players.Count < 1)
            {
                return Outcome<StartRoundOutcome>.Fail(new StartRoundOutcome { Error = "Need at least 2 players to play"});
            }

            RoundReset();
            _roundActive = true;
            return Outcome<StartRoundOutcome>.Success(new StartRoundOutcome{FirstWinCondition = _winConditions.First().Description,NumberOfWinConditions = _winConditions.Count});
        }

        public Outcome<string> NextItem()
        {
            if (_isActive == false)
            {
                return Outcome<string>.Fail("No active game found");
            }
            if (_roundActive == false)
            {
                return Outcome<string>.Fail("No active round");
            }
            // todo pick and item and move the tracker
            var item = _roundItems.FirstOrDefault();
            if (item == null)
            {
                return Outcome<string>.Fail("No items remaining, everyone must be asleep");
            }
            _roundItems.RemoveAt(0);
            MarkItemOnPlayerCards(item);
            return Outcome<string>.Success(item);
        }

        public Outcome<CheckBingoOutcome> CheckBingo(string playerName)
        {
            var player = _players.FirstOrDefault(p => p.Name == playerName);
            if (player == null)
            {
                return Outcome<CheckBingoOutcome>.Fail(new CheckBingoOutcome { Error = "You are not registered" });
            }

            var winningOutcome = Outcome<CheckBingoOutcome>.Success(new CheckBingoOutcome());
            if (_winConditions[_winners].ConditionMet(GetPlayerGrid(playerName)))
            {
                _winners++;
                if (_winners >= _winConditions.Count)
                {
                    _roundActive = false;
                    winningOutcome.Info.RoundHasEnded = true;
                }
                else
                {
                    winningOutcome.Info.NextWinCondition = _winConditions[_winners].Description;
                }
                return winningOutcome;
            }
            return Outcome<CheckBingoOutcome>.Fail(new CheckBingoOutcome { Error = "We think you might have made a mistake there." });
        }

        public Outcome<string> Stop()
        {
            if (_isActive == false)
            {
                return Outcome<string>.Fail("No active game found");
            }

            _isActive = false;
            _roundActive = false;
            return Outcome<string>.Success();
        }

        public Grid GetPlayerGrid(string playerName)
        {
            return _players.First(p => p.Name == playerName).Grid;
        }

        private void GameReset()
        {
            _isActive = true;
            _roundActive = false;
            _items = _csvReader.Read("wordList.csv");
            _players = new List<Player>();
        }

        private void RoundReset()
        {
            AssingGridsToPlayers();

            // randomize the pool
            _roundItems = new List<string>(_items);
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
                player.Grid.Populate(_items);
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
