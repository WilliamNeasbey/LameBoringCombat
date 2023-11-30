using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public struct RagdollCleanupJob : IJob
{
    public NativeArray<int> ragdollInstanceIDs;
    public int maxRagdolls;

    public void Execute()
    {
        int ragdollCount = ragdollInstanceIDs.Length;

        if (ragdollCount >= maxRagdolls)
        {
            Debug.Log($"Number of ragdolls found: {ragdollCount}");

            // Rest of the cleanup logic
            // ...

            Debug.Log("Cleanup logic executed.");
        }
    }
}

public class RagdollData : MonoBehaviour
{
    public float creationTime;
}

public class RagdollManager : MonoBehaviour
{
    public float checkInterval = 10f; // Time interval to check for ragdolls
    public int maxRagdolls = 69; // Maximum number of ragdolls allowed

    private NativeList<int> ragdollInstanceIDs;
    private JobHandle ragdollCleanupJobHandle;
    private RagdollCleanupJob ragdollCleanupJob;
    private float nextCheckTime;

    private void OnEnable()
    {
        ragdollInstanceIDs = new NativeList<int>(Allocator.Persistent);
        nextCheckTime = Time.time + checkInterval; // Set initial check time
        InvokeRepeating(nameof(CheckRagdolls), 0f, checkInterval);
    }

    private void OnDisable()
    {
        ragdollCleanupJobHandle.Complete();
        ragdollInstanceIDs.Dispose();
    }

    private void CheckRagdolls()
    {
        Debug.Log($"Checking for ragdolls at time: {Time.time}");

        GameObject[] ragdolls = GameObject.FindGameObjectsWithTag("Ragdoll");
        int ragdollCount = ragdolls.Length;

        for (int i = 0; i < ragdollCount; i++)
        {
            int instanceID = ragdolls[i].GetInstanceID();

            if (!ragdollInstanceIDs.Contains(instanceID))
            {
                ragdollInstanceIDs.Add(instanceID);

                RagdollData ragdollData = ragdolls[i].GetComponent<RagdollData>();
                if (ragdollData == null)
                {
                    ragdollData = ragdolls[i].AddComponent<RagdollData>();
                }
                ragdollData.creationTime = Time.time;

                Debug.Log("Ragdoll object detected.");
            }
        }

        if (Time.time >= nextCheckTime)
        {
            Debug.Log($"Checking for objects with the Ragdoll layer. Found {ragdollCount} objects.");
            nextCheckTime = Time.time + checkInterval;

            if (!ragdollCleanupJobHandle.IsCompleted)
            {
                ragdollCleanupJobHandle.Complete();
            }

            ragdollCleanupJob = new RagdollCleanupJob
            {
                ragdollInstanceIDs = new NativeArray<int>(ragdollInstanceIDs.ToArray(), Allocator.Persistent),
                maxRagdolls = maxRagdolls
            };

            ragdollCleanupJobHandle = ragdollCleanupJob.Schedule();
        }

        Debug.Log($"Time until the next check: {nextCheckTime - Time.time:F2} seconds.");
    }
}
