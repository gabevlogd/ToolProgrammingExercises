using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorChanger))]
public class ColorChangerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        // Get the target object reference and cast it to the ColorChanger type
        ColorChanger colorChanger = (ColorChanger)target;
        
        // Display the objectColor field in the inspector
        colorChanger.objectColor = EditorGUILayout.ColorField("Object Color", colorChanger.objectColor); 
        
        // If the "Change Color" button is pressed, call the UpdateColor method
        if (GUILayout.Button("Change Color"))
        {
            colorChanger.UpdateColor();
        }
    }
}
    