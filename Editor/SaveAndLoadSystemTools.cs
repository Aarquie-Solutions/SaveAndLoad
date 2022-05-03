using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AarquieSolutions.SaveAndLoadSystem.Editor
{
    public class SaveAndLoadSystemTools : EditorWindow
    {
        private static HashSet<string> keys = new HashSet<string>();
        private static HashSet<MonoBehaviour> checkedMonoBehaviours = new HashSet<MonoBehaviour>();
        private static string consoleText;
        private const int ButtonWidth = 50;
        private const string matchingPattern = "[SaveData";
        private const string attributeName = "SaveData";
        private static readonly int skipCharacters = "SaveData(\"".Length;
        
        [MenuItem("Tools/Save and Load System/Show Keys")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SaveAndLoadSystemTools), false, "Saved Keys Data");
            keys = new HashSet<string>();
            FindAllFieldsWithSaveDataAttribute();
        }

        [MenuItem("Tools/Save and Load System/Delete All Saved Keys")]
        public static void DeleteAllKeys()
        {
            FindAllFieldsWithSaveDataAttribute();
            keys = new HashSet<string>();
            foreach (string key in keys)
            {
                if (SaveLoadSystem.Delete(key))
                {
                    Debug.Log($"Deleted key: {key}");
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Delete Keys", EditorStyles.boldLabel);
            foreach (string key in keys)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(key);
                if (GUILayout.Button("Print Data",GUILayout.Width(ButtonWidth+50)))
                {
                    if (SaveLoadSystem.Load(key,isInEditorMode: true )!=null)
                    {
                        Debug.Log($"Value for Key: \"{key}\" is {SaveLoadSystem.Load(key)}");
                        consoleText = $"Printed key: \"{key}\" value in console.";
                    }
                    else
                    {
                        consoleText = $"Key: \"{key}\" is null.";
                    }
                }

                if (GUILayout.Button("Delete", GUILayout.Width(ButtonWidth)))
                {
                    if (SaveLoadSystem.Delete(key, isInEditorMode: true))
                    {
                        consoleText = $"Deleted Key: \"{key}\"";
                    }
                }
                
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label(consoleText);
        }
        
        private static void FindAllFieldsWithSaveDataAttribute()
        {
            string[] guids = AssetDatabase.FindAssets("t:Monoscript");
    
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (!monoScript.text.Contains(matchingPattern))
                {
                    continue;
                }
                if (monoScript.name.Contains(nameof(SaveAndLoadSystemTools)))
                {
                    continue;
                }

                IEnumerable<int> indices = GetAllIndexes(monoScript.text, attributeName);
                foreach (int index in indices)
                {
                    int startIndex = index + skipCharacters;
                    int endIndex =  monoScript.text.IndexOf('"', startIndex);
                    keys.Add(monoScript.text.Substring(startIndex, endIndex-startIndex));  
                }

            }
        }

        private static IEnumerable<int> GetAllIndexes(string source, string matchString)
        {
            matchString = Regex.Escape(matchString);
            foreach (Match match in Regex.Matches(source, matchString))
            {
                yield return match.Index;
            }
        }
    }
}