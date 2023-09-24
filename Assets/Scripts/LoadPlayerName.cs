using UnityEngine;
using TMPro;

public class LoadPlayerName : MonoBehaviour
{
    public TMP_Text playerNameText; // Reference to your TMP Text component.
    private const string PlayerPrefsKey = "playername";

    private void Start()
    {
        // Check if the player name is saved in PlayerPrefs.
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            // Load the player name from PlayerPrefs and display it in the TMP Text component.
            string playerName = PlayerPrefs.GetString(PlayerPrefsKey);
            playerNameText.text = playerName;
        }
        else
        {
            // If the player name is not saved, you can set a default message or leave it empty.
            playerNameText.text = "Not Set"; // You can customize this message.
        }
    }
}
