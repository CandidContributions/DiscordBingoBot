using System.Collections.Generic;

namespace BingoCore.Services
{
    public interface IBingoService
    {
        BingoGame GetGame(string identifier);
        void RemoveGame(string identifier);
        List<string> GetIdentifiers();

        /// <summary>
        /// Warning, this method extends the cache retention time on all games
        /// </summary>
        void RefreshIdentifiers();

        /// <summary>
        /// Warning, this method extends the cache retention time on all games
        /// </summary>
        List<string> GetJoinableIdentifier();

        List<KeyValuePair<string, BingoGame>> GetGames();
        bool GameExists(string gameIdentifier);
    }
}