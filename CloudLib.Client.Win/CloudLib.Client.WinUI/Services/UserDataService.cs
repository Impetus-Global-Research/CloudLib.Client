using System;
using System.Threading.Tasks;
using Windows.Storage;
using CloudLib.Client.WinUI.Core.Helpers;
using CloudLib.Client.WinUI.Core.Interfaces;
using CloudLib.Client.WinUI.Core.Services;
using CloudLib.Client.WinUI.Core.Models;
using CloudLib.Client.WinUI.Extensions;
using CloudLib.Client.WinUI.Helpers;
using CloudLib.Client.WinUI.ViewModels;

namespace CloudLib.Client.WinUI.Services
{
    public class UserDataService
    {
        private const string USER_SETTINGS_KEY = "IdentityUser";
        private const string DEFAULT_GUEST_USER_NAME = "Guest User";

        private UserViewModel? _user;

        //private IIdentityService IdentityService => Singleton<CognitoIdentityService>.Instance;

        private MicrosoftGraphService MicrosoftGraphService => Singleton<MicrosoftGraphService>.Instance;

        public event EventHandler<UserViewModel?> UserDataUpdated;

        public UserDataService()
        {
        }

        public void Initialize()
        {
            //IdentityService.LoggedIn += OnLoggedIn;
            //IdentityService.LoggedOut += OnLoggedOut;
        }

        public async Task<UserViewModel?> GetUserAsync()
        {
            if (_user == null)
            {
                _user = await GetUserFromCacheAsync();
                if (_user == null)
                {
                    _user = GetDefaultUserData();
                }
            }

            return _user;
        }

        private async void OnLoggedIn(object sender, EventArgs e)
        {
            //_user = await GetUserFromGraphApiAsync();
            //TODO: Replace with real user data fetch logic
            _user = GetDefaultUserData();

            UserDataUpdated?.Invoke(this, _user);
        }

        private async void OnLoggedOut(object sender, EventArgs e)
        {
            _user = null;
            await ApplicationData.Current.LocalFolder.SaveAsync<User>(USER_SETTINGS_KEY, null);
        }

        private async Task<UserViewModel?> GetUserFromCacheAsync()
        {
            var cacheData = await ApplicationData.Current.LocalFolder.ReadAsync<User>(USER_SETTINGS_KEY);
            return await GetUserViewModelFromData(cacheData);
        }

        //private async Task<UserViewModel?> GetUserFromGraphApiAsync()
        //{
        //    var accessToken = await IdentityService.GetAccessTokenForGraphAsync();
        //    if (string.IsNullOrEmpty(accessToken))
        //    {
        //        return null;
        //    }

        //    var userData = await MicrosoftGraphService.GetUserInfoAsync(accessToken);
        //    if (userData != null)
        //    {
        //        userData.Photo = await MicrosoftGraphService.GetUserPhoto(accessToken);
        //        await ApplicationData.Current.LocalFolder.SaveAsync(_userSettingsKey, userData);
        //    }

        //    return await GetUserViewModelFromData(userData);
        //}

        private async Task<UserViewModel?> GetUserViewModelFromData(User? userData)
        {
            if (userData == null)
            {
                return null;
            }

            var userPhoto = string.IsNullOrEmpty(userData.Photo)
                ? ImageHelper.ImageFromAssetsFile("DefaultIcon.png")
                : await ImageHelper.ImageFromStringAsync(userData.Photo);

            return new UserViewModel()
            {
                Name = userData.DisplayName,
                UserPrincipalName = userData.UserPrincipalName,
                Photo = userPhoto
            };
        }

        private UserViewModel? GetDefaultUserData()
        {
            return new UserViewModel()
            {
                Name = DEFAULT_GUEST_USER_NAME,
                Photo = ImageHelper.ImageFromAssetsFile("DefaultIcon.png")
            };
        }
    }
}
