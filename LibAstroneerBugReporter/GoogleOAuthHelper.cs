using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace LibAstroneerBugReporter.GoogleApiHelper
{
    public interface IGoogleServiceAuthenticator
    {
        GoogleApiServiceConfiguration Authenticate(GoogleService service);
    }

    public enum GoogleService
    {
        Drive = 0,
        UrlShortener = 1
    }

    internal static class ApiStrings
    {
        public const string User = "user";
        public const string Credentials = ".credentials";
        public const string DriveSecret = "drive-astroneer-bug-reports.json";
        public const string UrlShortenerSecret = "shortener-astroneer-bug-reports.json";

        public static string CredentialStorageBaseDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal), Credentials);
            }
        }
    }

    public class Installed
    {
        public string client_id { get; set; }
        public string project_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_secret { get; set; }
        public List<string> redirect_uris { get; set; }
    }

    public class OAuthAccessToken
    {
        public Installed installed { get; set; }
    }

    public class GoogleApiServiceConfiguration
    {
        private GoogleService _service;

        private void initializeConfiguration()
        {
            string baseDir = ApiStrings.CredentialStorageBaseDirectory;
            Func<string, string> toStr =
                s => Path.Combine(baseDir, s);
            string credStore = null;

            Func<string, string[]> toSrvStr =
                s => new string[] { s };
            string[] srvScopes = null;

            switch (_service)
            {
                case GoogleService.Drive:
                    credStore = toStr(ApiStrings.DriveSecret);
                    srvScopes = toSrvStr(DriveService.Scope.DriveFile);
                    break;

                case GoogleService.UrlShortener:
                    credStore = toStr(ApiStrings.UrlShortenerSecret);
                    srvScopes = toSrvStr(UrlshortenerService.Scope.Urlshortener);
                    break;
            }

            _credentialStorageDirectory = credStore;
            _googleServiceScopes = srvScopes;
            _oAuthAccessToken = JsonConvert.DeserializeObject
                <OAuthAccessToken>(AccessToken.JsonString);
        }

        private void initializeClientService()
        {
            Func<BaseClientService.Initializer> init = () =>
            new BaseClientService.Initializer
            {
                HttpClientInitializer = _token,
                ApplicationName = _oAuthAccessToken.installed.project_id
            };
            BaseClientService clientSrv = null;
            switch (_service)
            {
                case GoogleService.Drive:
                    clientSrv = new DriveService(init());
                    break;

                case GoogleService.UrlShortener:
                    clientSrv = new UrlshortenerService(init());
                    break;
            }
            _clientService = clientSrv;
        }

        public GoogleApiServiceConfiguration(GoogleService service)
        {
            _service = service;
            initializeConfiguration();
        }

        private OAuthAccessToken _oAuthAccessToken = null;

        public OAuthAccessToken OAuthAccessToken
        {
            get
            {
                return _oAuthAccessToken;
            }
        }

        private string _credentialStorageDirectory;

        public string CredentialStorageDirectory
        {
            get
            {
                return _credentialStorageDirectory;
            }
        }

        private string[] _googleServiceScopes;

        public string[] GoogleServiceScopes
        {
            get
            {
                return _googleServiceScopes;
            }
        }

        private UserCredential _token;

        public UserCredential Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
                if (_clientService == null)
                {
                    initializeClientService();
                }
            }
        }

        private BaseClientService _clientService;

        public BaseClientService ClientService
        {
            get
            {
                return _clientService;
            }
        }

        public string User
        {
            get
            {
                return ApiStrings.User;
            }
        }

        public string Credentials
        {
            get
            {
                return ApiStrings.Credentials;
            }
        }
    }

    public class GoogleOAuthHelper
    {
        private GoogleService _googleService;

        private Dictionary<GoogleService, GoogleApiServiceConfiguration> _serviceConfigurations = null;

        public GoogleApiServiceConfiguration GoogleApiServiceConfiguration
        {
            get
            {
                return _serviceConfigurations[_googleService];
            }
        }

        public GoogleOAuthHelper(GoogleService googleService)
        {
            _googleService = googleService;
            _serviceConfigurations = new Dictionary<GoogleService, GoogleApiServiceConfiguration>();
            _serviceConfigurations.Add(_googleService, new GoogleApiServiceConfiguration(_googleService));
        }

        public GoogleApiServiceConfiguration Authorize()
        {
            var serviceConfig = _serviceConfigurations[_googleService];

            if (serviceConfig.Token != null)
            {
                return serviceConfig;
            }

            using (var mStream = new MemoryStream(Encoding.ASCII.GetBytes(AccessToken.JsonString)))
            {
                var token = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(mStream).Secrets,
                    serviceConfig.GoogleServiceScopes,
                    serviceConfig.User,
                    CancellationToken.None,
                    new FileDataStore(serviceConfig.CredentialStorageDirectory, true))
                    .Result;
                serviceConfig.Token = token;
            }
            return serviceConfig;
        }
    }
}