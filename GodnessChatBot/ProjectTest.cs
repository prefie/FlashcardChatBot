using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GodnessChatBot.Domain;
using GodnessChatBot.Domain.LearningWays;
using NUnit.Framework;

namespace GodnessChatBot
{
    [TestFixture]
    public class ProjectTests
    {
        private readonly Repository repository = new Repository();
        
        [Test]
        public void Repository_CanCreateNewSpreadsheetForUser()
        {
            repository.CreateSpreadsheetForUser("Тестовая таблица");
        
            var newSpreadsheetPacks = repository.GetPacksNames("Тестовая таблица").ToList();
        
            newSpreadsheetPacks.Should().Contain("Тестовая колода");
        }
        
        [Test]
        public void Repository_CanAddNewPackInSpreadsheet()
        {
            var newPack = new Pack("Новая колода");
            repository.AddPack("Тестовая таблица", newPack);
        
            var packsNames = repository.GetPacksNames("Тестовая таблица").ToList();
            packsNames.Count.Should().Be(2);
            packsNames.Should().Contain("Новая колода");
        }
        
        [Test]
        public void Repository_CanAddCardsInPacks()
        {
            var receivedPack = repository.GetPack("Тестовая таблица", "Новая колода");
            var previousPackCount = 0;
            if (receivedPack != null) 
                previousPackCount = receivedPack.Count;
            repository.AddCardInPack("Тестовая таблица", "Новая колода", new Card("face1", "back1"));
            repository.AddCardInPack("Тестовая таблица", "Новая колода", new Card("face2", "back2"));
            receivedPack = repository.GetPack("Тестовая таблица", "Новая колода");
        
            (receivedPack.Count - previousPackCount).Should().Be(2);
        }
        
        [Test]
        public void Repository_CanGetSpreadsheetUrl()
        {
            var url = repository.GetSpreadsheetUrl("Тестовая таблица");

            url.Should().Be("https://docs.google.com/spreadsheets/d/1lRL2405Tzr09s3XlCF_UiZM7IC8wZqp1ZcWnhFKhvBw/edit");
        }

        [Test]
        public void Repository_CanGetPacksNames()
        {
            var packsNames = repository.GetPacksNames("Тестовая таблица").ToHashSet();

            packsNames.Count.Should().Be(2);
            packsNames.Should().Contain("Новая колода");
            packsNames.Should().Contain("Тестовая колода");
        }

        [Test]
        public void Repository_CanUpdateStatistics()
        {
            var pack = repository.GetPack("Тестовая таблица","Тестовая колода");
            
            foreach (var card in pack)
                card.Statistic = -card.Statistic;
            
            repository.UpdateStatisticsPack("Тестовая таблица", pack);
            var packWithNewStatistics = repository.GetPack("Тестовая таблица","Тестовая колода");
            
            for (var i = 0; i < pack.Count; i++)
            {
                pack[i].Should().BeEquivalentTo(packWithNewStatistics[i]);
                pack[i].Statistic.Should().Be(packWithNewStatistics[i].Statistic);
            }
        }

        [Test]
        public void LearningWayByTest_CanLearn()
        {
            var learningWay = new LearningWayByTest();
            var pack = new Pack("Test", new []
            {
                new Card("face1", "back1"), 
                new Card("face2", "back2"),
                new Card("face3", "back3"),
                new Card("face4", "back4")
            });
            
            var firstAttempt = learningWay.Learn(null, pack[0], pack, null);
            firstAttempt.Messages.Should().Be("face1");
            firstAttempt.ReplyOptions.Should().BeEquivalentTo(new List<string> {"back1", "back2", "back3", "back4"});
            
            var secondAttempt = learningWay.Learn(pack[0], pack[1], pack, "back2");
            var messages = secondAttempt.Messages.Split('\n');
            messages[0].Should().Be("Неверно :(");
            messages[1].Should().Be("Правильный ответ: back1");
            messages[3].Should().Be("face2");
        }

        [Test]
        public void LearningWayByTest_CanLearnTwoCardsWithSameBack()
        {
            var learningWay = new LearningWayByTest();
            var pack = new Pack("Test", new []
            {
                new Card("face1", "back"), 
                new Card("face2", "back"),
                new Card("face3", "back3"),
                new Card("face4", "back4")
            });
            var firstAttempt = learningWay.Learn(null, pack[0], pack, null);
            firstAttempt.ReplyOptions.Should().BeEquivalentTo(new List<string> {"back", "back3", "back4"});
        }

        [Test]
        public void LearningWayCheckYourself_CanLearn()
        {
            var learningWay = new LearningWayCheckYourself();
            var pack = new Pack("Test", new []
            {
                new Card("face1", "back1"), 
                new Card("face2", "back2"),
                new Card("face3", "back3"),
                new Card("face4", "back4")
            });
            
            var firstAttempt = learningWay.Learn(null, pack[0], pack, null);
            firstAttempt.ReplyOptions.Should().BeEquivalentTo(new List<string> {"Показать ответ"});
            learningWay.NeedNextCard.Should().BeFalse();
            
            var secondAttempt = learningWay.Learn(pack[0], pack[0], pack, "Показать ответ");
            secondAttempt.Messages.Should().Be("back1");
            secondAttempt.ReplyOptions.Should().BeEquivalentTo(new List<string> {"Помню", "Не помню"});
            learningWay.NeedNextCard.Should().BeTrue();

            var thirdAttempt = learningWay.Learn(pack[0], pack[1], pack, "Помню");
            thirdAttempt.ReplyOptions.Should().BeEquivalentTo(new List<string> {"Показать ответ"});
            learningWay.NeedNextCard.Should().BeFalse();
        }

        [Test]
        public void LearningWayCheckYourself_CanLearnWithIncorrectAnswer()
        {
            var learningWay = new LearningWayCheckYourself();
            var pack = new Pack("Test", new []
            {
                new Card("face1", "back1"), 
                new Card("face2", "back2"),
                new Card("face3", "back3"),
                new Card("face4", "back4")
            });
            
            learningWay.Learn(null, pack[0], pack, null);
            var secondAttempt = learningWay.Learn(pack[0], pack[1], pack, "abracadabra");
            secondAttempt.Messages.Should().BeEquivalentTo("Недопустимый вариант ответа, нажми на одну из кнопок сообщения выше :)");
        }
    }
}