using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Urlshortener.v1.Data;
using Google.Apis.Services;
using LibAstroneerBugReporter.GoogleApiHelper;
using static Google.Apis.Urlshortener.v1.UrlResource;

namespace LibAstroneerBugReporter.UrlShortener
{
    public class GoogleUrlShortener : IUrlShortener, IGoogleServiceAuthenticator
    {
        private GoogleOAuthHelper _googleOAuthHelper = null;
        private GoogleApiServiceConfiguration _googleApiServiceConfiguration = null;
        private bool _requireAuthorization = false;

        public GoogleUrlShortener(bool requireAuthorization = false)
        {
            _requireAuthorization = requireAuthorization;
        }

        public GoogleApiServiceConfiguration Authenticate(GoogleService service)
        {
            _googleOAuthHelper = new GoogleOAuthHelper(service);
            if (_requireAuthorization)
            {
                _googleApiServiceConfiguration = _googleOAuthHelper.Authorize();
            }
            return _googleApiServiceConfiguration 
                ?? (_googleApiServiceConfiguration = 
                _googleOAuthHelper.GoogleApiServiceConfiguration);
        }

        public string Shorten(string longDownloadLink)
        {
            Authenticate(GoogleService.UrlShortener);
            var insRequest = new InsertRequest(_googleApiServiceConfiguration.ClientService, new Url
            {
                LongUrl = longDownloadLink
            });

            var result = insRequest.Execute();

            return !String.IsNullOrEmpty(result.Id)
                ? result.Id
                : result.Status;
        }
    }
}
