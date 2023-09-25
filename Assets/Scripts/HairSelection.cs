using UnityEngine;

public class HairSelection : MonoBehaviour
{
    public GameObject hair1; // Reference to the first hair object in the Hierarchy.
    public GameObject hair2; // Reference to the second hair object in the Hierarchy.

    private const string PlayerPrefsKey = "SelectedHair"; // PlayerPrefs key to store the selected hair option.

    // Function to handle the button press and set the selected hair option.
    public void SetSelectedHair(int hairOption)
    {
        PlayerPrefs.SetInt(PlayerPrefsKey, hairOption);
        PlayerPrefs.Save();

        // Check which hair option was selected and enable/disable accordingly.
        if (hairOption == 1)
        {
            hair1.SetActive(true);
            hair2.SetActive(false);
        }
        else if (hairOption == 2)
        {
            hair1.SetActive(false);
            hair2.SetActive(true);
        }
    }

    // Initialize the hair objects based on the PlayerPrefs value (e.g., in Start() method).
    private void Start()
    {
        int selectedHair = PlayerPrefs.GetInt(PlayerPrefsKey, 1); // Default to hair1 if PlayerPrefs is not set.
        SetSelectedHair(selectedHair);
    }
}
