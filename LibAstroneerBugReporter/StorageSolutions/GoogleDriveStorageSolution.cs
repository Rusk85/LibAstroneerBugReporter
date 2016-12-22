using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using LibAstroneerBugReporter.GoogleApiHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using static Google.Apis.Drive.v3.FilesResource;

namespace LibAstroneerBugReporter
{
    public class GoogleDriveStorageSolution : IStorageSolution, IGoogleServiceAuthenticator
    {
        private const string _contentType = @"application/x-compressed";

        private GoogleOAuthHelper _googleOAuthHelper = null;

        public string Store(object resourceHandle)
        {
            if(resourceHandle.GetType() == typeof(FileInfo))
            {
                Authenticate(GoogleService.Drive);
                return uploadZipArchive(resourceHandle as FileInfo);
            }
            return null;
        }

        public GoogleApiServiceConfiguration Authenticate(GoogleService service)
        {
            _googleOAuthHelper = _googleOAuthHelper ?? new GoogleOAuthHelper(service);
            _googleOAuthHelper.Authorize();
            return _googleOAuthHelper.GoogleApiServiceConfiguration;
        }


        private string uploadZipArchive(FileInfo zipArchive)
        {
            var file = new Google.Apis.Drive.v3.Data.File
            {
                Name = zipArchive.Name,
            };

            using (var fStream = new FileStream(zipArchive.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var uploadRequest = new CreateMediaUpload(_googleOAuthHelper.GoogleApiServiceConfiguration.ClientService, file, fStream, _contentType);
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

        private string getDownloadLink(Google.Apis.Drive.v3.Data.File file)
        {

            DriveService service = (DriveService)_googleOAuthHelper.GoogleApiServiceConfiguration.ClientService;

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

    
}
