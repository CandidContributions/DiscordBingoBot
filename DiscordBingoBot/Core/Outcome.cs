using System;

namespace DiscordBingoBot.Core
{
    /// <summary>
    /// Returns a boolean and the reason why that result was returned if it was false
    /// </summary>
    public struct Outcome<TReason> : IEquatable<Outcome<TReason>>
    {
        private readonly TReason _reason;

        public bool Result { get; }

        public TReason Reason => _reason;

        private Outcome(bool result, TReason reason)
        {
            Result = result;
            _reason = reason;
        }

        public static Outcome<TReason> Success()
        {
            return new Outcome<TReason>(true, default);
        }

        public static Outcome<TReason> Fail(TReason reason)
        {
            return new Outcome<TReason>(false, reason);
        }

        public bool Equals(Outcome<TReason> other)
        {
            return Result == other.Result && Reason.Equals(other.Reason);
        }

        public override bool Equals(object obj)
        {
            return obj is Outcome<TReason> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Result.GetHashCode() * 397) ^ _reason.GetHashCode();
            }
        }
    }
}
