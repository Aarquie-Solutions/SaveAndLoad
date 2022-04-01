using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace AarquieSolutions.SaveAndLoadSystem
{
    public static class SaveLoadSystem
    {
        private const string SavedDataFileName = "SaveData";
        private static readonly string pathToAllKeysFile = $"{Application.persistentDataPath}/{SavedDataFileName}.bsf";
        private static Dictionary<string, string> savedData = new Dictionary<string, string>();
        
        public static bool HasKey(string key)
        {
            return savedData.ContainsKey(key);
        }

        private static void SaveKeys()
        {
            string savePath = $"{Application.persistentDataPath}/{SavedDataFileName}.bsf";
            WriteDictionaryToBinaryFile(savedData, savePath);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadKeys()
        {
            if (!File.Exists(pathToAllKeysFile))
            {
                using FileStream fileStream = File.Open(pathToAllKeysFile, FileMode.Create);
                return;
            }

            ReadBinaryFileToDictionary(savedData, pathToAllKeysFile);
        }

        private static void DeleteKey(string key)
        {
            if (!File.Exists(pathToAllKeysFile))
            {
                return;
            }

            ReadBinaryFileToDictionary(savedData, pathToAllKeysFile);
            savedData.Remove(key);
            
            SaveKeys();
        }

        public static bool Save(string key, object objectToBeSaved, string fileName = null, string path = null)
        {
            if (!objectToBeSaved.GetType().IsSerializable)
            {
                Debug.LogError($"Some objects could not be saved because the objects were not serializable. Key:{key}");
                return false;
            }

            BinaryFormatter formatter = new BinaryFormatter();

            string finalPath = key;

            if (fileName != null)
            {
                finalPath = fileName;
                if (!string.IsNullOrEmpty(path))
                {
                    finalPath = $"{path}/{fileName}";
                }
            }

            string savePath = $"{Application.persistentDataPath}/{finalPath}.bsf";
            Directory.CreateDirectory($"{Application.persistentDataPath}/{path}");
            
            FileStream stream = new FileStream(savePath, FileMode.Create);
            formatter.Serialize(stream, objectToBeSaved);
            stream.Close();

            if (!savedData.ContainsKey(key))
            {
                savedData.Add(key, finalPath);
                SaveKeys();
            }

            return true;
        }

        public static object Load(string key, object defaultValue = null)
        {
            if (!savedData.ContainsKey(key))
            {
                return defaultValue;
            }

            string path = $"{Application.persistentDataPath}/{savedData[key]}.bsf";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using FileStream stream = new FileStream(path, FileMode.Open);

                object loadedObject = formatter.Deserialize(stream);
                return loadedObject;
            }
            else
            {
                Debug.LogError($"File doesn't exist. No data to load for {key}. This shouldn't have happened.");
                return defaultValue;
            }

        }

        public static bool Delete(string key)
        {
            if (!savedData.ContainsKey(key))
            {
                return true;
            }
            
            string path = $"{Application.persistentDataPath}/{savedData[key]}.bsf";
            
            if (File.Exists(path))
            {
                File.Delete(path);
                DeleteKey(key);
                return true;
            }
            else
            {
                Debug.LogError($"File doesn't exist. No data to delete for {key}. This shouldn't have happened.");
                return false;
            }
        }
        
        private static void WriteDictionaryToBinaryFile(Dictionary<string, string> dictionary, string fileName)
        {
            using FileStream fileStream = File.OpenWrite(fileName);
            using BinaryWriter writer = new BinaryWriter(fileStream);
            writer.Write(dictionary.Count);

            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }

        private static void ReadBinaryFileToDictionary(Dictionary<string, string> dictionary, string fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            using FileStream fileStream = File.OpenRead(fileName);
            using BinaryReader reader = new BinaryReader(fileStream);

            if (fileStream.Length == 0)
            {
                return;
            }

            int count = reader.ReadInt32(); 

            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();
                dictionary[key] = value;
            }
        }
    }
}
