using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    private Animator anim;
    public float health = 50f;
    public float moveSpeed = 3f; // Speed at which the enemy moves towards the player
    public float attackRange = 1f; // Lowered attack range to 1 unit
    private Transform playerTransform;
    private Transform allyTransform;
    private bool canAttack = true; // Flag to control attack cooldown
    private float attackCooldown = 0f;
    private float timeBetweenAttacks;
    private float lastComboStepTime = 0f; // Add this variable
    private int comboStep = 0; // Add this variable
    private float comboCooldown = 2f; // Adjust the cooldown time as needed
    private float minComboCooldown = .2f; // Minimum combo cooldown time (adjust as needed)
    private float maxComboCooldown = 1f; // Maximum combo cooldown time (adjust as needed)
    private NavMeshAgent navMeshAgent;
    public GameObject ragdollPrefab;
    public float ragdollForce = 500f; // Adjust the force magnitude as needed
    public float forceRandomRange = 100f; // Adjust the random range as needed

    public float minDistanceToOtherEnemies = 2f; // Minimum distance to maintain from other enemies

    // References to  colliders 
    public WeaponCollisionEnemy weapon;
    public GameObject WeaponObject;
    public LeftFistCollisionEnemy leftFist;
    public GameObject LeftFistObject;
    public RightFistCollisionEnemy rightFist;
    public GameObject RightFistObject;

    private void Awake()
    {
        timeBetweenAttacks = Random.Range(0.2f, 0.5f); // Adjusted time between attacks to be quicker
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        allyTransform = GameObject.FindGameObjectWithTag("Ally").transform;
    }


    private void Update()
    {
        if (playerTransform != null && allyTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            float distanceToAlly = Vector3.Distance(transform.position, allyTransform.position);

            Transform targetTransform = null;

            if (distanceToAlly < distanceToPlayer)
            {
                targetTransform = allyTransform;
            }
            else
            {
                targetTransform = playerTransform;
            }

            if (targetTransform != null)
            {
                if (Vector3.Distance(transform.position, targetTransform.position) <= attackRange)
                {
                    // Stop moving
                    navMeshAgent.isStopped = true;

                    // Rotate to face the target
                    transform.LookAt(targetTransform);

                    // Attack the target
                    AttackPlayer();
                }
                else
                {
                    // Player/ally is not in attack range, so move towards the target
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(targetTransform.position);

                    // Rotate to face the movement direction
                    transform.LookAt(targetTransform);
                }
            }
        }
    }


    private void MoveTowardsPlayer()
    {
        // Calculate the direction to move towards the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Move the enemy towards the player
        transform.Translate(directionToPlayer * moveSpeed * Time.deltaTime, Space.World);

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

        // Set a random combo cooldown timer
        comboCooldown = Random.Range(minComboCooldown, maxComboCooldown);
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
