using System.Collections.Generic;

namespace BingoCore.Models.BingoConfiguration
{
    public class BingoConfiguration
    {
        public List<string> Words { get; set; }
        public List<KeyedPhrases> KeyedPhrases { get; set; }
        public AutoRoundSettings AutoRoundSettings { get; set; }
    }

}
