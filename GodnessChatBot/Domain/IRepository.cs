using System.Collections.Generic;

namespace GodnessChatBot.Domain
{
    public interface IRepository
    {
        Pack GetPack(string userId, string packName);
        void AddPack(string userId, Pack pack);
        void UpdateStatisticsPack(string userId, Pack pack);
        IEnumerable<string> GetPacksNames(string userId);
        void CreateSpreadsheetForUser(string userId);
        string GetSpreadsheetUrl(string userId);
        void AddCardInPack(string userId, string packName, Card card);
    }
}