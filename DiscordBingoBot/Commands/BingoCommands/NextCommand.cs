using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class NextCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;

        public NextCommand(IBingoService bingoService)
        {
            _bingoService = bingoService;
        }

        [Command("next")]
        [Summary("Calls the next item in a bingo round")]
        public async Task Next()
        {
            var message = Context.Message;

            var next = _bingoService.NextItem();
            if (next.Result)
            {
                await ReplyAsync("And the next one is " + next.Reason);
            }
            else
            {
                await Context.User.SendMessageAsync("Can't call a new item: " + next.Reason);
            }

            await message.DeleteAsync();
        }
    }
}
