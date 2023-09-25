using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RightFistCollisionEvent : UnityEvent<Transform> { }

public class RightFistCollision : MonoBehaviour
{
    public RightFistCollisionEvent onHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            onHit.Invoke(other.transform);
        }
    }
}
