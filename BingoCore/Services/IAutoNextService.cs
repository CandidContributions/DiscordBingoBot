using System.Threading.Tasks;

namespace DiscordBingoBot.Services
{
    public interface IAutoNextService
    {
        bool Paused { get; }
        Task Start(object context);
        void Pause();
    }
}