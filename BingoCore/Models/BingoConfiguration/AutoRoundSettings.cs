namespace BingoCore.Models.BingoConfiguration
{
    public class AutoRoundSettings
    {
        public int MinimumTimeout { get; set; }
        public int MaximumTimeout { get; set; }
        public int PreferredTimeout { get; set; }
        public int PreferredTimeoutSkewPercentage { get; set; }
    }
}