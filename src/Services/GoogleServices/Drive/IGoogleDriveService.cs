using Google.Apis.Drive.v3;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gfile = Google.Apis.Drive.v3.Data.File;

namespace MailingApp.GoogleServices.Drive
{
    interface IGoogleDriveService
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
            AboutResource About();
        }
    }
}
