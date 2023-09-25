using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RightFistCollisionEnemyEvent : UnityEvent<Transform> { }

public class RightFistCollisionEnemy : MonoBehaviour
{
    public RightFistCollisionEvent onHit;

    private TacticalMode tacticalMode; // Reference to the TacticalMode script
    public ParticleSystem collisionParticlePrefab; // Reference to the particle system prefab
    public AudioSource collisionSound; // Reference to the audio source

    private void Start()
    {
        // Find and store the TacticalMode script on an object in your scene (e.g., the player)
        tacticalMode = FindObjectOfType<TacticalMode>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onHit.Invoke(other.transform);
            // Trigger the GetHit method in the TacticalMode script
            tacticalMode.GetHit(10f); // Pass the desired damage amount (e.g., 10) as an argument

            // Instantiate the collision particle effect at the collision location
            if (collisionParticlePrefab != null)
            {
                Instantiate(collisionParticlePrefab, transform.position, Quaternion.identity);
            }

            // Play the collision sound
            if (collisionSound != null)
            {
                collisionSound.Play();
            }
        }
    }
}
