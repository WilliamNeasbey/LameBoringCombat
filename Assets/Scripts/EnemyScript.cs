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
    private bool isHit = false; // Flag to control hit state
    private float hitDuration = 0.5f; // Half a second hit duration
    private float lastHitTime = 0f;
    private bool isHitStun = false; // Flag to control hit stun state
    private float hitStunDuration = 1.8f; // Adjust hit stun duration as needed
    private float hitStunEndTime = 0f; // Add this variable to store the end time of hit stun
    private NavMeshAgent navMeshAgent;
    public GameObject ragdollPrefab;
    public float ragdollForce = 500f; // Adjust the force magnitude as needed
    public float forceRandomRange = 100f; // Adjust the random range as needed

    private bool hasDanced = false;

    public float minDistanceToOtherEnemies = 2f; // Minimum distance to maintain from other enemies

    // References to  colliders 
    public WeaponCollisionEnemy weapon;
    public GameObject WeaponObject;
    public LeftFistCollisionEnemy leftFist;
    public GameObject LeftFistObject;
    public RightFistCollisionEnemy rightFist;
    public GameObject RightFistObject;

    public int pointsValue = 100; // Points to add when this enemy is destroyed

    private PointCounter pointCounter;

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
            // Check if the enemy is overlapping with the player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer < 1.0f)
            {
                // Calculate a direction away from the player
                Vector3 moveDirection = (transform.position - playerTransform.position).normalized;
                moveDirection.y = 0f; // Keep the Y component at 0 to prevent vertical rotation

                // Move the enemy away from the player by 1 unit
                transform.Translate(moveDirection * 1.0f * Time.deltaTime, Space.World);
            }

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
                // Calculate the direction to the target
                Vector3 targetDirection = targetTransform.position - transform.position;
                targetDirection.y = 0f; // Keep the Y component at 0 to prevent vertical rotation

                // Check if the enemy is within attack range
                if (distanceToPlayer <= attackRange || distanceToAlly <= attackRange)
                {
                    // Stop moving
                    navMeshAgent.isStopped = true;

                    // Rotate to face the target while keeping the Y rotation constant
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = targetRotation;

                    // Attack the target
                    AttackPlayer();
                }
                else
                {
                    // Player/ally is not in attack range, so move towards the target
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(targetTransform.position);

                    // Rotate to face the movement direction while keeping Y rotation constant
                    Vector3 moveDirection = (targetTransform.position - transform.position).normalized;
                    moveDirection.y = 0f; // Keep the Y component at 0 to prevent vertical rotation
                    transform.rotation = Quaternion.LookRotation(moveDirection);
                }
            }
            // Check if the enemy is in a hit stun state
            if (isHitStun)
            {
                // Check if the hit stun duration has passed
                if (Time.time >= hitStunEndTime)
                {
                    isHitStun = false;
                }
            }

            // If not in hit state or hit stun state, the enemy can attack
            if (!isHit && !isHitStun)
            {
                AttackPlayer();
            }
        }
        // Check if the player object is destroyed and the dance animation hasn't been triggered yet
        if (playerTransform == null && !hasDanced)
        {
            // Player is destroyed, trigger a dance animation once
            TriggerDanceAnimationOnce();

            // Set the flag to true to indicate that the dance animation has been triggered
            hasDanced = true;
        }
    }







    private void MoveTowardsPlayer()
    {
        // Calculate the direction to move towards the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Calculate the minimum distance at which the enemy should push away
        float minPushDistance = 1.0f; // You can adjust this value as needed

        // Check if the enemy is too close to the player
        if (distanceToPlayer < minPushDistance)
        {
            // Calculate the direction to push the enemy away from the player
            Vector3 pushDirection = -directionToPlayer;

            // Move the enemy away from the player
            transform.Translate(pushDirection * moveSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // If the enemy is not too close, simply move towards the player
            transform.Translate(directionToPlayer * moveSpeed * Time.deltaTime, Space.World);
        }

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
        if (!isHitStun)
        {
            // Check if it's time for the next combo step
            if (Time.time - lastComboStepTime >= comboCooldown)
            {
                lastComboStepTime = Time.time;
                ComboStep();
            }
        }

        // If the enemy is in a hit state, check the hit duration
        if (isHit && Time.time - lastHitTime >= hitDuration)
        {
            // Reset the hit state when the hit duration has passed
            isHit = false;
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
        
        // Apply damage
            health -= 10;

            // Trigger the hit animation
            anim.SetTrigger("Hit");

            // Check if the enemy's health is depleted
            if (health <= 0f)
            {
                Die();
            }
        // Set the hit stun state and its end time
        EnterHitStun();
        // Check if the enemy is not already in a hit state
        if (!isHit)
        {
            // Set the hit state and duration
            isHit = true;
            lastHitTime = Time.time;

            
        }
    }




    private void EnterHitStun()
    {
        // Set hit stun state and its end time
        isHitStun = true;
        hitStunEndTime = Time.time + hitStunDuration;
    }


    public void Die()
    {

        Destroy(gameObject);
        // Play death audio or perform other actions here

       

        // Notify listeners that the enemy has died
        SendMessage("OnEnemyDied", SendMessageOptions.DontRequireReceiver);

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

        // Add points when the enemy is destroyed
        PointCounter pointCounter = FindObjectOfType<PointCounter>();
        if (pointCounter != null)
        {
            pointCounter.AddPoints(100); // Add 100 points for each destroyed enemy
        }

    }

    private void TriggerDanceAnimationOnce()
    {
        // List of dance animation trigger names
        string[] danceAnimations = { "Dance1", "Dance2", "Dance3" };

        // Choose a random dance animation
        string randomDanceAnimation = danceAnimations[Random.Range(0, danceAnimations.Length)];

        // Trigger the chosen dance animation
        anim.SetTrigger(randomDanceAnimation);
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
