using System;
using System.Threading.Tasks;
using CloudLib.Client.WinUI.Core.Helpers;

namespace CloudLib.Client.WinUI.Core.Interfaces
{
    public interface IIdentityService
    {
        event EventHandler LoggedIn;
        event EventHandler LoggedOut;

        Task<bool> AcquireTokenSilentAsync();
        Task<string> GetAccessTokenForGraphAsync();
        Task<string> GetAccessTokenForWebApiAsync();
        string GetAccountUserName();
        bool IsAuthorized();
        bool IsLoggedIn();
        Task<LoginResultType> LoginAsync();
        Task LogoutAsync();
    }
}