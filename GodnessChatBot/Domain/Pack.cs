using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot.Domain
{
    public class Pack : IEnumerable<Card>
    {
        public string Name { get; }
        private List<Card> cards;
        public int Count => cards.Count;

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
        
        public override int GetHashCode() => Name.GetHashCode();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<Card> GetEnumerator()
        {
            foreach (var card in cards)
                yield return card;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            var pack = (Pack) obj;
            return string.Equals(Name, pack.Name);
        }

        public Card this[int index] => cards[index];
    }
}