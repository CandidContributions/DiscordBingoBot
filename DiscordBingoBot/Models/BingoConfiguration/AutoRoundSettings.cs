namespace DiscordBingoBot.Models.BingoConfiguration
{
    public class AutoRoundSettings
    {
        public int MinimumTimeout { get; set; }
        public int MaximumTimeout { get; set; }
        public int PreferedTimeout { get; set; }
        public int PreferedTimeoutSkewPercentage { get; set; }
    }
}