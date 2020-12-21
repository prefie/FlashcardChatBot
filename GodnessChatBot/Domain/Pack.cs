using System;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot
{
    public class Pack
    {
        public string Name { get; private set; }
        private List<Card> cards;
        public IReadOnlyList<Card> Cards => cards.AsReadOnly();

        public bool CanReverse { get; private set; }

        public Pack(
            string name,
            IEnumerable<Card> cards,
            bool canReverse)
        {
            Name = name;
            CanReverse = canReverse;
            this.cards = cards.ToList();
        }

        public Pack(string name, bool canReverse)
        {
            Name = name;
            CanReverse = canReverse;
            cards = new List<Card>();
        }

        public void OrderCards() => cards = cards.OrderBy(x => x.Statistic).ToList();
        
        public void AddCard(Card card) => cards.Add(card);

        public bool RemoveCard(Card card) => cards.Remove(card);
        public void Rename(string name) => Name = name;

        public Pack Share() => new Pack(Name, Cards, CanReverse);
        
        public override int GetHashCode() => Name.GetHashCode();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            var category = (Pack) obj;
            return string.Equals(Name, category.Name);
        }

        public Card this[int index] => cards[index];
    }
}