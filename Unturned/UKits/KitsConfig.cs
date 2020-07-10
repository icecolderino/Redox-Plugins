using System;
using System.Linq;

namespace UKits
{
    [Serializable]
    internal class KitsConfig
    {
        public Kit[] Kits;
        internal KitsConfig Init()
        {
            Kits = new Kit[]
            {
                new Kit("basic", "Basic kit for basic needs", string.Empty, 120, 5, 0, false, new Item[]
                {
                    new Item(179, 1),
                    new Item(209, 1)
                }, Array.Empty<ushort>()),
                new Kit("heavy", "Heavy kit for heavy operations", "ukits_guns.use", 600, 2, 0, false, new Item[]
                {
                    new Item(129, 1),
                    new Item(130, 3),
                    new Item(15, 4)
                }, new ushort[] { 7})
            };
            return this;
        }
        internal Kit GetKit(string name)
        {
            return Kits.FirstOrDefault(x => x.Name == name);
        }
    }
}
