using System.Threading.Tasks;
using ApexDiscord.Bot.Attributes;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace ApexDiscord.Bot.Commands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class StartCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;

        public StartCommand(IBingoService bingoService)
        {
            _bingoService = bingoService;
        }

        [Command("start")]
        [Summary("Starts a new bingo game")]
        public async Task Start()
        {
            var result = _bingoService.Start();
            if (result.Result)
            {
                await ReplyAsync("Bingo game started");
            }
            else
            {
                await ReplyAsync("Can't stat a bingo game: " + result.Reason);
            }

        }
    }
}
