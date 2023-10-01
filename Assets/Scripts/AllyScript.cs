using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyScript : MonoBehaviour
{
    private Animator anim;
    public float health = 50f;
    public float moveSpeed = 3f; // Speed at which the enemy moves towards the player
    public float attackRange = 1f; // Lowered attack range to 1 unit
    private Transform enemyTransform; // Reference to the nearest enemy's transform
    private bool canAttack = true; // Flag to control attack cooldown
    private float attackCooldown = 0f;
    private float timeBetweenAttacks;
    private float lastComboStepTime = 0f; // Add this variable
    private int comboStep = 0; // Add this variable
    private float comboCooldown = 2f; // Adjust the cooldown time as needed
    private float minComboCooldown = .2f; // Minimum combo cooldown time (adjust as needed)
    private float maxComboCooldown = 1f; // Maximum combo cooldown time (adjust as needed)
    public GameObject ragdollPrefab;
    public float ragdollForce = 500f; // Adjust the force magnitude as needed
    public float forceRandomRange = 100f; // Adjust the random range as needed

    public float minDistanceToOtherEnemies = 2f; // Minimum distance to maintain from other enemies

    // References to colliders
    public WeaponCollision weapon;
    public GameObject WeaponObject;
    public LeftFistCollision leftFist;
    public GameObject LeftFistObject;
    public RightFistCollision rightFist;
    public GameObject RightFistObject;

    private void Awake()
    {
        timeBetweenAttacks = Random.Range(0.1f, 0.3f); // Adjusted time between attacks to be quicker
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        // Find the nearest enemy with the "Enemy" tag
        FindNearestEnemy();
    }

    private void Update()
    {
        if (enemyTransform == null)
        {
            // If there is no nearest enemy, try to find one
            FindNearestEnemy();
        }

        if (enemyTransform != null)
        {
            // Calculate the direction to move towards the nearest enemy
            Vector3 directionToEnemy = (enemyTransform.position - transform.position).normalized;

            // Create a new direction vector with Y set to zero (horizontal direction)
            Vector3 horizontalDirection = new Vector3(directionToEnemy.x, 0f, directionToEnemy.z).normalized;

            // Rotate the enemy to face the horizontal direction
            if (horizontalDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(horizontalDirection);
            }

            // Check if the enemy is within the attack range
            if (Vector3.Distance(transform.position, enemyTransform.position) <= attackRange)
            {
                // Stop moving towards the enemy and trigger the attack
                canAttack = true;

                // Check if the attack cooldown has elapsed
                if (canAttack && Time.time >= attackCooldown)
                {
                    AttackEnemy();
                }
            }
            else
            {
                // If the enemy is not in attack range, move towards the enemy while avoiding other enemies
                canAttack = false;
                MoveTowardsEnemy();
            }
        }
    }

    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < nearestDistance)
            {
                nearestDistance = distanceToEnemy;
                enemyTransform = enemy.transform;
            }
        }
    }

    private void MoveTowardsEnemy()
    {
        if (enemyTransform != null)
        {
            // Calculate the direction to move towards the enemy
            Vector3 directionToEnemy = (enemyTransform.position - transform.position).normalized;

            // Move the enemy towards the enemy
            transform.Translate(directionToEnemy * moveSpeed * Time.deltaTime, Space.World);

            // Avoid standing too close to other enemies
            AvoidOtherEnemies();
        }
    }

    private void AvoidOtherEnemies()
    {
        if (enemyTransform != null)
        {
            // Find all enemies with the "Enemy" tag
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                if (enemy != gameObject && enemy.transform != enemyTransform) // Skip the current enemy and the target enemy
                {
                    // Calculate the distance to the other enemy
                    float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

                    if (distanceToEnemy < minDistanceToOtherEnemies)
                    {
                        // Calculate a direction away from the other enemy
                        Vector3 avoidDirection = (transform.position - enemy.transform.position).normalized;

                        // Move the enemy away from the other enemy
                        transform.Translate(avoidDirection * moveSpeed * Time.deltaTime, Space.World);
                    }
                }
            }
        }
    }

    private void AttackEnemy()
    {
        // Check if it's time for the next combo step
        if (Time.time - lastComboStepTime >= comboCooldown)
        {
            lastComboStepTime = Time.time;
            ComboStep();
        }
    }

    private void ComboStep()
    {
        float randomValue = Random.value;

        if (comboStep == 0)
        {
            // First attack of the combo
            if (randomValue <= 0.6f) // 60% chance to perform Punch1
            {
                anim.SetTrigger("Punch");
            }
            else if (randomValue <= 0.8f) // 20% chance to perform Punch2
            {
                anim.SetTrigger("Punch2");
            }
            else
            {
                anim.SetTrigger("AirKick"); // 20% chance to perform AirKick
            }
        }

        comboStep = (comboStep + 1) % 3; // Increment comboStep and wrap around

        // Set a fixed combo cooldown timer (you can adjust this value as needed)
        comboCooldown = 0.2f; // Set to the desired cooldown time.
    }

    // Other methods for handling hits, death, etc.

    public void GetHit()
    {
        anim.SetTrigger("Hit");
        health -= 10;
        if (health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
        // Play death audio or perform other actions here

        // Spawn the ragdoll prefab at the same position as the enemy
        GameObject ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);

        // Calculate the force direction (opposite of the enemy's forward direction)
        Vector3 forceDirection = -transform.forward;

        // Apply a random force within the specified range
        float randomForceMagnitude = ragdollForce + Random.Range(-forceRandomRange, forceRandomRange);
        Vector3 appliedForce = forceDirection * randomForceMagnitude;

        // Get all rigidbodies in the ragdoll
        Rigidbody[] ragdollRigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();

        // Apply the force to each rigidbody in the ragdoll
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.AddForce(appliedForce);
        }
    }

    public void HitEvent()
    {
        WeaponObject.SetActive(true);
    }

    public void HitDisable()
    {
        WeaponObject.SetActive(false);
    }

    public void HitEventRightFist()
    {
        RightFistObject.SetActive(true);
    }

    public void HitDisableRightFist()
    {
        RightFistObject.SetActive(false);
    }

    public void HitEventLeftFist()
    {
        LeftFistObject.SetActive(true);
    }

    public void HitDisableLeftFist()
    {
        LeftFistObject.SetActive(false);
    }
}
