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
        private readonly string usersSpreadsheetId;
        private readonly string usersSheetTitle;
        private readonly SheetsService service;

        internal TeachersRepository()
        {
            client = "credentials.json";
            scopesSheets = new []{SheetsService.Scope.Spreadsheets};
            appName = "GodnessChatBot";
            usersSpreadsheetId = "1In99pe6lf7W57OT_FWABz8fzpl8T2nbcjcZCLIJAe0s";
            usersSheetTitle = "Users";
            
            var credential = GetSheetCredentials();
            service = GetService(credential);
        }

        public Pack GetPack(string userId, string packName)
        {
            var spreadsheetId = GetSpreadsheetId(userId);
            var table = GetValuesSheet(spreadsheetId, packName)?.Values;
            
            var pack = new Pack(packName, false);

            if (table == null) return pack;
            for (var i = 1; i < table.Count; i++)
            {
                if (table[i].Count != 3) continue;
                pack.AddCard(
                    new Card(table[i][0].ToString(), table[i][1].ToString(), int.Parse(table[i][2].ToString())));
            }

            return pack;
        }

        public void UpdateStatisticsPack(string userId, Pack pack)
        {
            var spreadsheetId = GetSpreadsheetId(userId);
            var cards = pack.Cards.ToDictionary(card => (card.Face, card.Back));
            
            // В предположении, что новых карт нет.

            var table = GetValuesSheet(spreadsheetId, pack.Name);
            var values = table.Values;
            for (var i = 1; i < values.Count; i++)
            {
                var face = values[i][0].ToString();
                var back = values[i][1].ToString();
                if (!cards.ContainsKey((face, back)))
                    continue;
                
                values[i][2] = cards[(face, back)].Statistic;
            }
            
            UpdateValuesTable(table, spreadsheetId, table.Range);
        }

        public IEnumerable<string> GetPacksNames(string userId)
        {
            var spreadsheetId = GetSpreadsheetId(userId);
            var spreadsheet = service.Spreadsheets.Get(spreadsheetId).Execute();
            return spreadsheet?.Sheets.Select(x => x.Properties.Title);
        }

        public void CreateSpreadsheet(string userId)
        {
            if (GetSpreadsheetId(userId) != null)
                return;

            const string baseSheet = "Тестовая колода";
            var values = GetValuesSheet(usersSpreadsheetId, baseSheet);

            var firstSheet = new Sheet {Properties = new SheetProperties {Title = baseSheet}};

            var spreadsheet = service.Spreadsheets.Create(new Spreadsheet
            {
                Properties = new SpreadsheetProperties {Title = userId},
                Sheets = new List<Sheet> {firstSheet}
            }).Execute();

            UpdateValuesTable(values, spreadsheet.SpreadsheetId, values.Range);
            AddUserInSpreadsheet(userId, spreadsheet.SpreadsheetId, spreadsheet.SpreadsheetUrl);
        }

        public string GetSpreadsheetUrl(string userId)
        {
            var table = GetValuesSheet(usersSpreadsheetId, usersSheetTitle)?.Values;

            if (table == null) return null;
            for (var i = 1; i < table.Count; i++)
            {
                var row = table[i].Select(x => x.ToString()).ToArray();
                if (row[0] == userId)
                    return row[2];
            }

            return null;
        }

        public void AddPack(string userId, Pack pack)
        {
            var spreadsheetId = GetSpreadsheetId(userId);
            
            CreateNewSheet(userId, pack.Name);
            AddDataToEndOfSheet(spreadsheetId, pack.Name, new object[]{"FACE", "BACK", "STATISTICS"});
        }

        public void AddCardInPack(string userId, string packName, Card card)
        {
            IList<object> data = new object[] {card.Face, card.Back, card.Statistic.ToString()};
            var spreadsheetId = GetSpreadsheetId(userId);
            AddDataToEndOfSheet(spreadsheetId, packName, data);
        }

        public void RemoveCardFromPack(string userId, string packName, Card card)
        {
            var spreadsheetId = GetSpreadsheetId(userId);
            var table = GetValuesSheet(spreadsheetId, packName);
            var values = table.Values;

            for (var i = 1; i < values.Count; i++)
            {
                if (values[i].Count < 3) continue;
                var face = values[i][0].ToString();
                var back = values[i][1].ToString();
                if (face == card.Face && back == card.Back)
                {
                    values[i] = new List<object>{"", "", ""}; // Иначе чистить всю таблицу
                    break;
                }
            }
            
            UpdateValuesTable(table, spreadsheetId, table.Range);
        }

        private void AddUserInSpreadsheet(string userId, string spreadsheetId, string url) =>
            AddDataToEndOfSheet(usersSpreadsheetId, usersSheetTitle, new object[] {userId, spreadsheetId, url});
        
        private string GetSpreadsheetId(string userId)
        {
            var table = GetValuesSheet(usersSpreadsheetId, usersSheetTitle)?.Values;

            if (table == null) return null;
            for (var i = 1; i < table.Count; i++)
            {
                var row = table[i].Select(x => x.ToString()).ToArray();
                if (row[0] == userId)
                    return row[1];
            }

            return null;
        }
        
        private void UpdateValuesTable(ValueRange values, string spreadsheetId, string range)
        {
            var request = service.Spreadsheets.Values.Update(values, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            request.Execute();
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
        
        private void AddDataToEndOfSheet(string spreadsheetId, string sheetName, IList<object> data)
        {
            var table = GetValuesSheet(spreadsheetId, sheetName);
            if (table.Values == null)
                table.Values = new List<IList<object>>{data};
            else
                table.Values.Add(data);

            UpdateValuesTable(table, spreadsheetId, table.Range);
        }

        private void CreateNewSheet(string userId, string sheetName)
        {
            var spreadsheetId = GetSpreadsheetId(userId);
            
            var addSheetRequest = new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = sheetName
                    }
                }
            };
        
            var request = new List<Request> {addSheetRequest};
            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest {Requests = request};
            try
            {
                service.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId).Execute();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private ValueRange GetValuesSheet(string spreadsheetId, string sheetName)
        {
            try
            {
                var values = service.Spreadsheets.Values.Get(spreadsheetId, sheetName).Execute();
                return values;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}