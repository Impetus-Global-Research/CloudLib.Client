using System;
using System.Configuration;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using CloudLib.Client.WinUI.Core.Helpers;
using CloudLib.Client.WinUI.Core.Interfaces;

namespace CloudLib.Client.WinUI.Core.Services
{
    public class CognitoIdentityService : IIdentityService
    {
        private readonly CancellationTokenSource _cancellationSource;
        private const string UNAUTHENTICATED_USERNAME = @"GUEST USER";

        private readonly string _clientId = ConfigurationManager.AppSettings["IdentityClientId"];
        //TODO: Replace this with a production ready secret management solution!
        private readonly string _clientSecret = ConfigurationManager.AppSettings["IdentityClientSecret"];
        private readonly RegionEndpoint _AwsRegion = RegionEndpoint.GetBySystemName(ConfigurationManager.AppSettings["AWSRegion"]);
        private readonly string _IdentityPoolId = ConfigurationManager.AppSettings["IdentityPoolId"];
        private readonly string _cognitoUserPoolId = ConfigurationManager.AppSettings["CognitoUserPoolId"];
        private readonly string _cognitoAccessKey = ConfigurationManager.AppSettings["CognitoAccessKey"];
        private readonly string _cognitoAccessId = ConfigurationManager.AppSettings["CognitoAccessId"];
        private readonly CognitoUser? _unauthenticatedUser;


        private AmazonCognitoIdentityProviderClient _provider;
        private CognitoUserPool _userPool;
        private AuthFlowResponse? _authenticationResult;

        public CognitoIdentityService(CancellationTokenSource cancellationSource = null)
        {
            _cancellationSource = cancellationSource ?? new CancellationTokenSource();
            _provider = new AmazonCognitoIdentityProviderClient(_cognitoAccessId, _cognitoAccessKey, _AwsRegion);
            _userPool = new CognitoUserPool(_cognitoUserPoolId, _clientId, _provider, _clientSecret);
        }

        public CognitoIdentityService() : this(null)
        {
        }

        public event EventHandler LoggedIn;
        public event EventHandler LoggedOut;

        public Task<bool> AcquireTokenSilentAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAccessTokenForGraphAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAccessTokenForWebApiAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string GetAccountUserName()
            => _userPool.GetUser()?.Username;

        public bool IsAuthorized()
        {
            throw new NotImplementedException();
        }

        public bool IsLoggedIn() => _authenticationResult is not null;

        public async Task<LoginResultType> LoginAsync()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return LoginResultType.NoNetworkAvailable;
            }

            try
            {
                var initiateAuthResponse =
                    await _provider.InitiateAuthAsync(new InitiateAuthRequest(), _cancellationSource.Token);

                return LoginResultType.Success;
            }
            catch (AmazonCognitoIdentityProviderException e)
            {
                Console.WriteLine($"Encountered an error while attempting to login...");
                Console.WriteLine($"\t{e.Message}");

                return e.ErrorCode == "authentication_canceled"
                    ? LoginResultType.CancelledByUser
                    : LoginResultType.UnknownError;
            }
            catch (Exception)
            {
                return LoginResultType.UnknownError;
            }
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
