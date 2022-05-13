using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;


namespace AarquieSolutions.SaveAndLoadSystem.Editor
{
    public class SaveAndLoadSystemTools : EditorWindow
    {
        private static HashSet<string> keys = new HashSet<string>();
        private static string consoleText;
        private const int ButtonWidth = 50;
        private static bool TypeCannotHaveAttribute(Type type) => type.IsInterface || type.IsEnum || type.IsAbstract;

        [MenuItem("Tools/Save and Load System/Show Keys")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SaveAndLoadSystemTools), false, "Saved Keys Data");
            keys = new HashSet<string>();
        }

        private void OnEnable()
        {
            EditorCoroutineUtility.StartCoroutine(FindAllFieldsWithSaveDataAttribute(), this);
        }

        private void OnGUI()
        {
            if (keys.Count == 0)
            {
                GUILayout.Label("Loading...", EditorStyles.boldLabel);
                return;
            }

            GUILayout.Label("All Keys", EditorStyles.centeredGreyMiniLabel);
            foreach (string key in keys)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(key);
                if (GUILayout.Button("Print Data", GUILayout.Width(ButtonWidth + 50)))
                {
                    if (SaveLoadSystem.Load(key, isInEditorMode: true) != null)
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
                    DeleteKeyInEditorMode(key);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button(" Delete All Keys"))
            {
                foreach (string key in keys)
                {
                    DeleteKeyInEditorMode(key);
                }
            }
            GUILayout.Label($"\n{consoleText}");
        }

        private static void DeleteKeyInEditorMode(string key)
        {
            if (SaveLoadSystem.Delete(key, isInEditorMode: true))
            {
                consoleText = $"Deleted Key: \"{key}\"";
            }
        }

        private IEnumerator FindAllFieldsWithSaveDataAttribute()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Task[] tasks = new Task[assemblies.Length];
            foreach (Assembly assembly in assemblies)
            {
                EditorCoroutineUtility.StartCoroutine(FindAllAttributedMembersInAssembly(assembly), this);
            }

            yield return null;
        }

        private IEnumerator FindAllAttributedMembersInAssembly(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            Task[] tasks = new Task[types.Length];
            
            foreach (Type type in types)
            {
                EditorCoroutineUtility.StartCoroutine(FindAllAttributedMembersInType(type), this);
            }

            yield return null;
        }

        private static IEnumerator FindAllAttributedMembersInType(Type type)
        {
            if (TypeCannotHaveAttribute(type))
            {
                yield return null;
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

            yield return null;
        }
    }
    
}