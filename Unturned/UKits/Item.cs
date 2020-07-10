using System;


namespace UKits
{
    [Serializable]
    internal class Item
    {
        public ushort Id { get; }
        public byte Amount { get; }

        public Item(ushort id, byte  amount)
        {
            this.Id = id;
            this.Amount = amount;
        }
    }
}
