using System.Collections.Generic;

namespace GodnessChatBot
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var category = new Category("Первая");
            
            category.AddPack(new Pack("Фрукты", 
                new []
                {
                    new Card("Что зелёное?", "Яблоко"),
                    new Card("Что жёлтое?", "Банан")
                }, 
                new []
                {
                    LearningWay.LearnYourself, LearningWay.LearnByTest, LearningWay.LearnByTyping
                }, 
                false));
            
            var teacher = new Teacher(new List<Category> {category});
            teacher.StartLearning("Первая", "Фрукты", LearningWay.LearnByTest);
            var face = teacher.GetFaceCard();
            var answers = teacher.GetPossibleAnswers();
            var result = teacher.GiveAnswer("Яблоко");
            var answer = teacher.GetBackCard();
        }
    }
}