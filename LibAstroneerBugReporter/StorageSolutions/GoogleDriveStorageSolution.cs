using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.Drive.v3.FilesResource;

namespace LibAstroneerBugReporter
{
    public class GoogleDriveStorageSolution : IStorageSolution
    {
        private OAuthAccessToken _accessToken;
        private const string _DriveApiEndpoint = @"https://www.googleapis.com/upload/drive/v3/files";
        private string[] _scope = new string[] { @"https://www.googleapis.com/auth/drive.file" };

        private string _credentialsStorage = Path.Combine(Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal), ".credentials", "drive-astroneer-bug-reports.json");

        private string _contentType = @"application/x-compressed";
        private UserCredential _token = null;

        public GoogleDriveStorageSolution()
        {
            _accessToken = JsonConvert.DeserializeObject<OAuthAccessToken>(AccessToken.JsonString);
        }

        public string Store(object resourceHandle)
        {
            if(resourceHandle.GetType() == typeof(FileInfo))
            {
                return uploadZipArchive(resourceHandle as FileInfo);
            }
            return null;
        }

        private string uploadZipArchive(FileInfo zipArchive)
        {
            using (var memStream = new MemoryStream(Encoding.ASCII.GetBytes(AccessToken.JsonString)))
            {
                var secrets = GoogleClientSecrets.Load(memStream).Secrets;

                _token = _token ?? GoogleWebAuthorizationBroker.AuthorizeAsync(secrets,
                    _scope,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(_credentialsStorage, true)).Result;

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _token,
                    ApplicationName = _accessToken.installed.project_id
                });

                var file = new Google.Apis.Drive.v3.Data.File
                {
                    Name = zipArchive.Name,
                };

                using (var fStream = new FileStream(zipArchive.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var uploadRequest = new CreateMediaUpload(service, file, fStream, _contentType);
                    var response = uploadRequest.Upload();
                    if (response.Status == Google.Apis.Upload.UploadStatus.Completed)
                    {
                        return getDownloadLink(uploadRequest.ResponseBody);
                    }
                    else
                    {
                        return response.Exception.Message;
                    }
                }
            }
        }

        private string getDownloadLink(Google.Apis.Drive.v3.Data.File file)
        {
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = _token,
                ApplicationName = _accessToken.installed.project_id
            });


            var batch = new BatchRequest(service);
            string returnValue = null;
            Permission returnPermission = null;
            BatchRequest.OnResponse<Permission> permissionCallback = delegate
            (
                Permission permission,
                RequestError error,
                int index,
                HttpResponseMessage message
            )
            {
                if (error != null)
                {
                    returnValue = error.Message;
                    return;
                }
                Console.WriteLine(permission.Id);
                returnPermission = permission;
                if (!String.IsNullOrEmpty(file.WebContentLink))
                {
                    returnValue = file.WebContentLink;
                }
                else
                {
                    var metadataRequest = new GetRequest(service, file.Id);
                    metadataRequest.Fields = "webContentLink";
                    var newFile = metadataRequest.Execute();
                    returnValue = newFile.WebContentLink;
                }
            };


            var downloadPermission = new Permission
            {
                Type = "anyone",
                Role = "reader",
            };

            var dlPermissionRequest = service.Permissions.Create(downloadPermission, file.Id);
            batch.Queue(dlPermissionRequest, permissionCallback);
            var task = batch.ExecuteAsync();
            try
            {
                task.Wait();
            }
            catch (Exception) { }
            return returnValue;
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
}
