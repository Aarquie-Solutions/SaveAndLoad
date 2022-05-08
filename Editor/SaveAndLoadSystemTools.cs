using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AarquieSolutions.SaveAndLoadSystem.Editor
{
    public class SaveAndLoadSystemTools : EditorWindow
    {
        private static HashSet<string> keys = new HashSet<string>();
        private static string consoleText;
        private const int ButtonWidth = 50;
        
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
            if (keys.Count == 0)
            {
                GUILayout.Label("Loading...", EditorStyles.boldLabel);
            }
            
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

        private static async void FindAllFieldsWithSaveDataAttribute()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Task[] tasks = new Task[assemblies.Length];
            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                tasks[i] = FindAllAttributedMembersInAssembly(assembly);
            }

            await Task.WhenAll(tasks);
        }

        private static async Task FindAllAttributedMembersInAssembly(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            Task[] tasks = new Task[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                tasks[i] = FindAllAttributedMembersInType(type);
            }

            await Task.WhenAll(tasks);
        }

        private static async Task FindAllAttributedMembersInType(Type type)
        {
            if (type.IsInterface || type.IsEnum)
            {
                await Task.CompletedTask;
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            MemberInfo[] memberInfos = type.GetMembers(flags);

            foreach (MemberInfo memberInfo in memberInfos)
            {
                if (memberInfo.CustomAttributes.ToArray().Length > 0)
                {
                    SaveDataAttribute saveDataAttribute = memberInfo.GetCustomAttribute<SaveDataAttribute>();
                    if (saveDataAttribute != null)
                    {
                        keys.Add(saveDataAttribute.key);
                    }
                }
            }
            await Task.CompletedTask;
        }
    }
}