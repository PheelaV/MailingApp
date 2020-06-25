using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

namespace MailingApp.GoogleServices.OAuth2
{
    interface IGoogleOauth2Service
    {
        Task<GoogleJsonWebSignature.Payload> GetJwtPayloadAsync(UserCredential credentials);
        Task<UserCredential> GetUserCredentialAsync();
    }
}
