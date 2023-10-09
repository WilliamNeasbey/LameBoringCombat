using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AllyScript : MonoBehaviour
{
    private Animator anim;
    public float health = 50f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float attackRange = 1f;
    private Transform enemyTransform;
    private bool canAttack = true;
    private float attackCooldown = 0f;
    private float timeBetweenAttacks;
    private float lastComboStepTime = 0f;
    private int comboStep = 0;
    private float comboCooldown = 2f;
    private float minComboCooldown = .2f;
    private float maxComboCooldown = 1f;
    public GameObject ragdollPrefab;
    public float ragdollForce = 500f;
    public float forceRandomRange = 100f;
    public float minDistanceToOtherEnemies = 2f;

    // References to colliders
    public WeaponCollision weapon;
    public GameObject WeaponObject;
    public LeftFistCollision leftFist;
    public GameObject LeftFistObject;
    public RightFistCollision rightFist;
    public GameObject RightFistObject;

    private Transform nearestEnemyTransform;
    private NavMeshAgent navMeshAgent;

    private float enemyDetectionRange = 10f; // Adjust this value to control the range at which enemies are detected
    public Transform playerTransform; // Reference to the player's transform
    private float followDistance = 2f; // Adjust this value to control the distance behind the player
    private AllyState currentState = AllyState.Idle;

    private bool hasLost = false;
    private bool isPlayerAlive = true;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        timeBetweenAttacks = Random.Range(0.2f, 0.5f);
    }

    

    private void Start()
    {
        anim = GetComponent<Animator>();
       // StartCoroutine(FindNearestEnemyRoutine());
    }


    public enum AllyState
    {
        Idle,       // The ally is not doing anything
        MovingToPlayer,  // The ally is moving towards the player
        MovingToEnemy,   // The ally is moving towards an enemy
        Attacking   // The ally is attacking an enemy
    }


    private void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearestEnemy = FindNearestEnemy(enemies);

        switch (currentState)
        {
            case AllyState.Idle:
                if (nearestEnemy != null)
                {
                    currentState = AllyState.MovingToEnemy;
                    MoveTowardsEnemy(nearestEnemy);
                }
                else if (playerTransform != null)
                {
                    currentState = AllyState.MovingToPlayer;
                    FollowPlayer();
                }
                break;

            case AllyState.MovingToPlayer:
                if (playerTransform != null)
                {
                    FollowPlayer();

                    // Check if there are enemies in range
                    if (nearestEnemy != null && Vector3.Distance(transform.position, nearestEnemy.position) <= attackRange)
                    {
                        currentState = AllyState.Attacking;
                    }
                }
                else
                {
                    currentState = AllyState.Idle;
                }
                break;

            case AllyState.MovingToEnemy:
                if (nearestEnemy != null)
                {
                    MoveTowardsEnemy(nearestEnemy);

                    // Check if the enemy is in attack range
                    if (Vector3.Distance(transform.position, nearestEnemy.position) <= attackRange)
                    {
                        currentState = AllyState.Attacking;
                    }
                }
                else
                {
                    currentState = AllyState.Idle;
                }
                break;

            case AllyState.Attacking:
                if (nearestEnemy != null)
                {
                    AttackEnemy(nearestEnemy);
                }
                else
                {
                    currentState = AllyState.Idle;
                }
                break;
        }
        // Check if the player object is destroyed and the "Lose" animation hasn't been triggered yet
        if (playerTransform == null && !hasLost)
        {
            // Set the flag to true to indicate that the "Lose" animation has been triggered
            hasLost = true;

            // Disable the NavMeshAgent to stop the ally from moving
            navMeshAgent.enabled = false;

            // Player is destroyed, trigger the "Lose" animation once
            TriggerLoseAnimationOnce();
            // Disable the AllyScript component to stop the ally from attacking
            enabled = false;
        }
    }






    private Transform FindNearestEnemy(GameObject[] enemies)
    {
        Transform nearestEnemy = null;
        float nearestDistance = enemyDetectionRange;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy <= nearestDistance)
            {
                nearestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    private void FollowPlayer()
    {
        if (playerTransform != null)
        {
            // Calculate the direction from the player to the ally
            Vector3 playerToAlly = -playerTransform.forward;

            // Calculate the target position behind the player
            Vector3 targetPosition = playerTransform.position + playerToAlly * followDistance;

            // Use NavMeshAgent to move the ally to the target position
            navMeshAgent.SetDestination(targetPosition);

            // Reset the target enemy transform
            enemyTransform = null;
        }
    }
    private void MoveTowardsEnemy(Transform enemy)
    {
        if (enemy != null && isPlayerAlive)
        {
            // Calculate the direction to move towards the enemy
            Vector3 directionToEnemy = (enemy.position - transform.position).normalized;

            // Use NavMeshAgent for ally movement
            navMeshAgent.SetDestination(enemy.position);

            // Avoid standing too close to other enemies
            AvoidOtherEnemies();

            // Check the distance to the enemy
            if (Vector3.Distance(transform.position, enemy.position) <= attackRange)
            {
                currentState = AllyState.Attacking;
            }
        }
        else
        {
            currentState = AllyState.Idle;
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

                        // Move the ally away from the other enemy
                        transform.Translate(avoidDirection * moveSpeed * Time.deltaTime, Space.World);
                    }
                }
            }
        }
    }

    private void AttackEnemy(Transform enemy)
    {
        // Check if it's time for the next combo step
        if (Time.time - lastComboStepTime >= comboCooldown)
        {
            lastComboStepTime = Time.time;
            ComboStep();
        }

        // Check if the enemy is still in range
        if (enemy != null && Vector3.Distance(transform.position, enemy.position) > attackRange)
        {
            currentState = AllyState.Idle;
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
        comboCooldown = 0.3f;
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

    private void TriggerLoseAnimationOnce()
    {
        // Trigger the "Lose" animation
        anim.SetTrigger("Lose");
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
