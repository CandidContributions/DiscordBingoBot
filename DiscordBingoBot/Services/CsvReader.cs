using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Discord;

namespace DiscordBingoBot.Services
{
    public class CsvReader : ICsvReader
    {
        private readonly ILogger _logger;

        public CsvReader(ILogger logger)
        {
            _logger = logger;
        }

        public List<string> Read(string path)
        {
            var text = File.ReadAllText(path);
            var delimiter = FindDelimiter(text);
            var items = text.Split(delimiter).Where(s => s.Trim().Length > 0).ToList();
            _logger.Info("CsvReader.Read found " + items.Count + " items");
            return items;
        }

        private string FindDelimiter(string input)
        {
            var validCharacterRegex = new Regex(@"^[a-zA-Z0-9]*$");

            // find the first non alphanumeric character
            var delimiterStart = 0;
            var currentIndex = 0;
            while (currentIndex < input.Length && delimiterStart == 0)
            {
                if (validCharacterRegex.IsMatch(input.Substring(currentIndex, 1)) == false)
                {
                    delimiterStart = currentIndex;
                }

                currentIndex++;
            }
            // find the first alphanumeric character after the first invalid
            var delimiterEnd = 0;
            while (currentIndex < input.Length && delimiterEnd == 0)
            {
                if (validCharacterRegex.IsMatch(input.Substring(currentIndex, 1)))
                {
                    delimiterEnd = currentIndex;
                }

                currentIndex++;
            }

            // return the substring
            var delimiter = input.Substring(delimiterStart, delimiterEnd - delimiterStart);
            if (delimiter.Length > 0)
            {
                _logger.Info("CsvReader.FindDelimiter found \"" + delimiter + "\"");
            }
            return delimiter;
        }
    }
}
