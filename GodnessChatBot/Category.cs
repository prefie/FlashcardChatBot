using System;
using System.Collections.Generic;
using System.Linq;

namespace GodnessChatBot
{
    public class Category
    {
        public string Name { get; private set; }
        
        private readonly List<Pack> packs;
        public IReadOnlyList<Pack> Packs => packs.AsReadOnly();

        public Category(string name)
        {
            Name = name;
            packs = new List<Pack>();
        }

        public void AddPack(Pack pack) => packs.Add(pack);

        public Pack GetPack(string name)
        {
            foreach (var pack in packs.Where(pack => pack.Name == name))
                return pack;

            throw new ArgumentException($"Category {name} is not in this category.");
        }
        
        public void Rename(string name) => Name = name;

        public override int GetHashCode() => Name.GetHashCode();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            var category = (Category) obj;
            return string.Equals(Name, category.Name);
        }
    }
}