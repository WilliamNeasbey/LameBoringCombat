using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam; // Reference to the Cinemachine FreeLook component

    public float zoomSpeed = 10f; // Adjust the zoom speed as needed
    public float minFOV = 30f;   // Minimum field of view
    public float maxFOV = 90f;   // Maximum field of view

    private void Update()
    {
        // Get the mouse scroll wheel input
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

        // Calculate the new field of view
        float newFOV = freeLookCam.m_Lens.FieldOfView + scrollWheelInput * zoomSpeed;

        // Clamp the new field of view within the min and max values
        newFOV = Mathf.Clamp(newFOV, minFOV, maxFOV);

        // Apply the new field of view to the Cinemachine FreeLook component
        freeLookCam.m_Lens.FieldOfView = newFOV;
    }
}
