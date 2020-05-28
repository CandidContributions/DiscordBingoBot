using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace DiscordBingoBot.Services
{
    public class PermissionHandler : IPermissionHandler
    {
        private readonly IConfigurationRoot _configuration;

        public PermissionHandler(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public bool HasBingoManagementPermissions(SocketCommandContext context)
        {
            if (context == null)
            {
                return false;
            }

            var guildUser = context.Guild.GetUser(context.User.Id);
            var roles = _configuration["BingoManagementRoles"].Split(',');
            if (guildUser.Roles.Any(role => roles.Any(roleName => roleName == role.Name)))
            {
                return true;
            }

            context.Message.DeleteAsync();
            guildUser.SendMessageAsync("You do not have permissions to execute this command (" +
                                       context.Message.Content + ")");
            return false;
        }
    }
}
