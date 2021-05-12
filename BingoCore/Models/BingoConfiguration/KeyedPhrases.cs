using System.Collections.Generic;

namespace BingoCore.Models.BingoConfiguration
{
    public class KeyedPhrases
    {
        public string Key { get; set; }
        public List<Phrase> Phrases { get; set; }

        public KeyedPhrases()
        {
            
        }

        public KeyedPhrases(string key, List<Phrase> phrases)
        {
            Key = key;
            Phrases = phrases;
        }
    }
}