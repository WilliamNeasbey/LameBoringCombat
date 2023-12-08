using UnityEngine;

public class TeleportOutOfBounds : MonoBehaviour
{
     public GameObject player;
     public Transform respawnPoint;

    private void OnTriggerStay(Collider other)
    {
        // Check if the colliding object is the player GameObject
        if (other.gameObject == player)
        {
            // Teleport the player to the respawn point
            player.transform.position = respawnPoint.position;

            Debug.Log("Player teleported to respawn point.");
        }
    } 
   
}
