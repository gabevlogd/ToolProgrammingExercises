using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ColorChanger : MonoBehaviour
{
    
    public Color objectColor = Color.white;

    public void UpdateColor()
    {
        GetComponent<MeshRenderer>().sharedMaterial.color = objectColor;
    }
}