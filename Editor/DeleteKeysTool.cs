using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AarquieSolutions.SaveAndLoadSystem.Editor
{
    public class DeleteKeysTool : EditorWindow
    {
        private static HashSet<string> keys = new HashSet<string>();
        private static HashSet<MonoBehaviour> checkedMonoBehaviours = new HashSet<MonoBehaviour>();
        private static string consoleText;
        private const int ButtonWidth = 50;

        [MenuItem("Tools/Save and Load System/Delete Saved Key")]
        public static void ShowWindow()
        {
            GetWindow(typeof(DeleteKeysTool), false, "Delete Saved Keys");
            keys = new HashSet<string>();
            FindAllScriptsInScenes();
            FindAllPrefabs();
        }

        [MenuItem("Tools/Save and Load System/Delete All Saved Keys")]
        public static void DeleteAllKeys()
        {
            FindAllScriptsInScenes();
            FindAllPrefabs();
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
                if (GUILayout.Button("Delete", GUILayout.Width(ButtonWidth)))
                {
                    if (SaveLoadSystem.Delete(key))
                    {
                        consoleText = $"Deleted Key: {key}";
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label(consoleText);
        }

        private static void FindAllPrefabs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject gameObjectAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                MonoBehaviour[] monoBehaviours = gameObjectAsset.GetComponents<MonoBehaviour>();
                MonoBehaviour[] childrenMonoBehaviours = gameObjectAsset.GetComponentsInChildren<MonoBehaviour>();
                foreach (MonoBehaviour monoBehaviour in monoBehaviours)
                {
                    AddKeyFromMonobehaviour(monoBehaviour);
                }

                foreach (MonoBehaviour monoBehaviour in childrenMonoBehaviours)
                {
                    AddKeyFromMonobehaviour(monoBehaviour);
                }
            }
        }

        private static void FindAllScriptsInScenes()
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                EditorSceneManager.OpenScene(path);


                MonoBehaviour[] allMonoBehaviours =
                    (MonoBehaviour[]) Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour));
                foreach (MonoBehaviour monoBehaviour in allMonoBehaviours)
                {
                    AddKeyFromMonobehaviour(monoBehaviour);
                }
            }
        }

        private static void AddKeyFromMonobehaviour(MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour == null)
            {
                return;
            }

            if (checkedMonoBehaviours.Contains(monoBehaviour))
            {
                return;
            }

            FieldInfo[] objectFields = monoBehaviour.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            //Got through all the fields in the monobehaviour
            foreach (FieldInfo fieldInfo in objectFields)
            {
                //Check if the field has the attribute: SaveDateAttribute
                if (Attribute.GetCustomAttribute(fieldInfo, typeof(SaveDataAttribute)) is SaveDataAttribute
                    saveDataAttribute)
                {
                    checkedMonoBehaviours.Add(monoBehaviour);
                    keys.Add(saveDataAttribute.key);
                }
            }
        }
    }
}