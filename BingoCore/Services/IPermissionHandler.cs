namespace DiscordBingoBot.Services
{
    public interface IPermissionHandler
    {
        bool HasBingoManagementPermissions(object context);
    }
}