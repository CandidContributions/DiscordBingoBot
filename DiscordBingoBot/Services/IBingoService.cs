using System.Collections.Generic;
using DiscordBingoBot.Core;
using DiscordBingoBot.Models;

namespace DiscordBingoBot.Services
{
    public interface IBingoService
    {
        Outcome<string> Start();
        Outcome<string> Register(string playerName);
        Outcome<string> StartRound();
        Outcome<string> NextItem();
        Outcome<string> CheckBingo(string playerName);
        Outcome<string> Stop();
        IReadOnlyCollection<Player> Players { get; }
    }
}