namespace DiscordBingoBot.Models.BingoConfiguration
{
    public class Phrase
    {
        public string Text { get; set; }
        public int Boost { get; set; }

        public Phrase()
        {
            
        }

        public Phrase(string text, int boost)
        {
            Text = text;
            Boost = boost;
        }
    }
}