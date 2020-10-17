using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBingoBot.Services
{
    public interface IAutoNextService
    {
        bool Paused { get; }
        Task Start(SocketCommandContext context);
        void Pause();
    }
}