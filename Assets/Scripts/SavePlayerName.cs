using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SavePlayerName : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public Button saveButton;

    private const string PlayerPrefsKey = "playername";
    private const int MaxCharacterLimit = 33;

    private void Start()
    {
        // Check if player name is already saved and populate the TMP input field.
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string savedPlayerName = PlayerPrefs.GetString(PlayerPrefsKey);
            playerNameInput.text = savedPlayerName;
        }

        // Set the character limit for the TMP input field.
        playerNameInput.characterLimit = MaxCharacterLimit;

        // Add an event listener to the TMP input field to enable/disable the save button.
        playerNameInput.onValueChanged.AddListener(OnPlayerNameChanged);
        OnPlayerNameChanged(playerNameInput.text); // Initially check the input field.

        // Add an event listener to the save button to save the player name.
        saveButton.onClick.AddListener(SavePlayerNameToPrefs);
    }

    private void OnPlayerNameChanged(string text)
    {
        // Enable the save button only if there's at least one character in the TMP input field.
        saveButton.interactable = !string.IsNullOrEmpty(text);
    }

    private void SavePlayerNameToPrefs()
    {
        string playerName = playerNameInput.text;
        PlayerPrefs.SetString(PlayerPrefsKey, playerName);
        PlayerPrefs.Save();
        Debug.Log("Player name saved: " + playerName);
    }
}
