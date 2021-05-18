using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingoCore.Constants;
using BingoCore.Models.BingoConfiguration;

namespace BingoCore.Services
{
    public class ConfigurationServiceBase
    {
        protected BingoConfiguration _configuration;
        protected readonly Random _random = new Random();


        public virtual async Task<BingoConfiguration> GetConfiguration(bool refresh = false)
        {
            return null;
        }

        public async Task<string> GetPhrase(string key)
        {
            if (_configuration == null)
            {
                await GetConfiguration().ConfigureAwait(false);
            }

            var phraseList = _configuration?.KeyedPhrases.FirstOrDefault(p => p.Key == key) ??
                             FallBackPhrases.FirstOrDefault(p => p.Key == key);
            if (phraseList == null)
            {
                return "Phrase not found = " + key;
            }

            var boostCount = phraseList.Phrases.Sum(p => p.Boost);
            if (boostCount == 0)
            {
                return phraseList.Phrases[_random.Next(0, phraseList.Phrases.Count)].Text;
            }

            var targetWeightIndex = _random.Next(0, phraseList.Phrases.Count + boostCount);

            var processedWeight = 0;
            for (var i = 0; i < phraseList.Phrases.Count; i++)
            {
                if (processedWeight + i + phraseList.Phrases[i].Boost >= targetWeightIndex)
                {
                    return phraseList.Phrases[i].Text;
                }

                processedWeight += i + phraseList.Phrases[i].Boost;
            }

            return phraseList.Phrases.Last().Text;
        }

        private static readonly List<KeyedPhrases> FallBackPhrases = new List<KeyedPhrases>
        {
            new KeyedPhrases(PhraseKeys.Start.AlreadyActive,new List<Phrase>{new Phrase("There is currently a game active", 0)}),

            new KeyedPhrases(PhraseKeys.Register.InvalidName,new List<Phrase>{new Phrase("Invalid name", 0)}),
            new KeyedPhrases(PhraseKeys.Register.NoActiveGame,new List<Phrase>{new Phrase("No active game found", 0)}),
            new KeyedPhrases(PhraseKeys.Register.ActiveRound,new List<Phrase>{new Phrase("There is an active round, wait for the round to end", 0)}),
            new KeyedPhrases(PhraseKeys.Register.AlreadyRegistered,new List<Phrase>{new Phrase("{0} is already registered", 0)}),

            new KeyedPhrases(PhraseKeys.DeRegister.InvalidName,new List<Phrase>{new Phrase("Invalid name", 0)}),
            new KeyedPhrases(PhraseKeys.DeRegister.NotRegistered,new List<Phrase>{new Phrase("{0} is not registered", 0)}),

            new KeyedPhrases(PhraseKeys.StartRound.NoActiveGame,new List<Phrase>{new Phrase("No active game found", 0)}),
            new KeyedPhrases(PhraseKeys.StartRound.ActiveRound,new List<Phrase>{new Phrase("There is already a round active", 0)}),
            new KeyedPhrases(PhraseKeys.StartRound.NotEnoughPlayers,new List<Phrase>{new Phrase("Need at least 2 players to play", 0)}),

            new KeyedPhrases(PhraseKeys.Next.NoActiveGame,new List<Phrase>{new Phrase("No active game found", 0)}),
            new KeyedPhrases(PhraseKeys.Next.NoActiveRound,new List<Phrase>{new Phrase("No active game found", 0)}),
            new KeyedPhrases(PhraseKeys.Next.NoRemainingItems,new List<Phrase>{new Phrase("No items remaining, everyone must have fallen asleep", 0)}),
            new KeyedPhrases(PhraseKeys.Next.Success,new List<Phrase>{new Phrase("And the next one is {0}", 0)}),

            new KeyedPhrases(PhraseKeys.Bingo.NotRegistered,new List<Phrase>{new Phrase("You are not registered", 0)}),
            new KeyedPhrases(PhraseKeys.Bingo.Mistake,new List<Phrase>{new Phrase("We think you might have made a mistake there", 0)}),

            new KeyedPhrases(PhraseKeys.Stop.NoActive,new List<Phrase>{new Phrase("No active game found", 0)}),
        };
    }
}
