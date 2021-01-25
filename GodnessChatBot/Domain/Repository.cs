using System;
using System.Collections.Generic;
using System.Linq;
using GodnessChatBot.Infrastructure;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace GodnessChatBot.Domain
{
    public class Repository : GoogleSpreadsheetsHandler, IRepository
    {
        private readonly string usersSpreadsheetId;
        private readonly string usersSheetTitle;

        public Repository() : base("GodnessChatBot", 
            "credentials.json", 
            new []{DriveService.Scope.Drive, SheetsService.Scope.Spreadsheets})
        {
            usersSpreadsheetId = "1In99pe6lf7W57OT_FWABz8fzpl8T2nbcjcZCLIJAe0s";
            usersSheetTitle = "Users";
        }

        public Pack GetPack(string userId, string packName)
        {
            var spreadsheetId = GetSpreadsheetId(userId);
            var table = GetValuesSheet(spreadsheetId, packName)?.Values;
            
            var pack = new Pack(packName);

            if (table == null || table.Count < 2) return null;
            for (var i = 1; i < table.Count; i++)
            {
                if (table[i].Count != 3) continue;
                try
                {
                    var statistics = int.Parse(table[i][2].ToString());
                    pack.AddCard(
                        new Card(table[i][0].ToString(), table[i][1].ToString(), statistics));
                }
                catch (FormatException)
                {
                    return null;
                }
            }

            return pack;
        }

        public void UpdateStatisticsPack(string userId, Pack pack)
        {
            var spreadsheetId = GetSpreadsheetId(userId);
            var cards = pack.Cards.Distinct().ToDictionary(card => (card.Face, card.Back));

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
            var spreadsheet = GetSpreadsheet(spreadsheetId);
            return spreadsheet?.Sheets.Select(x => x.Properties.Title);
        }

        public void CreateSpreadsheetForUser(string userId)
        {
            if (GetSpreadsheetId(userId) != null)
                return;

            const string baseSheet = "Тестовая колода";
            var values = GetValuesSheet(usersSpreadsheetId, baseSheet);

            var firstSheet = new Sheet {Properties = new SheetProperties {Title = baseSheet}};
            
            var spreadsheet = CreateSpreadsheet(userId, new []{firstSheet});
            
            SetPermissions(spreadsheet.SpreadsheetId, "anyone", "writer");

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
                if (row.Length < 3) return null;
                if (row[0] == userId)
                    return row[2];
            }

            return null;
        }

        public void AddPack(string userId, Pack pack) => CreateNewPack(userId, pack.Name);

        public void AddCardInPack(string userId, string packName, Card card)
        {
            IList<object> data = new object[] {card.Face, card.Back, card.Statistic.ToString()};
            var spreadsheetId = GetSpreadsheetId(userId);
            AddDataToEndOfSheet(spreadsheetId, packName, data);
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
                
                if (row.Length < 3) return null;
                if (row[0] == userId)
                    return row[1];
            }

            return null;
        }

        private void CreateNewPack(string userId, string packName)
        {
            try
            {
                var spreadsheetId = GetSpreadsheetId(userId);
                CreateNewSheet(spreadsheetId, packName);
                AddDataToEndOfSheet(spreadsheetId, packName, new object[]{"FACE", "BACK", "STATISTICS"});
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}