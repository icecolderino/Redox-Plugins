using System;
using System.Linq;
using System.Collections.Generic;

using Redox.API.Data;
using Redox.API.Commands;
using Redox.API.Configuration;
using Redox.API.Libraries;
using Redox.API.Serialization;
using Redox.API.Plugins.CSharp;

using Redox.Unturned;
using Redox.Unturned.Player;
using Redox.API.Player;

namespace UKits
{
    public class KitsPlugin : RedoxPlugin
    {
        #region Info
        public override string Title => "UKits";
        public override string Description => "Kits plugin for unturned";
        public override string Author => "ice cold";
        public override Version Version => new Version("1.0.0");
        #endregion

        #region Fields
        public Map<ulong, Map<string, int>> KitData = new Map<ulong, Map<string, int>>();

        readonly Dictionary<ulong, Dictionary<string, double>> Cooldown = new Dictionary<ulong, Dictionary<string, double>>();

        KitsConfig config = new KitsConfig();
        Datafile datafile = new Datafile("ukits_Data");
        #endregion

        #region UKits Hooks
        protected override void Load()
        {
            LoadConfig();
            LoadData();

            Commands.Register("kit", "Kit command", CommandCaller.Player, this.KitCommand);
            Commands.Register("kits", "Shows all available kits", CommandCaller.Player, this.KitsCommand);

            foreach (var pl in Server.Players)
                AddPlayerData(pl);
        }     

        private void KitCommand(CommandExecutor executor, string[] args)
        {
            UnturnedPlayer player = Unturned.Server.FindUnturnedPlayerByID(executor.GetPlayer().ID);

            if(args.Length == 0)
            {
                var language = player.Language;
                player.Message(translation.Translate(language, "Help1"));
                player.Message(translation.Translate(language, "Help2"));
                player.Message(translation.Translate(language, "Help3"));
            }
            else
            {
                string arg = args[0];
                ReceiveKit(player, arg);
            }
        }

        private void ReceiveKit(UnturnedPlayer player, string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                player.Message(translation.Translate(player.Language, "Help1"));
                return;
            }
            Kit kit = config.GetKit(name);
            if(kit == null)
            {
                player.Message(translation.Translate(player.Language, "KitNotFound"));
                return;
            }
            if(!HasKitPermission(player, kit.Permission))
            {
                player.Message(translation.Translate(player.Language, "KitPermission"));
                return;
            }
            ulong id = player.UID;
            if(Cooldown[id].TryGetValue(name, out double cd))
            {
                double calc = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds - cd;
                if(calc < kit.Cooldown)
                {
                    double left = Math.Round(Math.Abs(kit.Cooldown - calc));
                    string message = translation.Translate(player.Language, "KitCooldown").Replace("%seconds%", left.ToString());
                    player.Message(message);
                    return;
                }
               
            }
            if(KitData[id].TryGetValue(name, out int used))
            {
                if(kit.MaxUses > 0 && used >= kit.MaxUses)
                {
                    string message = translation.Translate(player.Language, "KitMax").Replace("%used%", used.ToString()).Replace("%max%", kit.MaxUses.ToString());
                    player.Message(message);
                    return;
                }
            }
            foreach(var item in kit.Items)
                player.GiveItem(item.Id, item.Amount);
            foreach (var vehicle in kit.Vehicles)
                player.GiveVehicle(vehicle);
            KitData[id][kit.Name]++;
            Cooldown[id][kit.Name] = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
            player.Message(translation.Translate(player.Language, "KitReceived").Replace("%kit%", kit.Name));
        }

        private void KitsCommand(CommandExecutor executor, string[] args)
        {
            UnturnedPlayer player = Unturned.Server.FindUnturnedPlayerByID(executor.GetPlayer().ID);

            IEnumerable<string> kits = (from kit in config.Kits
                                        where HasKitPermission(player, kit.Permission)
                                        select kit.Name);
            player.Message(string.Join(",", kits));
        }   
        private void AddKitUsage(ulong id, string name)
        {
            if (KitData[id].ContainsKey(name))
                KitData[id][name]++;
            else
                KitData[id].Add(name, 1);
        }

        private bool HasKitPermission(UnturnedPlayer player, string permission)
        {
            if (string.IsNullOrEmpty(permission))
                return true;
            return player.IsAdmin || player.Permission.HasPermission(permission);
        }
        private void LoadData()
        {
            if (datafile.Exists)
                KitData = datafile.ReadObject<Map<ulong, Map<string, int>>>();
        }

        private void LoadConfig()
        {
            PluginConfig cfg = new PluginConfig("UKits", this);
            if (!cfg.Exists)
                cfg.Write(config.Init());
            else
                config = cfg.Read<KitsConfig>();
        }
        private void AddPlayerData(IPlayer player)
        {

            if (!Cooldown.ContainsKey(player.UID))
                Cooldown.Add(player.UID, new Dictionary<string, double>());
            if (!KitData.ContainsKey(player.UID))
                KitData.Add(player.UID, new Map<string, int>());
        }
        protected override void LoadDefaultTranslations()
        {
            translation.Register("english", new Dictionary<string, string>
            {
                {"Help1", "Syntax: /kit <kitname> - Receive a kit." },
                {"Help3", "Syntax: /kits - See all available kits." },
                {"KitReceived", "You've received kit %kit%." },
                {"KitCooldown", "This kit is under cooldown for another %seconds% second(s)." },
                {"KitPermission", "This kit is not available for you." },
                {"KitNotFound", "This kit doesn't exist." },
                {"KitMax", "You've reached the maximum uses of this kit %used%/%max%." }
            });
            translation.Save();
        }
        protected override void Unload()
        {
        }
        #endregion

        #region Hooks

        /* ----------------------------------------
         * OnServerSaved
         * Called when the server gets saved.
         * ----------------------------------------
        */
        public void OnServerSaved()
        {
            datafile.WriteObject(KitData);
        }
        /* ----------------------------------------
         * OnPlayerConnected
         * Called when a player joins the server.
         * ----------------------------------------
        */
        public void OnPlayerConnected(IPlayer player)
        {
            AddPlayerData(player);
        }
        #endregion
    }
}
