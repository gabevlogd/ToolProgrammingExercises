using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform targetPivot;


    private void Update()
    {
        if (targetPivot == null) return;
        transform.RotateAround(targetPivot.position, Vector3.forward, 20 * Time.deltaTime);
    }
}
