using UnityEngine;

public class MenuCameraOrbit : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 10f;
    public Vector3 offset = new Vector3(0f, 1.5f, -5f);

    void LateUpdate()
    {
        if (!target) return;

        // Rotate around Y axis
        transform.RotateAround(
            target.position,
            Vector3.up,
            rotationSpeed * Time.deltaTime
        );

        // Look at target
        transform.LookAt(target.position);
    }
}