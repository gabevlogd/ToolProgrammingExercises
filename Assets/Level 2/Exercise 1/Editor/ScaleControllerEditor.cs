using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScaleController))]
public class ScaleControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ScaleController scaleController = (ScaleController)target;

        // Slider for each axis
        scaleController.scaleX = EditorGUILayout.Slider("Scala X", scaleController.scaleX, 0.1f, 5f);
        scaleController.scaleY = EditorGUILayout.Slider("Scala Y", scaleController.scaleY, 0.1f, 5f);
        scaleController.scaleZ = EditorGUILayout.Slider("Scala Z", scaleController.scaleZ, 0.1f, 5f);

        // Update the scale if the GUI changed
        if (GUI.changed)
        {
            scaleController.UpdateScale();
        }
    }
}