using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBingoBot.Extenstions
{
    public static class RandomFactory
    {
        public static Random FromGuid(Guid seed)
        {
            return new Random(BitConverter.ToInt32(seed.ToByteArray(), 0));
        }
    }
}
