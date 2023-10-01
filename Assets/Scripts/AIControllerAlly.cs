using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControllerAlly : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private TacticalMode tacticalMode;
    private Transform target;

    private bool isAttacking;
    private float attackCooldown = 2.0f;
    private float lastAttackTime;

    // Adjust these values as needed
    public float attackRange = 2.0f;
    public float navigationRange = 10.0f;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        tacticalMode = GetComponent<TacticalMode>();
    }

    void Update()
    {
        // Find the nearest target within the navigation range
        FindNearestTarget();

        // Check if a target is found
        if (target != null)
        {
            // Calculate the distance to the target
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Check if the target is within attack range
            if (distanceToTarget <= attackRange)
            {
                // Stop moving
                navMeshAgent.isStopped = true;

                // Attack the target
                if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
                {
                    // Perform the punch-punch-kick combo
                    StartCoroutine(PerformCombo());
                }
            }
            else
            {
                // Move towards the target
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(target.position);
            }
        }
        else
        {
            // No target found, stop moving
            navMeshAgent.isStopped = true;
        }
    }

    void FindNearestTarget()
    {
        float nearestDistance = navigationRange;
        target = null;

        foreach (var potentialTarget in tacticalMode.targets)
        {
            if (potentialTarget != null)
            {
                float distanceToPotentialTarget = Vector3.Distance(transform.position, potentialTarget.position);

                // Check if the potential target is closer than the current target
                if (distanceToPotentialTarget < nearestDistance)
                {
                    nearestDistance = distanceToPotentialTarget;
                    target = potentialTarget;
                }
            }
        }
    }

    IEnumerator PerformCombo()
    {
        isAttacking = true;
        // Trigger punch animations or other attack actions here
        yield return new WaitForSeconds(0.5f); // Adjust the duration of the punch animation

        // Trigger the second punch animation
        // Adjust the animation trigger names as needed in your animator
        GetComponent<Animator>().SetTrigger("Punch");
        yield return new WaitForSeconds(0.5f); // Adjust the duration of the second punch animation

        // Trigger the kick animation
        // Adjust the animation trigger names as needed in your animator
        GetComponent<Animator>().SetTrigger("AirKick");
        yield return new WaitForSeconds(0.5f); // Adjust the duration of the kick animation

        isAttacking = false;
        lastAttackTime = Time.time;
    }
}
