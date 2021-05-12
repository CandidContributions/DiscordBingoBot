using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BingoCore.Services;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Extensions;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class PlayerListCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;
        private readonly IPermissionHandler _permissionHandler;

        public PlayerListCommand(IBingoService bingoService, IPermissionHandler permissionHandler)
        {
            _bingoService = bingoService;
            _permissionHandler = permissionHandler;
        }

        [Command("players", RunMode = RunMode.Async)]
        [Summary("Shows a list of active players")]
        public async Task List()
        {
            if (_permissionHandler.HasBingoManagementPermissions(Context) == false)
            {
                return;
            }

            var message = Context.Message;
            var bingoGame = _bingoService.GetGame(Context.GetChannelGuildIdentifier());

            var players = bingoGame.Players;
            var builder = new EmbedBuilder
            {
                Color = new Color(114, 137, 218)
            };

            builder.AddField(x =>
            {
                x.Name = "Players in the active bingo game";
                x.Value = string.Join('\n', players.Select(p => p.Name));
                x.IsInline = false;
            });

            await ReplyAsync("", false, builder.Build());
            await message.DeleteAsync();
        }
    }
}
