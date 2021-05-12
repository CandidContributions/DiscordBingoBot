using System.Threading.Tasks;
using BingoCore.Services;
using Discord.Commands;
using DiscordBingoBot.Extensions;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class LeaveCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;

        public LeaveCommand(IBingoService bingoService)
        {
            _bingoService = bingoService;
        }

        [Command("leave", RunMode = RunMode.Async)]
        [Summary("Leave the active bingo game you previously joined")]
        public async Task Leave()
        {
            var message = Context.Message;

            var bingoGame = _bingoService.GetGame(Context.GetChannelGuildIdentifier());

            var result = await bingoGame.DeRegister(Context.User.Mention).ConfigureAwait(false);
            if (result.Result)
            {
                await ReplyAsync(Context.User.Mention + " has left the Bingo game");
            }
            else
            {
                await ReplyAsync(Context.User.Mention + " can't leave the Bingo game: " + result.Info);
            }

            await message.DeleteAsync();
        }
    }
}
