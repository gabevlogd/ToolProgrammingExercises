using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MyComponent))]
public class MyComponentEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        // get the target object reference and cast it to the MyComponent type
        MyComponent myComponent = (MyComponent)target;
        
        // draw the text field for the ConsoleMsg 
        myComponent.ConsoleMsg = EditorGUILayout.TextField("Console Message", myComponent.ConsoleMsg);

        // draw the button
        // if the button is clicked, call the PrintMessage method
        if (GUILayout.Button("Print Message"))
        {
            myComponent.PrintMessage();
        }
    }
}
