using DiscordBingoBot.Models;

namespace DiscordBingoBot.WinConditions
{
    public interface IWinCondition
    {
        string Description { get; }
        bool ConditionMet(Grid grid);
    }
}