using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordBingoBot.Core;
using DiscordBingoBot.Extenstions;
using DiscordBingoBot.Models;

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

        public Outcome<string> StartRound()
        {
            if (_isActive == false)
            {
                return Outcome<string>.Fail("No active game found");
            }
            if (_roundActive)
            {
                return Outcome<string>.Fail("There is already a round active");
            }

            if (_players.Count < 1)
            {
                return Outcome<string>.Fail("Need at least 2 players to play");
            }

            RoundReset();
            _roundActive = true;
            return Outcome<string>.Success();
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

        public Outcome<string> CheckBingo(string playerName)
        {
            var player = _players.FirstOrDefault(p => p.Name == playerName);
            if (player == null)
            {
                return Outcome<string>.Fail("You are not registered");
            }

            if (GetPlayerGrid(playerName).IsFullyMarked())
            {
                return Outcome<string>.Success();
            }
            return Outcome<string>.Fail("Liar liar, pants on fire!");
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
