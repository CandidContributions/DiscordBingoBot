using System.Collections.Generic;

namespace DiscordBingoBot.Services
{
    public interface ICsvReader
    {
        List<string> Read(string path);
    }
}