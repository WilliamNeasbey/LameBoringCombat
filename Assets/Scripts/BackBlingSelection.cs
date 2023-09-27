using UnityEngine;

public class BackBlingSelection : MonoBehaviour
{
    public GameObject[] backBlingOptions; // Reference to an array of back bling objects in the Hierarchy.
    private const string PlayerPrefsKey = "SelectedBackBling"; // PlayerPrefs key to store the selected back bling option.

    // Function to handle the button press and set the selected back bling option.
    public void SetSelectedBackBling(int backBlingOption)
    {
        PlayerPrefs.SetInt(PlayerPrefsKey, backBlingOption);
        PlayerPrefs.Save();

        // Loop through all back bling options and enable/disable accordingly.
        for (int i = 0; i < backBlingOptions.Length; i++)
        {
            bool isSelected = (i == backBlingOption);
            backBlingOptions[i].SetActive(isSelected);
        }
    }

    // Initialize the back bling objects based on the PlayerPrefs value (e.g., in Start() method).
    private void Start()
    {
        int selectedBackBling = PlayerPrefs.GetInt(PlayerPrefsKey, 0); // Default to no back bling if PlayerPrefs is not set.
        SetSelectedBackBling(selectedBackBling);
    }
}
