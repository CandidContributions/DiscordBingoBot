using System.Collections.Generic;

namespace BingoCore.Services
{
    public interface ICsvReader
    {
        List<string> Read(string path);
    }
}