using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MailingApp.GoogleServices.OAuth2
{
    public class GoogleOauth2Service : IGoogleOauth2Service
    {
        private static readonly string[] Scopes = { DriveService.Scope.DriveFile, GmailService.Scope.GmailSend, "email", "profile", GmailService.Scope.MailGoogleCom };
        public async Task<UserCredential> GetUserCredentialAsync()
        {
            UserCredential credentials;
            using (var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                var secrets = GoogleClientSecrets.Load(stream).Secrets;

                credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync
                    (
                        clientSecrets: secrets,
                        scopes: Scopes,
                        user: "user",
                        taskCancellationToken: CancellationToken.None
                        //new FileDataStore(App.AppSettings["CredentialsPath"], Convert.ToBoolean(App.AppSettings["CredentialsPathIsFull"]))
                    );
            }

            return credentials;
        }

        public async Task<GoogleJsonWebSignature.Payload> GetJwtPayloadAsync(UserCredential credentials)
        {
            try
            {
                return await GoogleJsonWebSignature.ValidateAsync(credentials.Token.IdToken);
            }
            catch (InvalidJwtException ep)
            {
                var refreshed = await credentials.RefreshTokenAsync(CancellationToken.None);
                if (!refreshed)
                {
                    throw ep;
                }
                return await GoogleJsonWebSignature.ValidateAsync(credentials.Token.IdToken);
            }
        }
    }
}
