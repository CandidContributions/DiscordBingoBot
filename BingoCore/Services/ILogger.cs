using System.Threading.Tasks;

namespace DiscordBingoBot.Services
{
    public interface ILogger
    {
        Task Log(string msg);
        Task Info(string msg);
        Task Warn(string msg);
    }
}