using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBingoBot.Outcomes
{
    public class CheckBingoOutcome
    {
        public bool RoundHasEnded { get; set; }
        public string NextWinCondition { get; set; }
        public string Error { get; set; }
    }
}
