using System.Threading.Tasks;
using DiscordBingoBot.Models;
using DiscordBingoBot.Models.BingoConfiguration;

namespace DiscordBingoBot.Services
{
    public interface IConfigurationService
    {
        Task<BingoConfiguration> GetConfiguration(bool refresh = false);
        Task<string> GetPhrase(string key);
    }
}