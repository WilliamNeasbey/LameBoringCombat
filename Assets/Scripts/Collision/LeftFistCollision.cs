using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LeftFistCollisionEvent : UnityEvent<Transform> { }

public class LeftFistCollision : MonoBehaviour
{
    public LeftFistCollisionEvent onHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            onHit.Invoke(other.transform);
        }
    }
}
