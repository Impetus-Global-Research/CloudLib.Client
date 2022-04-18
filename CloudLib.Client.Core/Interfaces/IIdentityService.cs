using CloudLib.Client.Core.Helpers;
using System;
using System.Threading.Tasks;

namespace CloudLib.Client.Core.Services
{
    public interface IIdentityService
    {
        event EventHandler LoggedIn;
        event EventHandler LoggedOut;

        Task<bool> AcquireTokenSilentAsync();
        Task<string> GetAccessTokenForGraphAsync();
        Task<string> GetAccessTokenForWebApiAsync();
        string? GetAccountUserName();
        bool IsAuthorized();
        bool IsLoggedIn();
        Task<LoginResultType> LoginAsync();
        Task LogoutAsync();
    }
}