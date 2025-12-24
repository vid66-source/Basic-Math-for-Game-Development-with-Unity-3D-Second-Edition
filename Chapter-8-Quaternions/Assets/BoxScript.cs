using UnityEngine;

public class AlignSpace : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(
            Vector3.forward,
            Vector3.up
        );
    }
}
