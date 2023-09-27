using UnityEngine;

public class LeftHandAccessorySelection : MonoBehaviour
{
    public GameObject[] leftHandAccessoryOptions; // Reference to an array of left-hand accessory objects in the Hierarchy.
    private const string PlayerPrefsKey = "SelectedLeftHandAccessory"; // PlayerPrefs key to store the selected left-hand accessory option.

    // Function to handle the button press and set the selected left-hand accessory option.
    public void SetSelectedLeftHandAccessory(int leftHandAccessoryOption)
    {
        PlayerPrefs.SetInt(PlayerPrefsKey, leftHandAccessoryOption);
        PlayerPrefs.Save();

        // Loop through all left-hand accessory options and enable/disable accordingly.
        for (int i = 0; i < leftHandAccessoryOptions.Length; i++)
        {
            bool isSelected = (i == leftHandAccessoryOption);
            leftHandAccessoryOptions[i].SetActive(isSelected);
        }
    }

    // Initialize the left-hand accessory objects based on the PlayerPrefs value (e.g., in Start() method).
    private void Start()
    {
        int selectedLeftHandAccessory = PlayerPrefs.GetInt(PlayerPrefsKey, 0); // Default to no left-hand accessory if PlayerPrefs is not set.
        SetSelectedLeftHandAccessory(selectedLeftHandAccessory);
    }
}
