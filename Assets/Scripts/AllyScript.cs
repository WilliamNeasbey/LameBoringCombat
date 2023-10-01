using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        timeBetweenAttacks = Random.Range(0.2f, 0.5f);
    }

    private Transform nearestEnemyTransform;

    private void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(FindNearestEnemyRoutine());
    }

    private IEnumerator FindNearestEnemyRoutine()
    {
        while (true)
        {
            FindNearestEnemy();
            yield return new WaitForSeconds(1f); // Adjust the frequency of checking for the nearest enemy
        }
    }

    private void Update()
    {
        // Calculate the direction to move towards the nearest enemy
        if (enemyTransform == null)
        {
            FindNearestEnemy();
        }

        if (enemyTransform != null)
        {
            Vector3 directionToEnemy = (enemyTransform.position - transform.position).normalized;

            Vector3 horizontalDirection = new Vector3(directionToEnemy.x, 0f, directionToEnemy.z).normalized;

            if (horizontalDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(horizontalDirection);
            }

            if (Vector3.Distance(transform.position, enemyTransform.position) <= attackRange)
            {
                canAttack = true;

                if (canAttack && Time.time >= attackCooldown)
                {
                    AttackEnemy();
                }
            }
            else
            {
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

            // Move the ally towards the enemy
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

                        // Move the ally away from the other enemy
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
