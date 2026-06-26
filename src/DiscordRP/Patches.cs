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

namespace DiscordRPMod
{
    public class DiscordRPMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            new POptions().RegisterOptions(this, typeof(DiscordRPOptions));
            DiscordRPMod_GameOnLoadPatch.ReadSettings(); // Read settings early for the notepad description setting.

            new PLocalization().Register();
            Utilities.GenerateStringsTemplate(typeof(PHO_STRINGS));
        }
    }

    // Read all world names before translation
    [HarmonyPatch(typeof(GlobalAssets), "OnPrefabInit")]
    public static class DiscordRPMod_GlobalAssets_OnPrefabInit
    {

        public static Dictionary<string, string> englishStringFromClassPath;

        public static void Prefix()
        {
            englishStringFromClassPath = new Dictionary<string, string>();
            Logs.Log("Prefix");
            Logs.Log(LocString.GetStrings(typeof(STRINGS.WORLDS)).ToList().ListToString());

            InspectLocString(typeof(STRINGS.WORLDS));

        }
        
        private static void InspectLocString(Type type, string parent_path = "STRINGS.")
        {
            string text = parent_path;

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            text = text + type.Name + ".";
            Logs.Log(text);
            Logs.Log(fields.Length);
            foreach (FieldInfo fieldInfo in fields)
            {
                Logs.Log(fieldInfo);
                if (fieldInfo.FieldType == typeof(LocString) && fieldInfo.IsStatic)
                {
                    string text2 = text + fieldInfo.Name;
                    Logs.Log("text2: " + text2);
                    LocString locString = (LocString)fieldInfo.GetValue(null);
                    Logs.Log("locString: " + locString);
                    locString.SetKey(text2);
                    string text3 = locString.text;
                    Logs.Log("text3: " + text3);
                    Strings.Add(new string[]
                    {
                        text2,
                        text3
                    });
                    fieldInfo.SetValue(null, locString);
                }
            }
            Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            for (int i = 0; i < nestedTypes.Length; i++)
            {
                LocString.CreateLocStringKeys(nestedTypes[i], text);
            }
        }

    }

    // Load PLib settings on game load
    [HarmonyPatch(typeof(SaveLoader), "OnSpawn")]
    public static class DiscordRPMod_GameOnLoadPatch
    {
        public const string DISCORD_APP_ID = "363430471322828800"; // oni discord app id
        public static List<DiscordRpcClient> clients;
        public static DiscordRPOptions Settings { get; private set; }

        public static void Prefix()
        {
            ReadSettings();
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

        public static int GetDiscordProcessCount()
        {
            return Process.GetProcesses().Count(p => p.ProcessName.ToLower().Contains("discord") && p.MainWindowHandle != IntPtr.Zero);
        }

        public static void Postfix(SaveLoader __instance)
        {
            clients = new List<DiscordRpcClient>();

            Klei.CustomSettings.SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(Klei.CustomSettings.CustomGameSettingConfigs.ClusterLayout);
            ClusterLayout clusterLayout;
            SettingsCache.clusterLayouts.clusterCache.TryGetValue(currentQualitySetting.id, out clusterLayout);

            var rootTable = Traverse.Create(typeof(Strings)).Field<StringTable>("RootTable").Value;

            //Traverse.Create(rootTable).
            Logs.Log(rootTable.Dump());

            string asteroidName = Strings.Get(clusterLayout.name).String.Replace("<sup>", "").Replace("</sup>", "").Replace(" ", "_");
            Logs.Log(asteroidName);
            Logs.Log($"https://oxygennotincluded.wiki.gg/images/{asteroidName}.png");

            for (int i = 0; i < GetDiscordProcessCount(); i++)
            {
                // Create the client and setup some basic events
                var client = new DiscordRpcClient(DISCORD_APP_ID, i)
                {
                    Logger = new UnityLogger(LogLevel.Error),

                };

                client.OnReady += (sender, args) =>
                {
                    Debug.Log("Connected to discord: " + args.Dump());
                };

                //Connect to the RPC
                client.Initialize();

                /*
			    GameClock.Instance.GetTimePlayedInSeconds() / 3600f).ToString("0.00")), ToolTipScreen.Instance.defaultTooltipBodyStyle));
                */

                //Set the rich presence
                client.SetPresence(new RichPresence()
                {

                    Details = SaveLoader.Instance.GameInfo.baseName,
                    State = $"Cycle {GameUtil.GetCurrentCycle()} with {SaveLoader.Instance.GameInfo.numberOfDuplicants} dups",
                    Timestamps = Timestamps.Now,

                    Assets = new DiscordRPC.Assets()
                    {
                        SmallImageKey = $"https://oxygennotincluded.wiki.gg/images/{asteroidName}.png"
                    },
                    Type = ActivityType.Playing,

                    //StateUrl = "https://www.google.fr"

                });
                // client.UpdateState // add that when cycle pass
                clients.Add(client);
                // client.Dispose(); // Add that when closing
            }
        }
    }

}
