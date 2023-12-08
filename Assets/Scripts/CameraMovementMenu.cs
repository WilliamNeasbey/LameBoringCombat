using UnityEngine;

public class CameraMovementMenu : MonoBehaviour
{
    public Transform destination; // The destination where the camera will move
    public float moveSpeed = 5f; // Speed of the camera movement

    private bool isMoving = false; // Flag to check if the camera is currently moving

    void Update()
    {
        if (isMoving)
        {
            // Move the camera towards the destination
            transform.position = Vector3.MoveTowards(transform.position, destination.position, moveSpeed * Time.deltaTime);

            // Check if the camera has reached the destination
            if (transform.position == destination.position)
            {
                isMoving = false; // Stop moving the camera
                Debug.Log("Camera reached the destination!");
            }
        }
    }

    public void MoveCameraToDestination()
    {
        if (!isMoving)
        {
            // Triggered by a UI button click to start moving the camera
            isMoving = true;
        }
    }
}
