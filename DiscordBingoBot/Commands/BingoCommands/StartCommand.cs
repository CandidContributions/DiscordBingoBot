using System.Linq;
using System.Threading.Tasks;
using BingoCore.Services;
using Discord;
using Discord.Commands;
using DiscordBingoBot.Services;
using Microsoft.Extensions.Configuration;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class StartCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;
        private readonly CommandService _commandService;
        private readonly IConfigurationRoot _configuration;
        private readonly IPermissionHandler _permissionHandler;

        public StartCommand(IBingoService bingoService, CommandService commandService,
            IConfigurationRoot configuration, IPermissionHandler permissionHandler)
        {
            _bingoService = bingoService;
            _commandService = commandService;
            _configuration = configuration;
            _permissionHandler = permissionHandler;
        }

        [Command("start")]
        [Summary("Starts a new bingo game")]
        [RequireUserPermission(GuildPermission.Connect, Group = "test", ErrorMessage = "Failed test")]
        public async Task Start()
        {
            if (_permissionHandler.HasBingoManagementPermissions(Context) == false)
            {
                return;
            }

            var message = Context.Message;

            var result = await _bingoService.Start().ConfigureAwait(false);
            if (result.Result)
            {
                await ReplyAsync("Bingo game started by " + Context.User.Mention);
                var joinCommand = _commandService.Commands.First(c => c.Name == "join");
                await ReplyAsync("You can join the game by typing " + _configuration["DiscordBotPrefix"] + joinCommand.Name);
            }
            else
            {
                await Context.User.SendMessageAsync("Can't start a bingo game: " + result.Info);
            }

            await message.DeleteAsync();
        }
    }
}
