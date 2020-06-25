using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gfile = Google.Apis.Drive.v3.Data.File;

namespace MailingApp.GoogleServices.Drive
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private DriveService _service { get; set; }

        private static string AppFolderID { get; set; }

        public GoogleDriveService(UserCredential credentials)
        {
           _service =  new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = App.AppSettings["AppnName"]
            });
        }

        public void EnsureDriveFolderCreated()
        {
            if (!FoldeExists(App.AppSettings["AppnDriveFolder"]))
            {
                CreateFolder(App.AppSettings["AppnDriveFolder"]);
            }
            AppFolderID = GetFilesByName(App.AppSettings["AppnDriveFolder"]).First().Id;
        }

        public string CreateFile(string name, MemoryStream stream, string mime = null)
        {
            return CreateFileAsync(name, stream, mime).Result;
        }
        public async Task<string> CreateFileAsync(string name, MemoryStream stream = null, string mime = "text/plain")
        {
            var file = new Gfile()
            {
                Name = name,
                MimeType = mime,
                CreatedTime = DateTime.Now,
                Parents = new List<string>{
                    AppFolderID
                }

            };
            var response = await _service.Files.Create(file, stream, mime).UploadAsync();

            switch (response.Status)
            {
                case Google.Apis.Upload.UploadStatus.Failed:
                    throw new Exception("Upload failed");

                case Google.Apis.Upload.UploadStatus.Completed:
                    return GetFilesByName(name).First().Id;
            }
            return null;
        }

        public IList<Gfile> GetFilesByName(string name)
        {
            var request = _service.Files.List();
            request.Q = $"name = '{name}'";
            return request.Execute().Files;
        }

        //TODO: Destroy the synchronous version
        public bool FileExists(string name)
        {
            return FileExistsAsync(
                name: name,
                orderBy: null,
                fields: "nextPageToken, files(name, id)",
                //TODO: Replace string with enums => create a google export/import mimeType library
                query: "mimeType = 'application/vnd.google-apps.file'").Result;
        }

        public async Task<bool> FileExistsAsync(string name, string orderBy = null, string fields = null, string query = null)
        {
            bool result;
            string pageToken = null;

            do
            {
                var listing = _service.Files.List();
                listing.PageToken = pageToken;
                listing.OrderBy = orderBy;
                listing.Fields = fields;
                listing.Q = query;
                var response = await listing.ExecuteAsync();

                result = response.Files.Any(x => x.Name == name);

                if (result)
                {
                    return result;
                }
            } while (pageToken != null);

            return result;
        }

        public string CreateFolder(string name)
        {
            return CreateFolderAsync(name).Result;
        }
        public async Task<string> CreateFolderAsync(string name)
        {
            var file = new Gfile()
            {
                Name = name,
                MimeType = DriveMimeType.GFolder,
                CreatedTime = DateTime.Now,

            };
            var response = await _service.Files.Create(file).ExecuteAsync();
            return response.Id;
        }

        public bool FoldeExists(string name)
        {
            return FolderExistsAsync(name).Result;
        }
        public async Task<bool> FolderExistsAsync(string name)
        {
            return await FileExistsAsync(name, "folder");
        }

        public AboutResource About => _service.About;
    }
}
