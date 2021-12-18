using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AarquieSolutions.SaveAndLoadSystem
{
    [InitializeOnLoad]
    public class AttachSaveLoadComponent
    {
        static AttachSaveLoadComponent()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnGUIHierarchyItem;
        }

        static void OnGUIHierarchyItem(int instanceID, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (gameObject != null)
            {
                MonoBehaviour[] monoBehaviours = gameObject.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour monoBehaviour in monoBehaviours)
                {
                    if (monoBehaviour == null)
                    {
                        continue;
                    }
                    
                    FieldInfo[] objectFields = monoBehaviour.GetType()
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    
                    //Got through all the fields in the monobehaviour
                    foreach (FieldInfo fieldInfo in objectFields)
                    {
                        //Check if the field has the attribute: SaveDateAttribute
                        if (System.Attribute.GetCustomAttribute(fieldInfo,
                            typeof(SaveDataAttribute)) is SaveDataAttribute)
                        {
                            //If the component is not attached
                            if (!monoBehaviour.GetComponent<SaveLoadComponent>())
                            {
                                gameObject.AddComponent<SaveLoadComponent>();
                                Debug.Log(
                                    $"SaveLoadComponent script was attached to Gameobject {gameObject.name} since it has a script attached that uses the attribute ContainsDataToBeSaved");
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
