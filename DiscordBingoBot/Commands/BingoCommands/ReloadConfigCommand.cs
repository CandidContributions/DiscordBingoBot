using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class ReloadConfigCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;
        private readonly IPermissionHandler _permissionHandler;

        public ReloadConfigCommand(IBingoService bingoService, IPermissionHandler permissionHandler)
        {
            _bingoService = bingoService;
            _permissionHandler = permissionHandler;
        }

        [Command("reloadConfig")]
        [Summary("Reloads the config, doing this mid game should not change the word list for the active round")]
        public async Task ReloadConfig()
        {
            if (_permissionHandler.HasBingoManagementPermissions(Context) == false)
            {
                return;
            }

            var message = Context.Message;

            // todo should add a way to handle errors
            await message.DeleteAsync();
            await Context.User.SendMessageAsync("Updating configuration");
            await _bingoService.LoadConfiguration(true);
            await Context.User.SendMessageAsync("Configuration updated");
        }
    }
}
