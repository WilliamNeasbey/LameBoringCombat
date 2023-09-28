using UnityEngine;

public class RightHandAccessorySelection : MonoBehaviour
{
    public GameObject[] rightHandAccessoryOptions; // Reference to an array of right-hand accessory objects in the Hierarchy.
    private const string PlayerPrefsKey = "SelectedRightHandAccessory"; // PlayerPrefs key to store the selected right-hand accessory option.

    // Function to handle the button press and set the selected right-hand accessory option.
    public void SetSelectedRightHandAccessory(int rightHandAccessoryOption)
    {
        PlayerPrefs.SetInt(PlayerPrefsKey, rightHandAccessoryOption);
        PlayerPrefs.Save();

        // Loop through all right-hand accessory options and enable/disable accordingly.
        for (int i = 0; i < rightHandAccessoryOptions.Length; i++)
        {
            bool isSelected = (i == rightHandAccessoryOption);
            rightHandAccessoryOptions[i].SetActive(isSelected);
        }
    }

    // Initialize the right-hand accessory objects based on the PlayerPrefs value (e.g., in Start() method).
    private void Start()
    {
        int selectedRightHandAccessory = PlayerPrefs.GetInt(PlayerPrefsKey, 0); // Default to no right-hand accessory if PlayerPrefs is not set.
        SetSelectedRightHandAccessory(selectedRightHandAccessory);
    }
}
