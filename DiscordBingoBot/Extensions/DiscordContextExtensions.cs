using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBingoBot.Extensions
{
    public static class DiscordContextExtensions
    {
        public static string GetChannelGuildIdentifier(this SocketCommandContext context)
        {
            return context.Guild.Name + "|" + context.Channel.Name;
        }
    }
}
