using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Animator anim;
    public float health = 50f;
    public float moveSpeed = 3f; // Speed at which the enemy moves towards the player
    public float attackRange = 1f; // Lowered attack range to 1 unit
    private Transform playerTransform;
    private bool canAttack = true; // Flag to control attack cooldown
    private float attackCooldown = 0f;
    private float timeBetweenAttacks;
    private float lastComboStepTime = 0f; // Add this variable
    private int comboStep = 0; // Add this variable
    private float comboCooldown = 2f; // Adjust the cooldown time as needed


    public float minDistanceToOtherEnemies = 2f; // Minimum distance to maintain from other enemies

    private void Awake()
    {
        timeBetweenAttacks = Random.Range(0.5f, 2f); // Random time between 0.5 to 2 seconds for attacks
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Check if the player is within the attack range
        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            // Stop moving towards the player and trigger the attack
            canAttack = true;

            // Check if the attack cooldown has elapsed
            if (canAttack && Time.time >= attackCooldown)
            {
                AttackPlayer();
            }
        }
        else
        {
            // If the player is not in attack range, move towards the player while avoiding other enemies
            canAttack = false;
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        // Calculate the direction to move towards the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Move the enemy towards the player
        transform.Translate(directionToPlayer * moveSpeed * Time.deltaTime, Space.World);

        // Rotate the enemy to face the player (optional)
        transform.LookAt(playerTransform);

        // Avoid standing too close to other enemies
        AvoidOtherEnemies();
    }

    private void AvoidOtherEnemies()
    {
        // Find all enemies with the "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy != gameObject) // Skip the current enemy
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

    private void AttackPlayer()
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

        // Set the attack cooldown timer
        attackCooldown = Time.time + timeBetweenAttacks;
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
    }
}
