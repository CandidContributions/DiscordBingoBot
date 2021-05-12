using System.Threading.Tasks;
using BingoCore.Services;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class PauseCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IAutoNextService _autoNextService;

        public PauseCommand(IBingoService bingoService, IPermissionHandler permissionHandler,
            IAutoNextService autoNextService)
        {
            _bingoService = bingoService;
            _permissionHandler = permissionHandler;
            _autoNextService = autoNextService;
        }

        [Command("pause")]
        [Summary("Pauses auto calling the current round")]
        public async Task Pause()
        {
            if (_permissionHandler.HasBingoManagementPermissions(Context) == false)
            {
                return;
            }

            var message = Context.Message;
            await message.DeleteAsync();

            if (_bingoService.IsRoundActive == false)
            {
                //todo return a message
            }
            else if (_autoNextService.Paused)
            {
                //todo return a message
            }
            else {
                _autoNextService.Pause();
                await Context.User.SendMessageAsync("Paused next calling");
            }
        }
    }
}
