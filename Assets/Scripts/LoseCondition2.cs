using UnityEngine;

public class LoseCondition2 : MonoBehaviour
{
    public GameObject BattleUI; // Reference to the Battle UI GameObject
    public GameObject Cameras; // Reference to the PlayerCameras
    public GameObject HitSound; // Reference to the HitSound because it gets annoying
    public GameObject DeathUI;  // Reference to the Death UI GameObject
    public GameObject player;   // Reference to the player GameObject 
    public GameObject Wincondition; // Reference to the Wincondition
    public GameObject ally; // Reference to the ally GameObject

    private TacticalMode playerTacticalMode; // Reference to the TacticalMode script on the player
    private CharacterMovement playerMovement; // Reference to the CharacterMovement script on the player

    private void Start()
    {
        // Make sure both UI elements start in the desired state
        BattleUI.SetActive(true);
        DeathUI.SetActive(false);
        Cameras.SetActive(true);
        HitSound.SetActive(true);
        Wincondition.SetActive(true);

        // Get the TacticalMode and CharacterMovement scripts from the player
        playerTacticalMode = player.GetComponent<TacticalMode>();
        playerMovement = player.GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        // Check if the ally GameObject is destroyed
        if (ally == null)
        {
            // Ally is destroyed, so enable the Death UI and disable the Battle UI
            BattleUI.SetActive(false);
            Cameras.SetActive(false);
            HitSound.SetActive(false);
            Wincondition.SetActive(false);
            DeathUI.SetActive(true);

            // Disable the player's movement and tactical mode scripts
            if (playerTacticalMode != null)
            {
                playerTacticalMode.enabled = false;
            }

            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }
    }
}
