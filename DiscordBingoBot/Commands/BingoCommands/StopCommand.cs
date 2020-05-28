using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class StopCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;

        public StopCommand(IBingoService bingoService)
        {
            _bingoService = bingoService;
        }

        [Command("stop")]
        [Summary("Stops the active bingo game")]
        public async Task Stop()
        {
            var message = Context.Message;

            var result = _bingoService.Stop();
            if (result.Result)
            {
                await ReplyAsync("Bingo game stopped by " + Context.User.Mention);
            }
            else
            {
                await Context.User.SendMessageAsync("Can't stop the bingo game: " + result.Reason);
            }

            await message.DeleteAsync();
        }
    }
}
