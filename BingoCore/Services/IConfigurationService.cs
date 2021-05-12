using System.Threading.Tasks;
using BingoCore.Models.BingoConfiguration;

namespace BingoCore.Services
{
    public interface IConfigurationService
    {
        Task<BingoConfiguration> GetConfiguration(bool refresh = false);
        Task<string> GetPhrase(string key);
    }
}