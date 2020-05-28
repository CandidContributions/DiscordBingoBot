using DiscordBingoBot.Core;

namespace DiscordBingoBot.Services
{
    public interface IBingoService
    {
        Outcome<string> Start();
        Outcome<string> Register(string name);
        Outcome<string> StartRound();
        Outcome<string> NextItem();
        Outcome<string> CheckBingo();
        Outcome<string> Stop();
    }
}