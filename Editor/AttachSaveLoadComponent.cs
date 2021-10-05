using UnityEditor;
using UnityEngine;

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
                if (monoBehaviour.GetType().IsDefined(typeof(ContainsDataToBeSaved), false))
                {
                    if (!monoBehaviour.GetComponent<SaveLoadComponent>())
                    {
                        gameObject.AddComponent<SaveLoadComponent>();
                        Debug.Log($"SaveLoadComponent script was attached to Gameobject {gameObject.name} since it has a script attached that uses the attribute ContainsDataToBeSaved");                        
                    }
                }
            }
        }
    }
}
