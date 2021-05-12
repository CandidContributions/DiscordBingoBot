using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordBingoBot.Services
{
    public class Logger : ILogger
    {
        public Task Log(LogMessage msg)
        {
            return Log(msg.ToString());
        }

        public Task Log(string msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }

        public Task Info(string msg)
        {
            Console.WriteLine("[Info] "+msg);
            return Task.CompletedTask;
        }

        public Task Warn(string msg)
        {
            Console.WriteLine("[Warning] " + msg);
            return Task.CompletedTask;
        }
    }
}
