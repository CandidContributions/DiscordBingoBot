using Discord.Commands;

namespace DiscordBingoBot.Services
{
    public interface IPermissionHandler
    {
        bool HasBingoManagementPermissions(SocketCommandContext context);
    }
}