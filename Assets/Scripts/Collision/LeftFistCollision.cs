using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeftFistCollision : MonoBehaviour
{
    public CollisionEvent onHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            onHit.Invoke(other.transform);
        }
    }
}
