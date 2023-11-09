using UnityEngine;
using TMPro;

public class LoseConditionSurvivalMode : MonoBehaviour
{
    public GameObject BattleUI; // Reference to the Battle UI GameObject
    public GameObject SurvivalModeUI; // Reference to the Syrvuval UI GameObject
    public GameObject Cameras; // Reference to the PlayerCameras
    public GameObject HitSound; // Reference to the HitSound because it gets annoying
    public GameObject DeathUI;  // Reference to the Death UI GameObject
    public GameObject player;   // Reference to the player GameObject (the one to be destroyed)
    public GameObject Wincondition; // Reference to the Wincondition
    public GameObject LevelMusic; // Reference to the LevelMusic
    public GameObject gameOverMusic; // Reference to the Gameover music
    public TextMeshProUGUI scoreText; // Reference to the TextMeshPro UI for displaying the player's score
    public TextMeshProUGUI highScoreText; // Reference to the TextMeshPro UI for displaying the high score
    public TextMeshProUGUI randomQuoteText; // Reference to the TextMeshPro UI for displaying random quotes
    private bool quoteDisplayed = false;

    private bool gameEnded = false; // Flag to ensure the game over logic is executed only once

     string[] randomQuotes = {
    "Don't cry over spilled milk; respawn and chug a jug of victory!",
    //"A grenade in hand is worth two in the bush. Or something like that.",
   // "When life gives you lemons, throw them back and ask for a better killstreak.",
    //"In the darkest moments, remember: you're not camping; you're 'tactical waiting.'",
    //"It's not about the KD ratio; it's about the journey to unlock that golden weapon skin.",
    //"If at first you don't succeed, blame lag and try again.",
   // "Keep your friends close and your noobs closer. Teach 'em the way of the game.",
   // "There are two types of people: those who quickscope and those who complain about quickscopers.",
    //"Sometimes you win, sometimes you learn... how to rage-quit with style.",
   // "Aim for the stars, but watch out for that camper in the corner!",
   // "In Fortnite, we dance through the storm and glide like One Piece's 4Kids dub - filled with excitement and absolutely no awkward silences!",
  //  "When the night is dark and the animatronics are lurking, remember, 'You are the true Nightguard of your own destiny!' - Freddy Fazbear.",
    "Just as Luffy searched for the One Piece, you too can find your Victory Royale. Believe in the heart of the battle bus!",
    //"In the world of Undertale, it's always time to go 'sans' excuses and 'sans' hesitation - be determined like Frisk!",
    //"When things get spooky, remember, just like in Five Nights at Freddy's, there's always a 'light' at the end of the tunnel, even if it's in the form of a flashlight!",
    "Undertale's Sans once said, 'You gotta have a bad time,' but don't let that discourage you - reset, try again, and have a 'good time' instead!",
    "In the world of Fortnite, even the 'default skins' can become legends. You're just a few Victory Royales away from becoming a true Fortnite icon!",
   // "Just like Chun Li's kicks, keep your moves lightning fast!",
    "When the going gets tough, hit the griddy and dance your way to victory!",
   // "Remember, in gaming and life, you're as strong as Chun Li's spinning bird kick!",
   "Life's challenges are like Chug Jugs - take them head-on, and you'll come out refreshed and victorious.",
   "Just like in Fortnite, where every game starts with a drop, life begins with a leap of faith. Take the plunge and chase your Victory Royale!",
   "In the 'Dilemma' of life, my focus remains on my dreams. Even when I'm with my 'boo,' my determination knows no bounds.",
   "No matter how big it is It's time will come It will be reborn as something new",

    // Add more quotes related to your preferences
};



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
        if (player == null)
        {
            if (!quoteDisplayed) // Only display the quote if it hasn't been displayed yet
            {
                // Player is destroyed, so enable the Death UI and disable the Battle UI
                BattleUI.SetActive(false);
                Cameras.SetActive(false);
                HitSound.SetActive(false);
                Wincondition.SetActive(false);
                LevelMusic.SetActive(false);
                DeathUI.SetActive(true);
                gameOverMusic.SetActive(true);
                scoreText.gameObject.SetActive(true);
                highScoreText.gameObject.SetActive(true);

                // Get the player's score from the PointsCounter script (replace "PointsCounter" with your actual script name)
                int playerScore = FindObjectOfType<PointCounter>().points;

                // Get the current high score
                int highScore = PlayerPrefs.GetInt("survivalhighscore", 0);

                // Set the score text to display the player's score and the high score
                scoreText.text = "Your Score: " + playerScore;
                highScoreText.text = "High Score: " + highScore;

                // Display a random quote when the game ends
                DisplayRandomQuote();

                // Set quoteDisplayed to true to prevent constant changes
                quoteDisplayed = true;
            }
        }
    }

    // Function to display a random quote
    private void DisplayRandomQuote()
    {
        if (randomQuotes.Length > 0)
        {
            // Choose a random quote from the array
            int randomIndex = Random.Range(0, randomQuotes.Length);
            string randomQuote = randomQuotes[randomIndex];

            // Display the random quote
            randomQuoteText.text = randomQuote;
            randomQuoteText.gameObject.SetActive(true);
        }
    }

}
