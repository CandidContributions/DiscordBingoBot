﻿using System.Threading.Tasks;
using Discord.Commands;
using DiscordBingoBot.Services;

namespace DiscordBingoBot.Commands.BingoCommands
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
    public class LeaveCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IBingoService _bingoService;

        public LeaveCommand(IBingoService bingoService)
        {
            _bingoService = bingoService;
        }

        [Command("leave", RunMode = RunMode.Async)]
        [Summary("Leave the active bingo game you previously joined")]
        public async Task Leave()
        {
            var message = Context.Message;

            var result = _bingoService.DeRegister(Context.User.Mention);
            if (result.Result)
            {
                await ReplyAsync(Context.User.Mention + " has left the Bingo game");
            }
            else
            {
                await ReplyAsync(Context.User.Mention + " can't leave the Bingo game: " + result.Info);
            }

            await message.DeleteAsync();
        }
    }
}