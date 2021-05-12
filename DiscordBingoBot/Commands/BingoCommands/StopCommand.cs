using System.Threading.Tasks;
using BingoCore.Services;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class StopCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;
        private readonly IPermissionHandler _permissionHandler;

        public StopCommand(IBingoService bingoService, IPermissionHandler permissionHandler)
        {
            _bingoService = bingoService;
            _permissionHandler = permissionHandler;
        }

        [Command("stop")]
        [Summary("Stops the active bingo game")]
        public async Task Stop()
        {
            if (_permissionHandler.HasBingoManagementPermissions(Context) == false)
            {
                return;
            }

            var message = Context.Message;

            var result = await _bingoService.Stop().ConfigureAwait(false);
            if (result.Result)
            {
                await ReplyAsync("Bingo game stopped by " + Context.User.Mention);
            }
            else
            {
                await Context.User.SendMessageAsync("Can't stop the bingo game: " + result.Info);
            }

            await message.DeleteAsync();
        }
    }
}
