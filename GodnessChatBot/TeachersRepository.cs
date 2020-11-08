using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace GodnessChatBot
{
    internal class TeachersRepository
    {
        private readonly string client;
        private readonly string[] scopesSheets;
        private readonly string appName;
        private readonly string spreadsheetId;
        private readonly SheetsService service;

        internal TeachersRepository()
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
                var credPath = Path.Combine(Directory.GetCurrentDirectory(), "sheetsCreds");
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopesSheets,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }

        private ValueRange GetValuesSheet(string name)
        {
            try
            {
                var values = service.Spreadsheets.Values.Get(spreadsheetId, name).Execute();
                return values;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private List<Category> GetCategories(string userName)
        {
            var table = GetValuesSheet(userName)?.Values;
            var categories = new Dictionary<string, Category>();
            for (var i = 0; i < table.Count; i+=3)
            {
                var name = table[i][0].ToString();
                var category = new Category(name);
                if (!categories.ContainsKey(name))
                    categories.Add(name, category);
                var namePack = table[i + 1][0].ToString();
                var cards = new List<Card>();
                for (var j = 1; j < table[i].Count; j++)
                {
                    cards.Add(new Card(table[i][j].ToString(), table[i+1][j].ToString()));
                }
                categories[name].AddPack(new Pack(namePack, cards, new List<LearningWay> {LearningWay.LearnYourself}, false));
            }

            return categories.Values.ToList();
        }

        public Teacher GetTeacher(string userName)
        {
            var categories = GetCategories(userName);
            return new Teacher(categories);
        }
    }
}