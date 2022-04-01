using System;
using UnityEngine;
using System.Reflection;
using AarquieSolutions.Extensions.Type;

namespace AarquieSolutions.SaveAndLoadSystem
{
    public class SaverAndLoader : MonoBehaviour
    {
        private void Awake()
        {
            LoadValues();
        }

        private void LoadValues()
        {
            MonoBehaviour[] allAttachedMonobehaviours = GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour monoBehaviour in allAttachedMonobehaviours)
            {
                if (monoBehaviour == null)
                {
                    continue;
                }
                
                FieldInfo[] objectFields = monoBehaviour.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (FieldInfo fieldInfo in objectFields)
                {
                    if (Attribute.GetCustomAttribute(fieldInfo,
                            typeof(SaveDataAttribute)) is SaveDataAttribute saveData)
                    {
                        LoadDataIntoField(fieldInfo, saveData, monoBehaviour);
                    }
                }
            }
        }

        private void LoadDataIntoField(FieldInfo fieldInfo, SaveDataAttribute saveData, MonoBehaviour monoBehaviour)
        {
            Type type = fieldInfo.FieldType;
            switch (saveData.keyDoesNotExistReturnType)
            {
                case KeyDoesNotExistReturnType.DoNotReturn:
                    if (SaveLoadSystem.HasKey(saveData.key))
                    {
                        fieldInfo.SetValue(monoBehaviour, SaveLoadSystem.Load(saveData.key));
                    }
                    break;
                case KeyDoesNotExistReturnType.DefaultValue:
                    fieldInfo.SetValue(monoBehaviour,
                        SaveLoadSystem.Load(saveData.key, type.GetDefaultValue()));
                    break;
                case KeyDoesNotExistReturnType.ConstructorValue:
                    fieldInfo.SetValue(monoBehaviour,
                        SaveLoadSystem.Load(saveData.key, Activator.CreateInstance(type)));
                    break;
                default:
                    break;
            }
        }

        private void SaveValues()
        {
            MonoBehaviour[] allAttachedMonobehaviours = GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour monoBehaviour in allAttachedMonobehaviours)
            {
                if (monoBehaviour == null)
                {
                    continue;
                }
                
                FieldInfo[] objectFields = monoBehaviour.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (FieldInfo fieldInfo in objectFields)
                {
                    if (Attribute.GetCustomAttribute(fieldInfo,
                        typeof(SaveDataAttribute)) is SaveDataAttribute saveData)
                    {
                        SaveLoadSystem.Save(saveData.key, fieldInfo.GetValue(monoBehaviour));
                    }
                }
            }
        }

        private void OnDestroy()
        {
            SaveValues();
        }

    }
}
