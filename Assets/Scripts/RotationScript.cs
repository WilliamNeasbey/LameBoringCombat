using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public Vector3 rotationSpeeds = new Vector3(30f, 30f, 30f); // Speeds of rotation in degrees per second for each axis

    void Update()
    {
        // Calculate the rotation amounts based on the speeds and frame time
        float rotationX = rotationSpeeds.x * Time.deltaTime;
        float rotationY = rotationSpeeds.y * Time.deltaTime;
        float rotationZ = rotationSpeeds.z * Time.deltaTime;

        // Apply the rotations using the specified speeds
        transform.Rotate(Vector3.right, rotationX);
        transform.Rotate(Vector3.up, rotationY);
        transform.Rotate(Vector3.forward, rotationZ);
    }
}
