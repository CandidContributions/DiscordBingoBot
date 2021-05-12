using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingoCore.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class NewRoundCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;
        private readonly ILogger _logger;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IAutoNextService _autoNextService;

        public NewRoundCommand(IBingoService bingoService, ILogger logger,
            IPermissionHandler permissionHandler, IAutoNextService autoNextService)
        {
            _bingoService = bingoService;
            _logger = logger;
            _permissionHandler = permissionHandler;
            _autoNextService = autoNextService;
        }

        [Command("newRound",RunMode = RunMode.Async)]
        [Summary("Starts a new round in the active bingo game")]
        public async Task NewRound()
        {
            await ExecuteNewRound();
        }

        [Command("newRound", RunMode = RunMode.Async)]
        [Summary("Starts a new round in the active bingo game")]
        public async Task NewRoundVerbose(
            [Summary("Mode (verbose|silent)")]string mode)
        {
            switch (mode)
            {
                case "verbose":
                    await ExecuteNewRound(true);
                    return;
                case "silent":
                    await ExecuteNewRound(false);
                    return;
                default:
                    await Context.Guild.GetUser(Context.User.Id)
                        .SendMessageAsync("Invalid parameters in command (" + Context.Message.Content + ")");
                    return;
            }
        }

        [Command("newRoundAuto", RunMode = RunMode.Async)]
        [Summary("Starts a new round in the active bingo game that automatically calls out words")]
        public async Task NewRoundAutomatic()
        {
            await ExecuteNewRound(auto: true);

            if (_bingoService.IsRoundActive)
            {
                // succeeded in starting a round => start the automatic calling
                await _autoNextService.Start(Context);
                await Context.User.SendMessageAsync("Automatic round started");
            }
        }

        private async Task ExecuteNewRound(bool verbose = true, bool auto = false)
        {
            const char gridSeperator = '*';

            if (_permissionHandler.HasBingoManagementPermissions(Context) == false)
            {
                return;
            }

            var message = Context.Message;

            var result = await _bingoService.StartRound(verbose).ConfigureAwait(false);
            if (result.Result)
            {
                await ReplyAsync("A new round is starting with " + result.Info.NumberOfWinConditions + " win conditions");
                await ReplyAsync("Handing out new cards");
                foreach (var player in _bingoService.Players)
                {
                    var playerSocket = Context.Guild.Users.FirstOrDefault(u => u.Mention == player.Name);
                    if (playerSocket == null)
                    {
                        await _logger.Warn("Player " + player.Name + " no longer exists");
                        continue;
                    }

                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("Your new card: " + player.Grid.GridId);
                    stringBuilder.AppendLine();

                    var longestWordLength = player.Grid.Rows.SelectMany(r => r.Items).Max(i => i.Length);
                    var cellWidth = longestWordLength + 2;
                    var gridWith = cellWidth * player.Grid.Rows.Length + player.Grid.Rows.Length;
                    stringBuilder.AppendLine("```" + "".PadRight(gridWith + 1, gridSeperator));
                    for (var index = 0; index < player.Grid.Rows.Length; index++)
                    {
                        //stringBuilder.AppendLine(string.Join(" | ", player.Grid.Rows[index].Items.Select(w => w.PadRight(25))));
                        stringBuilder.Append(gridSeperator);
                        foreach (var item in player.Grid.Rows[index].Items)
                        {
                            var leftPad = (cellWidth - item.Length) / 2;
                            stringBuilder.Append("".PadRight(leftPad, ' ') + item +
                                                 "".PadRight(cellWidth - item.Length - leftPad, ' ') + gridSeperator);
                            
                        }
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine("".PadRight(gridWith +1, gridSeperator));
                    }
                    stringBuilder.Append("```");

                    await playerSocket.SendMessageAsync(stringBuilder.ToString());
                }

                await ReplyAsync("A new round has been started");
                await ReplyAsync("The first win condition is: " + result.Info.FirstWinCondition);
            }
            else
            {
                await Context.User.SendMessageAsync("Can't start a new round: " + result.Info.Error);
            }

            await message.DeleteAsync();
        }
    }
}
