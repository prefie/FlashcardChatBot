﻿using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot.Domain
{
    public class Pack
    {
        public string Name { get; private set; }
        private List<Card> cards;
        public IReadOnlyList<Card> Cards => cards.AsReadOnly();

        public Pack(
            string name,
            IEnumerable<Card> cards)
        {
            Name = name;
            this.cards = cards.ToList();
        }

        public Pack(string name)
        {
            Name = name;
            cards = new List<Card>();
        }

        public void OrderCards() => cards = cards.OrderBy(x => x.Statistic).ToList();
        
        public void AddCard(Card card) => cards.Add(card);

        public bool RemoveCard(Card card) => cards.Remove(card);
        public void Rename(string name) => Name = name;

        public Pack Share() => new Pack(Name, Cards);
        
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