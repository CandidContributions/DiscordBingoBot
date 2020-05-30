using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
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

            var players = _bingoService.Players;
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "Players in the active bingo game"
            };

            foreach (var player in players)
            {
                builder.AddField(x =>
                {

                    x.Name = player.NickName;
                    x.IsInline = false;
                });
            }

            await ReplyAsync("Active Player List", false, builder.Build());
            await message.DeleteAsync();
        }
    }
}
