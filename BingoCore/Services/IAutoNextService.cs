using System.Threading.Tasks;

namespace BingoCore.Services
{
    public interface IAutoNextService
    {
        Task<bool> Start(object context);
        bool Pause(object context);
        bool Stop(object context);
        bool IsPaused(object context);
    }
}