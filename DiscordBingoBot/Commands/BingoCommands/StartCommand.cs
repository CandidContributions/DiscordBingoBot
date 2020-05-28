using System.Linq;
using System.Threading.Tasks;
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

        public StartCommand(IBingoService bingoService, CommandService commandService,
            IConfigurationRoot configuration)
        {
            _bingoService = bingoService;
            _commandService = commandService;
            _configuration = configuration;
        }

        [Command("start")]
        [Summary("Starts a new bingo game")]
        public async Task Start()
        {
            var message = Context.Message;

            var result = _bingoService.Start();
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
