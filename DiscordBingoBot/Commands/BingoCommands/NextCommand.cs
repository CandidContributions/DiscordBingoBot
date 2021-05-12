using System.Threading.Tasks;
using BingoCore.Constants;
using BingoCore.Services;
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
        private readonly IConfigurationService _configurationService;

        public NextCommand(IBingoService bingoService, IPermissionHandler permissionHandler,
            IConfigurationService configurationService)
        {
            _bingoService = bingoService;
            _permissionHandler = permissionHandler;
            _configurationService = configurationService;
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
                var reply = string.Format(await _configurationService.GetPhrase(PhraseKeys.Next.Success).ConfigureAwait(false), next.Info);
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
