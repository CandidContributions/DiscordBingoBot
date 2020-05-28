using System.Threading.Tasks;
using Discord;

namespace DiscordBingoBot.Services
{
    public interface ILogger
    {
        Task Log(LogMessage msg);
        Task Info(string msg);
        Task Warn(string msg);
    }
}