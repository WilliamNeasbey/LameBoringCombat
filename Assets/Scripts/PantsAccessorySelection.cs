using UnityEngine;

public class PantsAccessorySelection : MonoBehaviour
{
    public GameObject[] pantsAccessoryOptions; // Reference to an array of pants accessory objects in the Hierarchy.
    private const string PlayerPrefsKey = "SelectedPantsAccessory"; // PlayerPrefs key to store the selected pants accessory option.

    // Function to handle the button press and set the selected pants accessory option.
    public void SetSelectedPantsAccessory(int pantsAccessoryOption)
    {
        PlayerPrefs.SetInt(PlayerPrefsKey, pantsAccessoryOption);
        PlayerPrefs.Save();

        // Loop through all pants accessory options and enable/disable accordingly.
        for (int i = 0; i < pantsAccessoryOptions.Length; i++)
        {
            bool isSelected = (i == pantsAccessoryOption);
            pantsAccessoryOptions[i].SetActive(isSelected);
        }
    }

    // Initialize the pants accessory objects based on the PlayerPrefs value (e.g., in Start() method).
    private void Start()
    {
        int selectedPantsAccessory = PlayerPrefs.GetInt(PlayerPrefsKey, 0); // Default to no pants accessory if PlayerPrefs is not set.
        SetSelectedPantsAccessory(selectedPantsAccessory);
    }
}
