using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class NewRoundCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;

        public NewRoundCommand(IBingoService bingoService)
        {
            _bingoService = bingoService;
        }

        [Command("newRound")]
        [Summary("Starts a new round in the active bingo game")]
        public async Task Start()
        {
            var message = Context.Message;

            var result = _bingoService.StartRound();
            if (result.Result)
            {
                await ReplyAsync("A new round has been started");
            }
            else
            {
                await Context.User.SendMessageAsync("Can't start a new round: " + result.Reason);
            }

            await message.DeleteAsync();
        }
    }
}
