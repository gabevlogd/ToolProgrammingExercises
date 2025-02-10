using UnityEngine;

public class MyComponent : MonoBehaviour
{
    public string ConsoleMsg = "Default Message";
    
    public void PrintMessage()
    {
        Debug.Log(ConsoleMsg);
    }
}
