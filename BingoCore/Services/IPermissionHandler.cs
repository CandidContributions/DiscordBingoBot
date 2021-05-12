namespace BingoCore.Services
{
    public interface IPermissionHandler
    {
        bool HasBingoManagementPermissions(object context);
    }
}