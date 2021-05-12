using BingoCore.Services;

namespace DiscordBingoBot.Services
{
    public interface IBingoService
    {
        BingoGame GetGame(string identifier);
        void RemoveGame(string identifier);
    }
}