using System;

namespace GodnessChatBot
{
    public class Card
    {
        public string Face { get; }
        public string Back { get; }

        public Card(string face, string back)
        {
            Face = face;
            Back = back;
        }

        public Card Reverse() => new Card(Back, Face);
    }
}