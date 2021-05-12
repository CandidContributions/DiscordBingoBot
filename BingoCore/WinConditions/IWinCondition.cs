using BingoCore.Models;

namespace BingoCore.WinConditions
{
    public interface IWinCondition
    {
        string Description { get; }
        bool ConditionMet(Grid grid);
    }
}