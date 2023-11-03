using UnityEngine;

public class BobAndRotate : MonoBehaviour
{
    public float bobSpeed = 1.0f;      // Speed of the bobbing motion.
    public float bobHeight = 0.5f;    // Height of the bobbing motion.
    public float rotateSpeed = 30.0f; // Speed of rotation.

    private Vector3 originalPosition;
    private float startTime;

    void Start()
    {
        originalPosition = transform.position;
        startTime = Time.time;
    }

    void Update()
    {
        // Calculate the new Y position for bobbing.
        float newY = originalPosition.y + Mathf.Sin((Time.time - startTime) * bobSpeed) * bobHeight;

        // Set the object's position to create the bobbing motion.
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotate the object.
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
