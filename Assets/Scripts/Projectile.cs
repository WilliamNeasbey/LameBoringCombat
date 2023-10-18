using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;

    // Set the target for the projectile
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        // Check if we have a target
        if (target != null)
        {
            // Calculate the direction to the target
            Vector3 direction = (target.position - transform.position).normalized;

            // Move the projectile towards the target
            transform.Translate(direction * speed * Time.deltaTime);

            // Optionally, you can rotate the projectile to face the target using Quaternion.LookRotation
            transform.rotation = Quaternion.LookRotation(direction);

            // Check if the projectile has reached the target
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget < 0.1f)
            {
                // The projectile has hit the target
                // You can add damage logic here or destroy the projectile
                Destroy(gameObject);
            }
        }
        else
        {
            // If the target is null (for example, the target is destroyed), destroy the projectile
           // Destroy(gameObject);
        }
    }
}
