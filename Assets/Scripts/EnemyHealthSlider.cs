using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthSlider : MonoBehaviour
{
    public Slider healthSlider; // Reference to your UI Slider
    public float maxHealth = 100f; // The maximum health value (should match the max health of your player)

    // Reference to the script containing the player's health variable
    public EnemyScript EnemyHealthScript;

    private void Start()
    {
        // Ensure the healthSlider and playerHealthScript references are set correctly
        if (healthSlider == null || EnemyHealthScript == null)
        {
            Debug.LogError("Ensure that healthSlider and playerHealthScript references are set in the Inspector.");
            return;
        }

        
        // Get the initial max health value from the attached enemy script
        maxHealth = EnemyHealthScript.health;

        // Set the maximum value of the Slider to match the enemy's max health
        healthSlider.maxValue = maxHealth;
    }

    private void Update()
    {
        // Update the Slider's value to match the player's current health
        healthSlider.value = EnemyHealthScript.health;
    }
}
