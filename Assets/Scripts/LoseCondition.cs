using UnityEngine;

public class LoseCondition : MonoBehaviour
{
    public GameObject BattleUI; // Reference to the Battle UI GameObject
    public GameObject Cameras; // Reference to the PlayerCameras
    public GameObject HitSound; // Reference to the HitSound because it gets annoying
    public GameObject DeathUI;  // Reference to the Death UI GameObject
    public GameObject player;   // Reference to the player GameObject (the one to be destroyed)

    private void Start()
    {
        // Make sure both UI elements start in the desired state
        BattleUI.SetActive(true);
        DeathUI.SetActive(false);
        Cameras.SetActive(true);
        HitSound.SetActive(true);
    }

    private void Update()
    {
        // Check if the player GameObject is destroyed
        if (player == null)
        {
            // Player is destroyed, so enable the Death UI and disable the Battle UI
            BattleUI.SetActive(false);
            Cameras.SetActive(false);
            HitSound.SetActive(false);
            DeathUI.SetActive(true);
        }
    }
}
