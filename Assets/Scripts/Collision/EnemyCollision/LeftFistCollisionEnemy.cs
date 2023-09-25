using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LeftFistCollisionEnemyEvent : UnityEvent<Transform> { }

public class LeftFistCollisionEnemy : MonoBehaviour
{
    public LeftFistCollisionEvent onHit;

    private TacticalMode tacticalMode; // Reference to the TacticalMode script

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
        }
    }
}
