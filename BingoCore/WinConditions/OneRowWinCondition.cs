using BingoCore.Models;

namespace BingoCore.WinConditions
{
    public class OneRowWinCondition : IWinCondition
    {
        public string Description { get; } = "A single row is necessary to win";

        public bool ConditionMet(Grid grid)
        {
            for (int i = 0; i < grid.Rows.Length; i++)
            {
                var count = 0;
                for (int j = 0; j < grid.Rows[i].Items.Length; j++)
                {
                    if (grid.IsMarked(i,j))
                    {
                        count++;
                    }
                }

                if (count == grid.Rows[i].Items.Length)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
