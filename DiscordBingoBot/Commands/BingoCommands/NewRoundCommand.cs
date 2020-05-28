using System.Linq;
using System.Text;
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
        private readonly ILogger _logger;

        public NewRoundCommand(IBingoService bingoService, ILogger logger)
        {
            _bingoService = bingoService;
            _logger = logger;
        }

        [Command("newRound")]
        [Summary("Starts a new round in the active bingo game")]
        public async Task Start()
        {
            var message = Context.Message;

            var result = _bingoService.StartRound();
            if (result.Result)
            {
                await ReplyAsync("A new round is starting, handing out new cards");
                foreach (var player in _bingoService.Players)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("Your new card: " + player.Grid.GridId);
                    for (var index = 0; index < player.Grid.Rows.Length; index++)
                    {
                        stringBuilder.AppendLine("Row" + (index + 1) + ": " +
                                                 string.Join(" | ", player.Grid.Rows[index].Items));
                    }

                    var playerSocket = Context.Guild.Users.FirstOrDefault(u => u.Mention == player.Name);
                    if (playerSocket == null)
                    {
                       await _logger.Warn("Player " + player.Name + " no longer exists");
                       continue;
                    }

                    await playerSocket.SendMessageAsync(stringBuilder.ToString());
                }

                await ReplyAsync("A new round has been started");
            }
            else
            {
                await Context.User.SendMessageAsync("Can't start a new round: " + result.Info);
            }

            await message.DeleteAsync();
        }
    }
}
