using UnityEngine;

public class HatSelection : MonoBehaviour
{
    public GameObject noHat; // Reference to the character without a hat.
    public GameObject hat1; // Reference to the first hat object in the Hierarchy.
    public GameObject hat2; // Reference to the second hat object in the Hierarchy.
    public GameObject hat3; // Reference to the third hat object in the Hierarchy.
    public GameObject hat4; // Reference to the fourth hat object in the Hierarchy.

    private const string PlayerPrefsKey = "SelectedHat"; // PlayerPrefs key to store the selected hat option.

    // Function to handle the button press and set the selected hat option.
    public void SetSelectedHat(int hatOption)
    {
        PlayerPrefs.SetInt(PlayerPrefsKey, hatOption);
        PlayerPrefs.Save();

        // Check which hat option was selected and enable/disable accordingly.
        noHat.SetActive(hatOption == 0);
        hat1.SetActive(hatOption == 1);
        hat2.SetActive(hatOption == 2);
        hat3.SetActive(hatOption == 3);
        hat4.SetActive(hatOption == 4);
    }

    // Initialize the hat objects based on the PlayerPrefs value (e.g., in Start() method).
    private void Start()
    {
        int selectedHat = PlayerPrefs.GetInt(PlayerPrefsKey, 0); // Default to no hat if PlayerPrefs is not set.
        SetSelectedHat(selectedHat);
    }
}
