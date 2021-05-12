using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBingoBot.Models.BingoConfiguration
{
    public class BingoConfiguration
    {
        public List<string> Words { get; set; }
        public List<KeyedPhrases> KeyedPhrases { get; set; }
        public AutoRoundSettings AutoRoundSettings { get; set; }
    }

}
