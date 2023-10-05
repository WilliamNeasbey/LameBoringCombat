using UnityEngine;
using UnityEngine.UI;

public class HealthSliderAlly : MonoBehaviour
{
    public Slider healthSlider; // Reference to your UI Slider
    public float maxHealth = 100f; // The maximum health value (should match the max health of your player)

    // Reference to the script containing the player's health variable
    public AllyScript AllyHealthScript;

    private void Start()
    {
        // Ensure the healthSlider and playerHealthScript references are set correctly
        if (healthSlider == null || AllyHealthScript == null)
        {
            Debug.LogError("Ensure that healthSlider and playerHealthScript references are set in the Inspector.");
            return;
        }

        // Set the maximum value of the Slider to match the player's max health
        healthSlider.maxValue = maxHealth;
    }

    private void Update()
    {
        // Update the Slider's value to match the player's current health
        healthSlider.value = AllyHealthScript.health;
    }
}
