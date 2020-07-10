using System;

namespace VoteRewards
{
    [Serializable]
    internal class VoteConfig
    {
        public bool EnableRewards;
        public ServiceProvider Service;
        public RewardPackage[] Rewards;


        public VoteConfig Init()
        {
            EnableRewards = false;
            Service = new ServiceProvider("Name of provider", "API Key", "Insert vote check url", "Insert vote claim url");
            Rewards = new RewardPackage[]
            {
                new RewardPackage("Starter", new PackageItem[] 
                {
                    new PackageItem("NameOrID", 1)
                })
            };
            return this;
        }
    }
    internal class ServiceProvider
    {
        public string Name;
        public string Key;
        public string CheckUrl;
        public string ClaimUrl;

        public ServiceProvider(string name, string key, string checkUrl, string claimUrl)
        {
            Name = name;
            Key = key;
            CheckUrl = checkUrl;
            ClaimUrl = claimUrl;
        }
    }
    internal class RewardPackage
    {
        public string Name;
        public PackageItem[] Items;

        public RewardPackage(string name, PackageItem[] items)
        {
            Name = name;
            Items = items;
        }
    }
    internal class PackageItem
    {
        public string Name;
        public ushort Amount;

        public PackageItem(string name, ushort amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}
