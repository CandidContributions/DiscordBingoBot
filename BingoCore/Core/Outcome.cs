using System;

namespace DiscordBingoBot.Core
{
    /// <summary>
    /// Returns a boolean and the reason why that result was returned if it was false
    /// </summary>
    public struct Outcome<Tinfo> : IEquatable<Outcome<Tinfo>>
    {
        private readonly Tinfo _info;

        public bool Result { get; }

        public Tinfo Info => _info;

        private Outcome(bool result, Tinfo info)
        {
            Result = result;
            _info = info;
        }

        public static Outcome<Tinfo> Success()
        {
            return new Outcome<Tinfo>(true, default);
        }

        public static Outcome<Tinfo> Success(Tinfo value)
        {
            return new Outcome<Tinfo>(true, value);
        }

        public static Outcome<Tinfo> Fail(Tinfo info)
        {
            return new Outcome<Tinfo>(false, info);
        }

        public bool Equals(Outcome<Tinfo> other)
        {
            return Result == other.Result && Info.Equals(other.Info);
        }

        public override bool Equals(object obj)
        {
            return obj is Outcome<Tinfo> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Result.GetHashCode() * 397) ^ _info.GetHashCode();
            }
        }
    }
}
