using DiscordRPC;
using DiscordRPC.Logging;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using Pholib;
using ProcGen;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DiscordRPMod
{
    public class DiscordRPMod : UserMod2
    {
        public static Timestamps gameLaunched;

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            gameLaunched = Timestamps.Now;
            new POptions().RegisterOptions(this, typeof(DiscordRPOptions));
            DiscordRPMod_GameOnLoadPatch_OnSpawn.ReadSettings(); // Read settings early for the notepad description setting.

            new PLocalization().Register();
            // Utilities.GenerateStringsTemplate(typeof(PHO_STRINGS));
        }
    }


    // Load PLib settings on game load
    [HarmonyPatch(typeof(SaveLoader), "OnSpawn")]
    public static class DiscordRPMod_GameOnLoadPatch_OnSpawn
    {
        public const string DISCORD_APP_ID = "363430471322828800"; // oni discord app id
        public static List<DiscordRpcClient> clients;
        public static DiscordRPOptions Settings { get; private set; }
        private static DiscordRPC.Assets assets = null;

        public static void Prefix()
        {
            ReadSettings();
        }

        public static void Dispose()
        {
            clients.ForEach(client => client.Dispose());
            clients.Clear();
        }

        private static RichPresence CreateRPC(bool onLoadSave = false)
        {
            RichPresence richPresence = new RichPresence();

            richPresence.Timestamps = DiscordRPMod.gameLaunched;
            richPresence.Type = ActivityType.Playing;
            if (Settings.ShowGameName)
                richPresence.Details = SaveLoader.Instance.GameInfo.baseName;

            if (Settings.ShowCycleAndDups)
                richPresence.State = GetState(onLoadSave);

            if (Settings.ShowGameName)
                richPresence.Details = SaveLoader.Instance.GameInfo.baseName;

            if (Settings.ShowAsteroid)
            {
                richPresence.Assets = assets;
            }
            return richPresence;
        }

        public static string GetState(bool onLoadSave = false)
        {
            return $"Cycle {GameUtil.GetCurrentCycle()} with {(onLoadSave ? SaveLoader.Instance.GameInfo.numberOfDuplicants : Components.LiveMinionIdentities.Count)} dups";
        }
        public static void ReadSettings()
        {
            // read the option each time the game is loaded - so we don't need to restart all the game
            Settings = POptions.ReadSettings<DiscordRPOptions>();
            if (Settings == null)
            {
                Settings = new DiscordRPOptions();
            }

        }

        // Manage multiple discord clients 
        public static int GetDiscordProcessCount()
        {
            return Mathf.Max(Process.GetProcesses().Count(p => p.ProcessName.ToLower().Contains("discord") && p.MainWindowHandle != IntPtr.Zero), 1);
        }


        public static void UpdateRPC()
        {
            var rpc = CreateRPC();
            clients.ForEach(client => client.SetPresence(rpc));
        }

        private static void OnLivingMinionsChanged(MinionIdentity minion)
        {
            UpdateRPC();
        }

        public static void Postfix(SaveLoader __instance)
        {
            if (clients != null)
            {
                Dispose();
            }
            clients = new List<DiscordRpcClient>();

            if (!Settings.ModEnabled) return;
                
            Klei.CustomSettings.SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(Klei.CustomSettings.CustomGameSettingConfigs.ClusterLayout);
            ClusterLayout clusterLayout;
            SettingsCache.clusterLayouts.clusterCache.TryGetValue(currentQualitySetting.id, out clusterLayout);

            Logs.LogIfDebugging(clusterLayout.name);

            var world = ClusterManager.Instance?.activeWorld;
            string worldNameFriendly = Strings.Get(clusterLayout.name);
            string worldNameLink = Strings.Get(clusterLayout.name);

            if (!DiscordRPMod_Localization_Initialize.reverseTranslationStrings.TryGetValue(worldNameLink, out worldNameLink))
            {
                worldNameLink = "Asteroid";
            }
            worldNameLink = worldNameLink.Replace("<sup>", "").Replace("</sup>", "").Replace(" ", "_");

            string smallImageURL = "https://oxygennotincluded.wiki.gg/images/Asteroid.png";
            // In base game it's more easy to get the asteroid name
            if (!DlcManager.FeatureClusterSpaceEnabled())
            {
                smallImageURL = $"https://oxygennotincluded.wiki.gg/images/{worldNameLink}_Asteroid.png";
            }
            else
            {
                smallImageURL = GetSpacedOutLink(worldNameLink);
            }
            Logs.Log(smallImageURL);

            // Safe quit
            Application.wantsToQuit += () =>
            {
                Dispose();
                return true;
            };

            for (int i = 0; i < GetDiscordProcessCount(); i++)
            {
                // Create the client and setup some basic events
                var client = new DiscordRpcClient(DISCORD_APP_ID, i)
                {
                    Logger = new UnityLogger(LogLevel.Error),
                };

                client.OnReady += (sender, args) =>
                {
                    Logs.Log("Connected to discord: " + args.Dump());

                    Components.LiveMinionIdentities.OnAdd += OnLivingMinionsChanged;
                    Components.LiveMinionIdentities.OnRemove += OnLivingMinionsChanged;
                };

                assets = new DiscordRPC.Assets
                {
                    SmallImageKey = smallImageURL,
                    SmallImageText = worldNameFriendly,
                };

                client.Initialize();

                client.SetPresence(CreateRPC(true));

                clients.Add(client);
            }
        }

        private static string GetSpacedOutLink(string worldNameLink)
        {
            Logs.Log($"Detected world name: {worldNameLink} (SpacedOut)");
            if (worldNameLink.StartsWith("Terrania")) worldNameLink = "Terrania_Asteroid.png";
            if (worldNameLink.StartsWith("Folia")) worldNameLink = "Folia_Asteroid.png";
            if (worldNameLink.StartsWith("Quagmiris")) worldNameLink = "Quagmiris_Asteroid.png";
            if (worldNameLink.StartsWith("Terra")) worldNameLink = "Terra_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Verdante")) worldNameLink = "Verdante_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Squelchy")) worldNameLink = "Squelchy_Asteroid.png";
            if (worldNameLink.StartsWith("Rime")) worldNameLink = "Rime_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Oceania")) worldNameLink = "Oceania_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Oasisse")) worldNameLink = "Oasisse_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("The Badlands")) worldNameLink = "The_Badlands_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Arboria")) worldNameLink = "Arboria_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Aridio")) worldNameLink = "Aridio_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Volcanea")) worldNameLink = "Volcanea_Asteroid_(Spaced_Out).png";
            if (worldNameLink.Contains("Metallic Swampy")) worldNameLink = "Metallic_Swampy_Asteroid.png";
            if (worldNameLink.Contains("Frozen Forest")) worldNameLink = "Frozen_Forest_Asteroid.png";
            if (worldNameLink.Contains("The Desolands")) worldNameLink = "The_Desolands_Asteroid.png";
            if (worldNameLink.Contains("Flipped")) worldNameLink = "Flipped_Asteroid.png";
            if (worldNameLink.Contains("Radioactive Ocean")) worldNameLink = "Radioactive_Ocean_Asteroid.png";
            if (worldNameLink.StartsWith("Ceres")) worldNameLink = "Ceres_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Blasted Ceres")) worldNameLink = "Blasted_Ceres_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Ceres Mantle")) worldNameLink = "Ceres_Mantle_Asteroid.png";
            if (worldNameLink.StartsWith("Ceres Minor Cluster")) worldNameLink = "Ceres_Minor_Asteroid.png";
            if (worldNameLink.StartsWith("Relica")) worldNameLink = "Relica_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Relica Minor")) worldNameLink = "Relica_Minor_Asteroid.png";
            if (worldNameLink.StartsWith("Marinea")) worldNameLink = "Marinea_Asteroid_(Spaced_Out).png";
            if (worldNameLink.StartsWith("Marinea Minor")) worldNameLink = "Marinea_Minor_Asteroid.png";
            if (worldNameLink.StartsWith("RelicAAAA")) worldNameLink = "RelicAAAAAAAGHH_Asteroid.png";
            return $"https://oxygennotincluded.wiki.gg/images/{worldNameLink}";
        }
    }

    // Update counter on auto save.
    [HarmonyPatch(typeof(ColonyAchievementTracker), "OnNewDay")]
    public static class DiscordRPMod_ColonyAchievementTracker_OnNewDay
    {
        public static void Postfix()
        {
            DiscordRPMod_GameOnLoadPatch_OnSpawn.UpdateRPC();
        }
    }

    // Dispose clients on game quit.
    [HarmonyPatch(typeof(PauseScreen), "OnQuitConfirm")]
    public static class DiscordRPMod_PauseScreen_OnQuitConfirm
    {
        public static void Postfix()
        {
            DiscordRPMod_GameOnLoadPatch_OnSpawn.Dispose();
        }
    }

    // Read all world names before translation
    [HarmonyPatch(typeof(Localization), "Initialize")]
    public static class DiscordRPMod_Localization_Initialize
    {

        public static Dictionary<string, string> reverseTranslationStrings;

        public static void Prefix()
        {
            reverseTranslationStrings = new Dictionary<string, string>();
            InspectLocString(typeof(CLUSTER_NAMES), addString: true);
            InspectLocString(typeof(WORLDS), addString: true);

        }
        public static void Postfix()
        {
            InspectLocString(typeof(CLUSTER_NAMES), addString: false);
            InspectLocString(typeof(WORLDS), addString: false);
            Logs.LogIfDebugging(reverseTranslationStrings.Dump());
        }

        private static void InspectLocString(Type type, string parent_path = "STRINGS.", bool addString = true)
        {
            string text = parent_path;

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            text = text + type.Name + ".";
            foreach (FieldInfo fieldInfo in fields)
            {
                if (fieldInfo.FieldType == typeof(LocString) && fieldInfo.IsStatic)
                {
                    string fullFieldPath = text + fieldInfo.Name;

                    if (fullFieldPath.StartsWith("STRINGS.") && fullFieldPath.EndsWith(".NAME"))
                    {
                        if (addString)
                        {
                            LocString locString = (LocString)fieldInfo.GetValue(null);
                            reverseTranslationStrings.Add(fullFieldPath, locString.text);
                        }
                        else
                        {
                            LocString locString = (LocString)fieldInfo.GetValue(null);
                            KeyValuePair<string, string> foundPair = reverseTranslationStrings.FirstOrDefault(pair => pair.Key == fullFieldPath);
                            if (!string.IsNullOrEmpty(foundPair.Key))
                            {
                                reverseTranslationStrings.ChangeKey(foundPair.Key, locString);
                            }
                        }
                    }
                }
            }
            Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            for (int i = 0; i < nestedTypes.Length; i++)
            {
                InspectLocString(nestedTypes[i], text, addString);
            }
        }
    }

}
