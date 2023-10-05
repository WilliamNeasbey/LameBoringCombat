using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class RenderTextureSwitcher : MonoBehaviour
{
    public RawImage[] images; // An array of Raw Images to crossfade
    public float crossfadeDuration = 2.0f; // Duration of the crossfade in seconds
    public float interval = 5.0f; // Time interval between crossfades in seconds

    private int currentIndex = 0;
    private bool isCrossfading = false;
    private float nextCrossfadeTime;

    private void Awake()
    {
        // Initialize the images
        if (images == null || images.Length < 2)
        {
            Debug.LogError("At least two Raw Images must be assigned in the Inspector.");
            enabled = false; // Disable the script if not enough Raw Images are assigned.
            return;
        }
    }

    private void Start()
    {
        // Calculate the time for the first crossfade
        nextCrossfadeTime = Time.time + interval;

        // Start the initial crossfade
        Crossfade();
    }

    private void Update()
    {
        // If not currently crossfading, check if it's time for the next crossfade
        if (!isCrossfading && Time.time >= nextCrossfadeTime)
        {
            Crossfade();
        }
    }

    private void Crossfade()
    {
        // Determine the next index for crossfade
        int nextIndex = (currentIndex + 1) % images.Length;

        // Start the crossfade effect
        StartCoroutine(CrossfadeCoroutine(currentIndex, nextIndex));
        currentIndex = nextIndex; // Update the current index

        // Calculate the time for the next crossfade
        nextCrossfadeTime = Time.time + interval;
    }

    private IEnumerator CrossfadeCoroutine(int fromIndex, int toIndex)
    {
        isCrossfading = true;

        RawImage fromImage = images[fromIndex];
        RawImage toImage = images[toIndex];

        float startTime = Time.time;
        float endTime = startTime + crossfadeDuration;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float alpha = elapsedTime / crossfadeDuration;

            // Set the alpha of both images to create the crossfade effect
            fromImage.color = new Color(fromImage.color.r, fromImage.color.g, fromImage.color.b, 1 - alpha);
            toImage.color = new Color(toImage.color.r, toImage.color.g, toImage.color.b, alpha);

            yield return null;
        }

        // Ensure a smooth transition by setting final alpha values
        fromImage.color = new Color(fromImage.color.r, fromImage.color.g, fromImage.color.b, 0);
        toImage.color = new Color(toImage.color.r, toImage.color.g, toImage.color.b, 1);

        isCrossfading = false;
    }
}
