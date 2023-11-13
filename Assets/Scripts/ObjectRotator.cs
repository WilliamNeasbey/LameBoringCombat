using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public float rotationSpeedX = 30f; // Adjust this speed in the Unity Editor for X-axis
    public float rotationSpeedY = 20f; // Adjust this speed in the Unity Editor for Y-axis

    // Update is called once per frame
    void Update()
    {
        // Rotate the object around its right axis (X-axis)
        transform.Rotate(Vector3.right, rotationSpeedX * Time.deltaTime);

        // Rotate the object around its up axis (Y-axis)
        transform.Rotate(Vector3.up, rotationSpeedY * Time.deltaTime);
    }
}
