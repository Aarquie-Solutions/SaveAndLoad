using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AarquieSolutions.SaveAndLoadSystem.Editor
{
    [InitializeOnLoad]
    public class AttachSaverAndLoaderComponent
    {
        static AttachSaverAndLoaderComponent()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnGUIHierarchyItem;
        }

        private static void OnGUIHierarchyItem(int instanceID, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (gameObject == null)
            {
                return;
            }
            
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
                    if (System.Attribute.GetCustomAttribute(fieldInfo, typeof(SaveDataAttribute)) is SaveDataAttribute)
                    {
                        //If the component is not attached
                        if (!monoBehaviour.GetComponent<SaverAndLoader>())
                        {
                            gameObject.AddComponent<SaverAndLoader>();
                            ShowEditorDialogueAsync();
                            return;
                        }
                    }
                }
            }
        }

        private static async void ShowEditorDialogueAsync()
        {
            await Task.Delay(1);
            EditorUtility.DisplayDialog("Component Added",
                $"{nameof(SaverAndLoader)} was added to the gameobject since it is a required component to save and load data.",
                "Okay");
        }
    }
}
