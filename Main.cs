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
        //        {"SniperMonkey Description", "Monkey shoots big dumb bloon hah."},
        //        {"SuperMonkey Description", "I'm like super man but my laser eyes are actually trash."}
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

                string[] fileEntries = Directory.GetFiles("Mods/AdvancedPopTips/"); // getting files from directory

                foreach (var file in fileEntries)
                {
                    if (file.Substring(file.Length - 4, 4).Equals(".txt"))
                    {
                        string rawText = File.ReadAllText(file);
                        string[] overrides = rawText.Split('\n');

                        foreach (var edit in overrides) // editing text table entries
                        {
                            string[] parts = edit.Split('|');

                            LocalizationManager.instance.textTable[parts[0]] = parts[1];
                        }
                    }

                }

                
                // for listing all the text table values
                //foreach (var entry in LocalizationManager.instance.textTable)
                //{
                //    using (System.IO.StreamWriter file =
                //        new System.IO.StreamWriter(@"BTD6TextTable.txt", true))
                //    {
                //        file.WriteLine("L: " + LocalizationManager.instance.activeLanguage + " || K:" + entry.key + " || V:" + entry.value);
                //    }
                //}
            }

            static void Postfix()
            {
                MelonCoroutines.Start(ProcessLocalizationManager());
            }
        }
    }
}