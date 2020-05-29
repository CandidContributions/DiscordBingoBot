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
        Outcome<StartRoundOutcome> StartRound(bool verbose);
        Outcome<string> NextItem();
        Outcome<CheckBingoOutcome> CheckBingo(string playerName);
        Outcome<string> Stop();
        IReadOnlyCollection<Player> Players { get; }
        bool Verbose { get; }
        Outcome<string> DeRegister(string name);
    }
}