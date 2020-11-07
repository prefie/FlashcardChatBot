using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace GodnessChatBot
{
    internal class UsersRepository
    {
        private readonly string client;
        private readonly string[] scopesSheets;
        private readonly string appName;
        private readonly string spreadsheetId;
        private readonly SheetsService service;

        internal UsersRepository()
        {
            client = "credentials.json";
            scopesSheets = new []{SheetsService.Scope.Spreadsheets};
            appName = "GodnessChatBot";
            spreadsheetId = "1RFIyiWWuF2wgZtd8iErUgGZGQQU04H6njiqLvbh-050";
            
            var credential = GetSheetCredentials();
            service = GetService(credential);
        }
        
        private SheetsService GetService(UserCredential credential)
        {
            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = appName
            });
        }

        private UserCredential GetSheetCredentials()
        {
            using (var stream = new FileStream(client, FileMode.Open, FileAccess.Read))
            {
                var credPath = Path.Combine(Directory.GetCurrentDirectory(), "sheetsCreds.json");
                var secrets = GoogleClientSecrets.Load(stream);
                var a = secrets.Secrets;
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    a,
                    scopesSheets,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }
    }
}