using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using MailingApp.GoogleServices.Drive;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;

namespace MailingApp
{
    class Program
    {
        private const string CREDENTIALS_PATH = "./";
        private DriveService driveService { get; set; }
        private GmailService gmailService { get; set; }
        static string[] Scopes = { DriveService.Scope.DriveFile, GmailService.Scope.GmailSend };
        static void Main(string[] args)
        {
            //var services = ConfigureServices();
            //var serviceProvider = services.BuildServiceProvider();
            //serviceProvider.GetService<ConsoleApplication>().Run();
            //var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            //new ClientSecrets
            //{
            //    ClientId = "put your client id here",
            //    ClientSecret = "put your client secret here"
            //},
            //new[] { "email", "profile", "https://mail.google.com/" },
            //"user",
            //CancellationToken.None
            //);

            //    var jwtPayload = GoogleJsonWebSignature.ValidateAsync(credential.Token.IdToken).Result;
            //    var username = jwtPayload.Email;

            UserCredential credentials;
            using (var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = CREDENTIALS_PATH;
                credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets: GoogleClientSecrets.Load(stream).Secrets,
                    scopes: Scopes,
                    user : "user",
                    taskCancellationToken: CancellationToken.None,
                    dataStore: new FileDataStore(credPath, true)).Result;
            }

            var jwtPayload = GoogleJsonWebSignature.ValidateAsync(credentials.Token.IdToken).Result;
            var username = jwtPayload.Email;
        }

        //private static IServiceCollection ConfigureServices()
        //{
        //    IServiceCollection services = new ServiceCollection();

        //    services.AddSingleton<IGoogleDriveService, GoogleDriveService>();
        //    services.AddTransient<ConsoleApplication>();
        //    return services;
        //}
    }
}
