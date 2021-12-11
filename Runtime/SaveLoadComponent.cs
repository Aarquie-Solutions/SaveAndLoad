using System;
using UnityEngine;
using System.Reflection;

namespace AarquieSolutions.SaveAndLoadSystem
{
    public class SaveLoadComponent : MonoBehaviour
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
                if (monoBehaviour.GetType().IsDefined(typeof(ContainsDataToBeSaved), false))
                {
                    FieldInfo[] objectFields = monoBehaviour.GetType()
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    foreach (FieldInfo fieldInfo in objectFields)
                    {
                        if (Attribute.GetCustomAttribute(fieldInfo, typeof(SaveData)) is SaveData saveData)
                        {
                            Type type = fieldInfo.FieldType;
                            switch (saveData.keyDoesNotExistReturnType)
                            {
                                case KeyDoesNotExistReturnType.DoNotReturn:
                                    if (SaveLoadSystem.HasKey(saveData.key))
                                    {
                                        fieldInfo.SetValue(monoBehaviour, SaveLoadSystem.LoadObject(saveData.key));
                                    }

                                    break;
                                case KeyDoesNotExistReturnType.DefaultValue:
                                    fieldInfo.SetValue(monoBehaviour,
                                        SaveLoadSystem.LoadObject(saveData.key, type.GetDefaultValue()));
                                    break;
                                case KeyDoesNotExistReturnType.ConstructorValue:
                                    fieldInfo.SetValue(monoBehaviour,
                                        SaveLoadSystem.LoadObject(saveData.key, Activator.CreateInstance(type)));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void SaveValues()
        {
            MonoBehaviour[] allAttachedMonobehaviours = GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour monoBehaviour in allAttachedMonobehaviours)
            {
                if (monoBehaviour.GetType().IsDefined(typeof(ContainsDataToBeSaved), false))
                {
                    FieldInfo[] objectFields = monoBehaviour.GetType()
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    foreach (FieldInfo fieldInfo in objectFields)
                    {
                        if (Attribute.GetCustomAttribute(fieldInfo, typeof(SaveData)) is SaveData saveData)
                        {
                            SaveLoadSystem.SaveObject(saveData.key, fieldInfo.GetValue(monoBehaviour));
                        }
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
