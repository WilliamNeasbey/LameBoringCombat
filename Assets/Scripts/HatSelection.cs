using UnityEngine;

public class HatSelection : MonoBehaviour
{
    public GameObject[] hatOptions; // Reference to an array of hat objects in the Hierarchy.
    private const string PlayerPrefsKey = "SelectedHat"; // PlayerPrefs key to store the selected hat option.

    // Function to handle the button press and set the selected hat option.
    public void SetSelectedHat(int hatOption)
    {
        PlayerPrefs.SetInt(PlayerPrefsKey, hatOption);
        PlayerPrefs.Save();

        // Loop through all hat options and enable/disable accordingly.
        for (int i = 0; i < hatOptions.Length; i++)
        {
            bool isSelected = (i == hatOption);
            hatOptions[i].SetActive(isSelected);
        }
    }

    // Initialize the hat objects based on the PlayerPrefs value (e.g., in Start() method).
    private void Start()
    {
        int selectedHat = PlayerPrefs.GetInt(PlayerPrefsKey, 0); // Default to no hat if PlayerPrefs is not set.
        SetSelectedHat(selectedHat);
    }
}
