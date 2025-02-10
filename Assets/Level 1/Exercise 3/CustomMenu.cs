using UnityEngine;
using UnityEditor;

public class CustomMenu
{
#if UNITY_EDITOR
    [MenuItem("Tools/Create Custom Object")]
    private static void CreateCustomObject()
    {
        // Creates a new GameObject
        GameObject newObject = new GameObject("Custom Object");
        
        // Place it at the center of the scene
        newObject.transform.position = Vector3.zero;

        // Automatically select the new object in the Hierarchy
        Selection.activeGameObject = newObject;

        Debug.Log("Custom Object created!");
    }
#endif
    
}