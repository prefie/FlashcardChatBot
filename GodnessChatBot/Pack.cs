using System;
using System.Collections;
using System.Collections.Generic;

namespace GodnessChatBot
{
    public class Pack
    {
        public string Name { get; private set; }
        private List<Card> cards;
        public IReadOnlyList<Card> Cards => cards.AsReadOnly();
        private List<LearningWay> learningWay;
        public IReadOnlyList<LearningWay> LearningWays => learningWay.AsReadOnly();
        
        public bool IsReadonly { get; private set; }

        public bool CanReverse { get; private set; }

        public Pack(
            string name,
            List<Card> cards,
            List<LearningWay> learningWay,
            bool isReadonly,
            bool canReverse)
        {
            Name = name;
            IsReadonly = isReadonly;
            CanReverse = canReverse;
            this.cards = cards;
            this.learningWay = learningWay;
        }

        public Pack(string name)
        {
            Name = name;
        }

        public void AddCard(Card card) => cards.Add(card);

        public bool RemoveCard(Card card) => cards.Remove(card);
        public void Rename(string name) => Name = name;

        public void Share()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Learn<T>(LearningWay learningWay)
        {
            throw new NotImplementedException();
        }
    }
}