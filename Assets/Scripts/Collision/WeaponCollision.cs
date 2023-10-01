using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[System.Serializable] public class CollisionEvent : UnityEvent<Transform> { }
public class WeaponCollision : MonoBehaviour
{

    public RightFistCollisionEvent onHit;
    public ParticleSystem collisionParticlePrefab; // Reference to the particle system prefab
    public AudioSource collisionSound; // Reference to the audio source

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.CompareTag("Enemy"))
        {
            onHit.Invoke(other.transform);
        }
        */
        if (other.CompareTag("Enemy"))
        {
            onHit.Invoke(other.transform);
            // Call the GetHit method of the AllyScript to apply damage to the ally
            EnemyScript ally = other.GetComponent<EnemyScript>();
            if (ally != null)
            {
                ally.GetHit();
            }
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
