using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordBingoBot.Core;
using DiscordBingoBot.Models;

namespace DiscordBingoBot.Services
{
    public class BingoService : IBingoService
    {
        private readonly ICsvReader _csvReader;

        public BingoService(ICsvReader csvReader)
        {
            _csvReader = csvReader;
        }

        private bool _isActive;
        private bool _roundActive;
        private List<string> _items;
        private List<Player> _players;

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

            _roundActive = true;
            // todo give every player a new card


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
            // todo mark all matching items on player cards
            return Outcome<string>.Success("Todo: pass the item");
        }

        public Outcome<string> CheckBingo()
        {
            return Outcome<string>.Fail("Not implemented");
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

        private void GameReset()
        {
            _isActive = true;
            _roundActive = false;
            _items = _csvReader.Read("wordList.csv");
            _players = new List<Player>();
        }

        private void RoundReset()
        {
            // todo randomize the item pool
            // todo reset the item pool tracker
        }
    }
}
