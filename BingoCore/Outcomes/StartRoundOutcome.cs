using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBingoBot.Outcomes
{
    public class StartRoundOutcome
    {
        public int NumberOfWinConditions{ get; set; }
        public string FirstWinCondition { get; set; }
        public string Error { get; set; }
    }
}
