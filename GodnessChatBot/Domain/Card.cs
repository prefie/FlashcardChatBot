namespace GodnessChatBot.Domain
{
    public class Card
    {
        public string Face { get; }
        public string Back { get; }
        public int Statistic { get; set; }

        public Card(string face, string back)
        {
            Face = face;
            Back = back;
        }
        
        public Card(string face, string back, int statistic)
        {
            Face = face;
            Back = back;
            Statistic = statistic;
        }

        public Card Reverse() => new Card(Back, Face);

        public override int GetHashCode() => (Face, Back).GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            var card = (Card) obj;

            return Face == card.Face && Back == card.Back;
        }
    }
}