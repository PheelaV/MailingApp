using EASendMail;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MailingApp.GoogleServices.Drive;
using MailingApp.GoogleServices.Gmail;
using MailingApp.GoogleServices.OAuth2;
using MailingApp.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.ApplicationInsights;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MailingApp
{
    class Program
    {
        private const string CREDENTIALS_PATH = "./";
        private GmailService gmailService { get; set; }

        //private static string ApplicationName = App.Configuration.getse;
        static async Task Main(string[] args)
        {
            #region Setup
            App.Configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddCommandLine(args)
              .Build();

            // Logging
            var configuration = TelemetryConfiguration.CreateDefault();
            configuration.TelemetryProcessorChainBuilder
                .Use(tp => new ApplicationInsightsOfflineFilter(tp))
                .Build();
            var telemetryClient = new TelemetryClient(configuration);
            #endregion


            var googleOauth = new GoogleOauth2Service();

            var credentials = await googleOauth.GetUserCredentialAsync();

            GoogleJsonWebSignature.Payload jwtPayload = await googleOauth.GetJwtPayloadAsync(credentials);

            var username = jwtPayload.Email;




            var driveService = new GoogleDriveService(credentials);

            var aboutRequest = driveService.About.Get();
            aboutRequest.Fields = "*";

            var response = await aboutRequest.ExecuteAsync();
            Console.WriteLine("Import formats");
            response.ExportFormats.ToList().ForEach(x => Console.WriteLine($"{x.Key} {x.Value.FirstOrDefault()}"));
            Console.WriteLine("Export formats");
            response.ImportFormats.ToList().ForEach(x => Console.WriteLine($"{x.Key} {x.Value.FirstOrDefault()}"));

            MailService.SendMailWithXOAUTH2(username, credentials.Token.AccessToken);
            Console.WriteLine("Finished :)");

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
