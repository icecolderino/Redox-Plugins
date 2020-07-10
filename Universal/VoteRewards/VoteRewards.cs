using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Redox.API.Commands;
using Redox.API.Plugins.CSharp;


using VoteRewards.Commands;
using Redox.API.Configuration;
using Redox.API.Player;

namespace VoteRewards
{
    public class VoteRewards : RedoxPlugin
    {
        #region Information
        public override string Title => "VoteRewards";
        public override string Description => "Allow people to vote your server for a reward in return.";
        public override string Author => "ice cold";
        public override Version Version => new Version("1.0.0");
        #endregion

        #region Fields
        internal static VoteRewards Instance;
      //  internal static Regex URL_REGEX = new Regex(@"(https ?| ftp)://(-\.)?([^\s/?\.#-]+\.?)+(/[^\s]*)?$@iS");
        internal static string Chatname = "VoteRewards";
        internal static VoteConfig Config = new VoteConfig();
        internal static Random Random = new Random();
        #endregion

       

        protected override void Load()
        {
            Instance = this;
            Commands.Register("vote", "Vote command", CommandCaller.Player, VoteCommand.Execute);
            LoadConfig();
        }
        internal static void GiveReward(IPlayer player)
        {
            if (Config.EnableRewards)
            {
                int rewards = Config.Rewards.Length;
                int index = Random.Next(rewards);
                RewardPackage reward = Config.Rewards[index];

                foreach(PackageItem item in reward.Items)
                {
                    player.Give(item.Name, item.Amount);
                }
                
            }
        }
        private void LoadConfig()
        {
            var config = new PluginConfig("VoteRewards", this);
            if (!config.Exists)
                config.Write(Config.Init());
            else
                Config = config.Read<VoteConfig>();

            if (Config.Service == null)
            {
                Logger.LogWarning("[VoteRewards] You didn't provide any voting service. This plugin cannot be used.");
                return;
            }
            /*
            if(!VoteRewards.URL_REGEX.IsMatch(VoteRewards.Config.Service.CheckUrl))
            {
                Logger.LogWarning("[VoteRewards] The url you inserted is invalid. This plugin cannot be used.");
                return;
            }
            */
        }
        protected override void LoadDefaultTranslations()
        {
            translation.Register("english", new Dictionary<string, string>
            {
                {"NotConfigured", "The voting system hasn't been configured yet. Please contact an admin." },
                {"VoteFailed", "Something went wrong when processing the vote. Please contact an administrator." },
                {"NoServices", "This server has not yet set up a voting service." },
                {"VoteAvailable", "You have one unclaimed vote to your disposal. Please type \"/vote claim\" to claim it." },
                {"VoteUnavailable", "You have no unclaimed votes." },
                {"VoteClaimed", "You've succesfully claimed your vote." },
                {"AlreadyVoted", "You already voted for this server. You can vote every 24 hours." },
                {"VoteBroadcast", "{0} claimed his reward for voted for this server {1} times." }      
            });
            translation.Save();
        }
        protected override void Unload()
        {
        }

    }
}
