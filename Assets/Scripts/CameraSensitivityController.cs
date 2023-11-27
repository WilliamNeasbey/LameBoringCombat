using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CameraSensitivityController : MonoBehaviour
{
    public Slider horizontalSlider;
    public Slider verticalSlider;

    public CinemachineFreeLook freeLookCamera; // Reference to the FreeLook camera rig

    private bool updatingHorizontalSlider = false;
    private bool updatingVerticalSlider = false;

    private void Start()
    {
        // Ensure the slider references are set correctly
        if (horizontalSlider == null || verticalSlider == null)
        {
            Debug.LogError("Ensure that both horizontalSlider and verticalSlider references are set in the Inspector.");
            return;
        }

        // Set listeners for slider changes
        horizontalSlider.onValueChanged.AddListener(delegate { UpdateHorizontalSensitivity(); });
        verticalSlider.onValueChanged.AddListener(delegate { UpdateVerticalSensitivity(); });

        // Set initial sensitivity values from the camera
        if (freeLookCamera != null)
        {
            horizontalSlider.value = freeLookCamera.m_XAxis.m_MaxSpeed;
            verticalSlider.value = freeLookCamera.m_YAxis.m_MaxSpeed;
        }
    }

    public void UpdateHorizontalSensitivity()
    {
        // Update horizontal sensitivity
        if (freeLookCamera != null && !updatingHorizontalSlider)
        {
            updatingHorizontalSlider = true;
            freeLookCamera.m_XAxis.m_MaxSpeed = horizontalSlider.value;
            updatingHorizontalSlider = false;
        }
    }

    public void UpdateVerticalSensitivity()
    {
        // Update vertical sensitivity
        if (freeLookCamera != null && !updatingVerticalSlider)
        {
            updatingVerticalSlider = true;
            freeLookCamera.m_YAxis.m_MaxSpeed = verticalSlider.value;
            updatingVerticalSlider = false;
        }
    }
}
