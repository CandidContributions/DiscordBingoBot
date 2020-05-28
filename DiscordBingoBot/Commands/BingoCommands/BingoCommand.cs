using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class BingoCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;

        public BingoCommand(IBingoService bingoService)
        {
            _bingoService = bingoService;
        }

        [Command("bingo")]
        [Summary("Call out you that you have bingo")]
        public async Task Bingo()
        {
            var message = Context.Message;

            var result = _bingoService.CheckBingo(Context.User.Mention);
            if (result.Result)
            {
                await ReplyAsync(Context.User.Mention + " is a winner!");
                // todo send the user a nice message

                if (result.Info.RoundHasEnded == false)
                {
                    await ReplyAsync("The next win condition is: " + result.Info.NextWinCondition);
                }
                else
                {
                    await ReplyAsync("That concludes this round, registrations are open again");
                }
            }
            else
            {
                await Context.User.SendMessageAsync("Invalid bingo call: " + result.Info);
            }

            await message.DeleteAsync();
        }
    }
}
