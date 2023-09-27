using UnityEngine;

public class HairSelection : MonoBehaviour
{
    public GameObject[] hairOptions; // Reference to an array of hair objects in the Hierarchy.
    private const string PlayerPrefsKey = "SelectedHair"; // PlayerPrefs key to store the selected hair option.

    // Function to handle the button press and set the selected hair option.
    public void SetSelectedHair(int hairOption)
    {
        PlayerPrefs.SetInt(PlayerPrefsKey, hairOption);
        PlayerPrefs.Save();

        // Loop through all hair options and enable/disable accordingly.
        for (int i = 0; i < hairOptions.Length; i++)
        {
            bool isSelected = (i == (hairOption - 1)); // Subtract 1 to match array index.
            hairOptions[i].SetActive(isSelected);
        }
    }

    // Initialize the hair objects based on the PlayerPrefs value (e.g., in Start() method).
    private void Start()
    {
        int selectedHair = PlayerPrefs.GetInt(PlayerPrefsKey, 1); // Default to hair1 if PlayerPrefs is not set.
        SetSelectedHair(selectedHair);
    }
}
