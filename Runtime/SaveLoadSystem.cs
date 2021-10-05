using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoadSystem 
{
    private static Dictionary<string, string> savedData = new Dictionary<string, string>();
    public const string SavedDataFileName = "SaveData";
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
        string path = $"{Application.persistentDataPath}/{SavedDataFileName}.bsf";
        
        if (!File.Exists(path))
        {
            using FileStream fileStream = File.Open(path, FileMode.Create);
            return;
        }

        ReadBinaryFileToDictionary(savedData, path);
    }

    public static bool SaveObject(string key, object objectToBeSaved, string fileName = null, string path = null)
    {
        if (!typeof(object).IsSerializable)
        {
            return false;
        }

        BinaryFormatter formatter = new BinaryFormatter();

        string finalPath = key;

        if (fileName!=null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                finalPath = $"{path}/{fileName}";
            }
            finalPath = fileName;
        }

        string savePath = $"{Application.persistentDataPath}/{finalPath}.bsf";

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

    public static object LoadObject(string key, object defaultValue = null)
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

    public static void WriteDictionaryToBinaryFile(Dictionary<string, string> dictionary, string fileName)
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

    public static bool ReadBinaryFileToDictionary(Dictionary<string, string> dictionary, string fileName)
    {
        if (!File.Exists(fileName))
        {
            return false;
        }

        using (FileStream fileStream = File.OpenRead(fileName))
        using (BinaryReader reader = new BinaryReader(fileStream))
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();
                dictionary[key] = value;
            }
        }
        return true;
    }
}
