using System.Threading.Tasks;

namespace BingoCore.Services
{
    public interface IAutoNextService
    {
        bool Paused { get; }
        Task Start(object context);
        void Pause();
    }
}