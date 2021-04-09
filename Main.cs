using System.Collections;
using System.Reflection;
using Assets.Scripts.Unity.Localization;
using Harmony;
using NKHook6;
using MelonLoader;
using UnityEngine;
using System.IO;

namespace AdvancedPopTips
{
    public partial class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            NKHook6.Logger.Log("AdvancedPopTips has loaded!"); // announcing mod launched
        }

        //private static System.Collections.Generic.Dictionary<string, string> LocaleOverrides =
        //    new System.Collections.Generic.Dictionary<string, string>()
        //    {
        //        {"SniperMonkey Description", "no shot"},
        //        {"SuperMonkey Description", "i dart monke but zoooooom"}
        //    };

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
                    // download and unzip
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


                //    // writes all the text table data - to be commented out for normal mod builds
                
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