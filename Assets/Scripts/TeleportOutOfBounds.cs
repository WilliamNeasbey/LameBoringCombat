using UnityEngine;

public class TeleportOutOfBounds : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerStay(Collider other)
    {
        // Check if the colliding object has the "Player" tag or is on the "Player" layer
        if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Teleport the player to the respawn point
            player.transform.position = respawnPoint.position;

            Debug.Log("Player teleported to respawn point.");
        }
    }
}
