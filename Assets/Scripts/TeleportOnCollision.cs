using UnityEngine;

public class TeleportOnCollision : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        
            player.transform.position = respawnPoint.transform.position;
            Physics.SyncTransforms();
        
    }
}
