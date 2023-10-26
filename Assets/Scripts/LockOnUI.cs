using UnityEngine;

public class LockOnUI : MonoBehaviour
{
    private Transform target; // The locked-on target's transform

    // Assign the target (locked-on enemy) for this UI element
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // LateUpdate is called once per frame, after Update
    void LateUpdate()
    {
        if (target != null)
        {
            // Update the UI position to match the locked-on target's position
            transform.position = target.position;

            // Make the UI always face the camera
            transform.LookAt(Camera.main.transform);
        }
    }
}
