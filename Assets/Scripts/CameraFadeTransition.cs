using UnityEngine;
using System.Collections;

public class CameraFadeTransition : MonoBehaviour
{
    public Camera firstCamera;
    public Camera secondCamera;
    public Material transitionMaterial;
    public float transitionDuration = 2.0f;

    private RenderTexture renderTexture;
    private bool isTransitioning = false;
    private float transitionStartTime;

    void Start()
    {
        // Create a new Render Texture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);

        // Set the second camera's target texture initially
        secondCamera.targetTexture = renderTexture;

        StartCoroutine(TransitionEveryFiveSeconds());
    }

    IEnumerator TransitionEveryFiveSeconds()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // Wait for 5 seconds
            StartTransition();
        }
    }

    private void Update()
    {
        if (isTransitioning)
        {
            float transitionProgress = (Time.time - transitionStartTime) / transitionDuration;
            if (transitionProgress < 1.0f)
            {
                // Set the transition material's "Cutoff" property to control the transition
                transitionMaterial.SetFloat("_Cutoff", transitionProgress);
            }
            else
            {
                // Transition is complete
                EndTransition();
            }
        }
    }

    private void StartTransition()
    {
        isTransitioning = true;
        transitionStartTime = Time.time;

        // Disable the first camera
        firstCamera.enabled = false;

        // Enable the second camera
        secondCamera.enabled = true;
    }

    private void EndTransition()
    {
        isTransitioning = false;

        // Disable the second camera
        secondCamera.enabled = false;

        // Enable the first camera
        firstCamera.enabled = true;

        // Reset the transition material
        transitionMaterial.SetFloat("_Cutoff", 0f);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (isTransitioning)
        {
            // Render the transition effect
            Graphics.Blit(renderTexture, destination, transitionMaterial);
        }
        else
        {
            // Pass the source texture directly to the destination
            Graphics.Blit(source, destination);
        }
    }
}
