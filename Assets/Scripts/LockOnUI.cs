using UnityEngine;

public class LockOnUI : MonoBehaviour
{
    private Transform target; // The locked-on target's transform

    public Vector3 positionOffset = new Vector3(0, 0, 0); // Offset from the target's position
    public Vector3 lookAtOffset = new Vector3(0, 0, 0); // Offset for making the UI face the camera

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
            // Update the UI position with the offset
            transform.position = target.position + positionOffset;

            // Make the UI always face the camera with the offset
            transform.LookAt(Camera.main.transform.position + lookAtOffset);
        }
    }
}
