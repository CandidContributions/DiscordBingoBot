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
    public class StopCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IAutoNextService _autoNextService;

        public StopCommand(IBingoService bingoService, IPermissionHandler permissionHandler, IAutoNextService autoNextService)
        {
            _bingoService = bingoService;
            _permissionHandler = permissionHandler;
            _autoNextService = autoNextService;
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
            var bingoGame = _bingoService.GetGame(Context.GetChannelGuildIdentifier());

            var result = await bingoGame.Stop().ConfigureAwait(false);
            if (result.Result)
            {
                _autoNextService.Stop(Context);
                _bingoService.RemoveGame(Context.GetChannelGuildIdentifier());
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
