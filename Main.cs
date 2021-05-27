using System.Collections;
using System.Reflection;
using System.Net;
using Assets.Scripts.Unity.Localization;
using Harmony;
using MelonLoader;
using UnityEngine;
using System.IO;
using System.IO.Compression;

namespace AdvancedPopTips
{
    public partial class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            System.Console.WriteLine("AdvancedPopTips has loaded!"); // announcing mod launched
        }


        [HarmonyPatch]
        class LocalizationManagerPatch
        {


            static MethodBase TargetMethod()
            {
                return typeof(LocalizationManager).GetMethod("Load");
            }

            static IEnumerator ProcessLocalizationManager()
            {
                while (
                    ReferenceEquals(LocalizationManager.instance, null) ||
                    ReferenceEquals(LocalizationManager.instance.textTable, null) ||
                    ReferenceEquals(LocalizationManager.instance.textTable.entries, null)
                )
                {
                    yield return new WaitForEndOfFrame();
                }


                if (!Directory.Exists("Mods/AdvancedPopTips/")) // downloading the overrides
                {
                    System.Console.WriteLine("Description overrides not detected. This is normal for a first time launch but requires internet. Downloading...");

                    // download
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile("https://github.com/shupik123/Advanced-Pop-Tips/raw/master/AdvancedPopTips.zip", @"Mods/AdvancedPopTips.zip");

                    ZipFile.ExtractToDirectory(@"Mods/AdvancedPopTips.zip", @"Mods/"); // extract

                    File.Delete(@"Mods/AdvancedPopTips.zip"); // delete zip
                }


                string[] fileEntries = Directory.GetFiles("Mods/AdvancedPopTips/"); // getting files from directory
                foreach (var file in fileEntries)
                {
                    if (file.Substring(file.Length - 4, 4).Equals(".txt")) // check file extension
                    {
                        string rawText = File.ReadAllText(file);
                        string[] overrides = rawText.Split('\n'); // split file into lines

                        foreach (var edit in overrides) // for every line
                        {
                            string[] parts = edit.Split('|'); // parsing the mod's file

                            LocalizationManager.instance.textTable[parts[0]] = parts[1]; // editing text table entry
                        }
                    }
                }


                //    // a test override - to be commented out for normal mod builds
                //private static System.Collections.Generic.Dictionary<string, string> LocaleOverrides =
                //    new System.Collections.Generic.Dictionary<string, string>()
                //    {
                //        {"SniperMonkey Description", "i snipe"},
                //        {"SuperMonkey Description", "dart monkey but speed"}
                //    };



                //    // writes all the text table data - to be commented out for normal mod builds
                //
                //    foreach (var entry in LocalizationManager.instance.textTable) // for listing all the text table values
                //    {
                //        using (System.IO.StreamWriter file =
                //            new System.IO.StreamWriter(@"BTD6TextTable.txt", true))
                //        {
                //            // only one of these should be uncommented

                //            file.WriteLine("L: " + LocalizationManager.instance.activeLanguage + " || K:" + entry.key + " || V:" + entry.value);
                //            //file.WriteLine(entry.key + "|");
                //        }
                //    }


            }

            static void Postfix()
            {
                MelonCoroutines.Start(ProcessLocalizationManager());
            }
        }
    }
}