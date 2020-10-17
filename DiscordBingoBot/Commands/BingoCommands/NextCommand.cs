using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class NextCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;
        private readonly IPermissionHandler _permissionHandler;

        public NextCommand(IBingoService bingoService, IPermissionHandler permissionHandler)
        {
            _bingoService = bingoService;
            _permissionHandler = permissionHandler;
        }

        [Command("next")]
        [Summary("Calls the next item in a bingo round")]
        public async Task Next()
        {
            if (_permissionHandler.HasBingoManagementPermissions(Context) == false)
            {
                return;
            }

            var message = Context.Message;

            var next = await _bingoService.NextItem().ConfigureAwait(false);
            if (next.Result)
            {
                var reply = "And the next one is " + next.Info;
                if (_bingoService.Verbose)
                {
                    await ReplyAsync(reply);
                }
                else
                {
                    await Context.User.SendMessageAsync(reply);
                }
                
            }
            else
            {
                await Context.User.SendMessageAsync("Can't call a new item: " + next.Info);
            }

            await message.DeleteAsync();
        }
    }
}
