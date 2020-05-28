using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
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
            var message = Context.Message;

            var result = _bingoService.Start();
            if (result.Result)
            {
                await ReplyAsync("Bingo game started by " + Context.User.Mention);
            }
            else
            {
                await Context.User.SendMessageAsync("Can't start a bingo game: " + result.Info);
            }

            await message.DeleteAsync();
        }
    }
}
