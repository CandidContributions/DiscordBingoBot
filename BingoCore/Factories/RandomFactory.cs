using System;

namespace BingoCore.Factories
{
    public static class RandomFactory
    {
        public static Random FromGuid(Guid seed)
        {
            return new Random(BitConverter.ToInt32(seed.ToByteArray(), 0));
        }
    }
}
