using System;

namespace UKits
{
    [Serializable]
    internal class Kit
    {
        public string Name { get; }
        public string Description { get; }
        public string Permission { get; }
        public double Cooldown { get; }
        public ushort MaxUses { get; }
        public int EffectID { get; }

        public bool AutoKit { get; }
        public Item[] Items { get; }
        public ushort[] Vehicles { get; }
        public Kit(string name, string description, string permission, double cooldown, ushort maxuses, int effectid, bool autokit, Item[] items, ushort[] vehicles)
        {
            this.Name = name;
            this.Description = description;
            this.Permission = permission;
            this.Cooldown = cooldown;
            this.MaxUses = maxuses;
            this.EffectID = effectid;
            this.AutoKit = autokit;
            this.Items = items;
            this.Vehicles = vehicles;
        }
    }
}
