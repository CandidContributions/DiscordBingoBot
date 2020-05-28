using System;
using System.Collections.Generic;
using System.Text;
using DiscordBingoBot.Core;

namespace DiscordBingoBot.Services
{
    public class BingoService : IBingoService
    {
        private bool hasStarted;

        public Outcome<string> Start()
        {
            if (hasStarted)
            {
                return Outcome<string>.Fail("There is currently a game active");
            }

            hasStarted = true;
            return Outcome<string>.Success();
        }
    }
}
