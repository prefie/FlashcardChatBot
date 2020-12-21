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
            var b = TryAddPack(0, "САМУНЬ", "САМЫЙ КРУТОЙ");
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
        
        private void CreateNewSheet(int idUser)
        {
            var addSheetRequest = new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = idUser.ToString(),
                        SheetId = idUser
                    }
                }
            };

            var request = new List<Request> {addSheetRequest};
            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest {Requests = request};
            try
            {
                service.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId).Execute();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private bool IsPackExist(int idUser, string nameCategory, string namePack, out int indexPack)
        {
            var table = GetValuesSheet(idUser)?.Values;
            for (var i = 0; i < table.Count; i+=3)
            {
                if (table[i][0].ToString() == nameCategory && table[i + 1][0].ToString() == namePack)
                {
                    indexPack = i;
                    return true;
                }
            }

            indexPack = -1;
            return false;
        }

        private bool TryAddPack(int idUser, string nameCategory, string namePack)
        {
            if (IsPackExist(idUser, nameCategory, namePack, out var indexPack))
                return false;
            
            var table = GetValuesSheet(idUser)?.Values;
            
            var requests = CreateRequests(idUser, new[] {nameCategory, namePack}, table.Count + 1, 0);
            ExecuteRequests(requests);
            
            return true;
        }

        private bool TryAddCard(int idUser,string nameCategory, string namePack, Card card)
        {
            if (!IsPackExist(idUser, nameCategory, namePack, out var indexPack))
                return false;
            
            var table = GetValuesSheet(idUser)?.Values;
            
            var requests = CreateRequests(idUser, new[] {card.Face, card.Back}, indexPack, table[indexPack].Count);
            ExecuteRequests(requests);

            return true;
        }
        
        private static List<Request> CreateRequests(int idUser, IReadOnlyList<string> data, int startRowIndex, int columnIndex)
        {
            var request = new List<Request>();
            for (var i = 0; i < data.Count; i++)
            {
                var values = new List<CellData>();
                values.Add(new CellData
                {
                    UserEnteredValue = new ExtendedValue
                    {
                        StringValue = data[i]
                    }
                });
                
                request.Add(
                    new Request
                    {
                        UpdateCells = new UpdateCellsRequest
                        {
                            Start = new GridCoordinate
                            {
                                SheetId = idUser,
                                RowIndex = startRowIndex + i,
                                ColumnIndex = columnIndex
                            },
                            Rows = new List<RowData>{new RowData {Values =values}},
                            Fields = "userEnteredValue"
                        }
                    }
                );
            }

            return request;
        }

        private void ExecuteRequests(IList<Request> requests)
        {
            var busr = new BatchUpdateSpreadsheetRequest {Requests = requests};
            service.Spreadsheets.BatchUpdate(busr, spreadsheetId).Execute();
        }

        private ValueRange GetValuesSheet(int userId)
        {
            try
            {
                var values = service.Spreadsheets.Values.Get(spreadsheetId, userId.ToString()).Execute();
                return values;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // private List<Category> GetCategories(int idUser)
        // {
        //     var table = GetValuesSheet(idUser)?.Values;
        //     var categories = new Dictionary<string, Category>();
        //     for (var i = 0; i < table.Count; i+=3)
        //     {
        //         var name = table[i][0].ToString();
        //         if (!categories.ContainsKey(name))
        //         {
        //             var category = new Category(name);
        //             categories.Add(name, category);
        //         }
        //
        //         var namePack = table[i + 1][0].ToString();
        //         var cards = new List<Card>();
        //         for (var j = 1; j < table[i].Count; j++)
        //         {
        //             cards.Add(new Card(table[i][j].ToString(), table[i+1][j].ToString()));
        //         }
        //         categories[name].AddPack(new Pack(namePack, cards, new List<LearningWay> {LearningWay.LearnYourself}, false));
        //     }
        //
        //     return categories.Values.ToList();
        // }

        public Pack GetPack(int idUser)
        {
            //return pack;
            throw new Exception();
        }
    }
}