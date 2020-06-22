using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gfile = Google.Apis.Drive.v3.Data.File;

namespace MailingApp.GoogleServices.Drive
{
    public interface IGoogleDriveService
    {
        string CreateFile(string name, MemoryStream stream, string mime = null);
        Task<string> CreateFileAsync(string name, MemoryStream stream = null, string mime = "text/plain");
        string CreateFolder(string name);
        Task<string> CreateFolderAsync(string name);
        void EnsureDriveFolderCreated();
        bool FileExists(string name);
        Task<bool> FileExistsAsync(string name, string orderBy = null, string fields = null, string query = null);
        bool FoldeExists(string name);
        Task<bool> FolderExistsAsync(string name);
        IList<Gfile> GetFilesByName(string name);
    }

    public class GoogleDriveService : IGoogleDriveService
    {
        static string[] Scopes = { DriveService.Scope.DriveFile };
        public const string ApplicationName = "DriveRpiConnection";
        private const string APP_FOLDER = "RpiTestFolder";
        private const string CREDENTIALS_PATH = "./";
        private DriveService _service { get; set; }

        public static string APP_FOLDER_ID { get; set; }

        public GoogleDriveService()
        {
            UserCredential credentials;
            using (var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = CREDENTIALS_PATH;
                credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                                Scopes,
                                "user",
                                CancellationToken.None,
                                new FileDataStore(credPath, true)).Result;
            }

            _service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });
        }

        public void EnsureDriveFolderCreated()
        {
            if (!FoldeExists(APP_FOLDER))
            {
                CreateFolder(APP_FOLDER);
            }
            APP_FOLDER_ID = GetFilesByName(APP_FOLDER).First().Id;
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
                    APP_FOLDER_ID
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

        public bool FileExists(string name)
        {
            return FileExistsAsync(
                name: name,
                orderBy: null,
                fields: "nextPageToken, files(name, id)",
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
    }
}
