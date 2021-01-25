using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace GodnessChatBot.Infrastructure
{
    public class GoogleSpreadsheetsHandler
    {
        private readonly SheetsService sheetsService;
        private readonly DriveService driveService;

        protected GoogleSpreadsheetsHandler(string appName, string client, string[] scopes)
        {
            var credential = GetSheetCredentials(client, scopes);
            
            sheetsService = GetSheetsService(credential, appName);
            driveService = GetDriveService(credential, appName);
        }

        protected Spreadsheet GetSpreadsheet(string spreadsheetId) =>
            sheetsService.Spreadsheets.Get(spreadsheetId).Execute();

        protected Spreadsheet CreateSpreadsheet(string spreadsheetName, Sheet[] sheets) =>
            sheetsService.Spreadsheets.Create(new Spreadsheet
            {
                Properties = new SpreadsheetProperties {Title = spreadsheetName},
                Sheets = sheets
            }).Execute();

        protected void SetPermissions(string spreadsheetId, string type, string role)
        {
            driveService.Permissions
                .Create(new Permission {Type = type, Role = role}, spreadsheetId).Execute();
        }
        
        protected void CreateNewSheet(string spreadsheetId, string sheetName)
        {
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
            sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId).Execute();
        }

        protected void UpdateValuesTable(ValueRange values, string spreadsheetId, string range)
        {
            var request = sheetsService.Spreadsheets.Values.Update(values, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            request.Execute();
        }

        private SheetsService GetSheetsService(UserCredential credential, string appName)
        {
            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = appName
            });
        }

        private DriveService GetDriveService(UserCredential credential, string appName)
        {
            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = appName
            });
        }

        private UserCredential GetSheetCredentials(string client, string[] scopes)
        {
            using (var stream = new FileStream(client, FileMode.Open, FileAccess.Read))
            {
                var credPath = Path.Combine(Directory.GetCurrentDirectory(), "sheetsCreds");
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }
        
        protected void AddDataToEndOfSheet(string spreadsheetId, string range, IList<object> data)
        {
            var table = GetValuesSheet(spreadsheetId, range);
            if (table.Values == null)
                table.Values = new List<IList<object>>{data};
            else
                table.Values.Add(data);

            UpdateValuesTable(table, spreadsheetId, table.Range);
        }
        
        protected ValueRange GetValuesSheet(string spreadsheetId, string range)
        {
            try
            {
                var values = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range).Execute();
                return values;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}