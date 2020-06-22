namespace MailingApp.GoogleServices.Drive
{
    // Future me, love me, the mime types are well documented here https://developers.google.com/drive/api/v3/mime-types
    public static class DriveMimeType
    {
        public static string GFile => "application/vnd.google-apps.file";
        public static string GFolder => "application/vnd.google-apps.folder";
    }
}
