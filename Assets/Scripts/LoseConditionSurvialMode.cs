using UnityEngine;
using TMPro;

public class LoseConditionSurvivalMode : MonoBehaviour
{
    public GameObject BattleUI; // Reference to the Battle UI GameObject
    public GameObject Cameras; // Reference to the PlayerCameras
    public GameObject HitSound; // Reference to the HitSound because it gets annoying
    public GameObject DeathUI;  // Reference to the Death UI GameObject
    public GameObject player;   // Reference to the player GameObject (the one to be destroyed)
    public GameObject Wincondition; // Reference to the Wincondition
    public GameObject LevelMusic; // Reference to the LevelMusic
    public GameObject gameOverMusic; // Reference to the Gameover music
    public TextMeshProUGUI scoreText; // Reference to the TextMeshPro UI for displaying the player's score
    public TextMeshProUGUI highScoreText; // Reference to the TextMeshPro UI for displaying the high score

    private bool gameEnded = false; // Flag to ensure the game over logic is executed only once

    private void Start()
    {
        // Make sure both UI elements start in the desired state
        BattleUI.SetActive(true);
        DeathUI.SetActive(false);
        Cameras.SetActive(true);
        HitSound.SetActive(true);
        Wincondition.SetActive(true);

        // Ensure the score texts are hidden initially
        scoreText.gameObject.SetActive(false);
        highScoreText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Check if the player GameObject is destroyed
        if (player == null && !gameEnded)
        {
            // Player is destroyed, so enable the Death UI and disable the Battle UI
            BattleUI.SetActive(false);
            Cameras.SetActive(false);
            HitSound.SetActive(false);
            Wincondition.SetActive(false);
            LevelMusic.SetActive(false);
            DeathUI.SetActive(true);
            gameOverMusic.SetActive(true);

            // Get the player's score from the PointsCounter script (replace "PointsCounter" with your actual script name)
            int playerScore = FindObjectOfType<PointCounter>().points;

            // Get the current high score
            int highScore = PlayerPrefs.GetInt("survivalhighscore", 0);

            // Set the score text to display the player's score and the high score
            scoreText.text = "Your Score: " + playerScore;
            highScoreText.text = "High Score: " + highScore;

            // Display the score texts
            scoreText.gameObject.SetActive(true);
            highScoreText.gameObject.SetActive(true);

            // Check if the player's score is higher than the current high score
            if (playerScore > highScore)
            {
                // Set the new high score in PlayerPrefs
                PlayerPrefs.SetInt("survivalhighscore", playerScore);
            }

            // Ensure the game over logic is executed only once
            //gameEnded = true;
        }
    }
}
