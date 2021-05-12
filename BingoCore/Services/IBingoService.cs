using System.Collections.Generic;
using System.Threading.Tasks;
using BingoCore.Core;
using BingoCore.Models;
using BingoCore.Outcomes;

namespace BingoCore.Services
{
    public interface IBingoService
    {
        Task<Outcome<string>> Start();
        Task<Outcome<string>> Register(string mention, string nickName);
        Task<Outcome<StartRoundOutcome>> StartRound(bool verbose);
        Task<Outcome<string>> NextItem();
        Task<Outcome<CheckBingoOutcome>> CheckBingo(string playerName);
        Task<Outcome<string>> Stop();
        IReadOnlyCollection<Player> Players { get; }
        bool Verbose { get; }
        bool IsActive { get; }
        bool IsRoundActive { get; }
        Task<Outcome<string>> DeRegister(string name);
        Task LoadConfiguration(bool force = false);
    }
}