using UnityEngine;

public class ScaleController : MonoBehaviour
{
    [Range(0.1f, 5f)] public float scaleX = 1f;
    [Range(0.1f, 5f)] public float scaleY = 1f;
    [Range(0.1f, 5f)] public float scaleZ = 1f;

    public void UpdateScale()
    {
        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
}