using System.Linq;
using FluentAssertions;
using GodnessChatBot.Domain;
using NUnit.Framework;

namespace GodnessChatBot
{
    public class Tests
    {
        [Test]
        public void Pack_CanAddCardsInPack()
        {
            var newPack = new Pack("Тестовая колода");
            newPack.Cards.Count.Should().Be(0);

            newPack.AddCard(new Card("face1", "back1"));
            newPack.AddCard(new Card("face2", "back2"));
            newPack.AddCard(new Card("face3", "back3"));
            newPack.Cards.Count.Should().Be(3);
        }

        [Test]
        public void Pack_CanRemoveCardsFromPack()
        {
            var newPack = new Pack("Тестовая колода");

            newPack.AddCard(new Card("face1", "back1"));
            newPack.AddCard(new Card("face2", "back2"));
            newPack.AddCard(new Card("face3", "back3"));
            newPack.Cards.Count.Should().Be(3);

            newPack.RemoveCard(new Card("face1", "back1")).Should().Be(true);
            newPack.Cards.Count.Should().Be(2);
        }

        [Test]
        public void Repository_CanCreateNewSpreadsheet()
        {
            Repository.CreateSpreadsheet("Тестовая таблица");

            var newSpreadsheetPacks = Repository.GetPacksNames("Тестовая таблица").ToList();

            newSpreadsheetPacks.Should().Contain("Тестовая колода");
        }

        [Test]
        public void Repository_CanAddNewPackInSpreadsheet()
        {
            var newPack = new Pack("Новая колода");
            Repository.AddPack("Тестовая таблица", newPack);

            var packsNames = Repository.GetPacksNames("Тестовая таблица").ToList();
            packsNames.Count.Should().Be(2);
            packsNames.Should().Contain("Новая колода");
        }

        [Test]
        public void Repository_CanAddCardsInPacks()
        {
            var receivedPack = Repository.GetPack("Тестовая таблица", "Новая колода");
            var previousPackCount = receivedPack.Cards.Count;
            Repository.AddCardInPack("Тестовая таблица", "Новая колода", new Card("face1", "back1"));
            Repository.AddCardInPack("Тестовая таблица", "Новая колода", new Card("face2", "back2"));
            receivedPack = Repository.GetPack("Тестовая таблица", "Новая колода");

            (receivedPack.Cards.Count - previousPackCount).Should().Be(2);
        }

        [Test]
        public void Repository_CanRemoveCardFromPack()
        {
            var receivedPack = Repository.GetPack("Тестовая таблица", "Новая колода");
            var previousPackCount = receivedPack.Cards.Count;
            Repository.RemoveCardFromPack("Тестовая таблица", "Новая колода", new Card("face1", "back1"));
            receivedPack = Repository.GetPack("Тестовая таблица", "Новая колода");
            
            (previousPackCount - receivedPack.Cards.Count).Should().Be(1);
        }
    }
}