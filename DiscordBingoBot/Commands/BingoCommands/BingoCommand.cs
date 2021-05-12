using System.Linq;
using System.Threading.Tasks;
using BingoCore.Services;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;
using Microsoft.Extensions.Configuration;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class BingoCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;
        private readonly CommandService _commandService;
        private readonly IConfigurationRoot _configuration;

        public BingoCommand(IBingoService bingoService, CommandService commandService, IConfigurationRoot configuration)
        {
            _bingoService = bingoService;
            _commandService = commandService;
            _configuration = configuration;
        }

        [Command("bingo")]
        [Summary("Call out you that you have bingo")]
        public async Task Bingo()
        {
            var message = Context.Message;

            var result = await _bingoService.CheckBingo(Context.User.Mention).ConfigureAwait(false);
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
                    var joinCommand = _commandService.Commands.First(c => c.Name == "join");
                    await ReplyAsync("That concludes this round, registrations are open again type "+ _configuration["DiscordBotPrefix"] + joinCommand.Name);
                }
            }
            else
            {
                await Context.User.SendMessageAsync("Invalid bingo call: " + result.Info.Error);
            }

            await message.DeleteAsync();
        }
    }
}
