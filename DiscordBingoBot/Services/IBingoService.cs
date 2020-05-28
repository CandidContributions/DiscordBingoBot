using DiscordBingoBot.Core;

namespace DiscordBingoBot.Services
{
    public interface IBingoService
    {
        Outcome<string> Start();
    }
}