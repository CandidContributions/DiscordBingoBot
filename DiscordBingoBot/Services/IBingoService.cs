using System.Collections.Generic;
using DiscordBingoBot.Core;
using DiscordBingoBot.Models;
using DiscordBingoBot.Outcomes;

namespace DiscordBingoBot.Services
{
    public interface IBingoService
    {
        Outcome<string> Start();
        Outcome<string> Register(string playerName);
        Outcome<string> StartRound();
        Outcome<string> NextItem();
        Outcome<CheckBingoOutcome> CheckBingo(string playerName);
        Outcome<string> Stop();
        IReadOnlyCollection<Player> Players { get; }
    }
}