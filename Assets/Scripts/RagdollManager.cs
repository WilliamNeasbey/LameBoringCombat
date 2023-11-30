using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using System.Collections.Generic;

[BurstCompile]
public struct RagdollCleanupJob : IJob
{
    public NativeList<int> ragdollInstanceIDs;
    public int maxRagdolls;
    public bool shouldCleanup;
    private Dictionary<int, GameObject> instanceIDToObjectMap;

    public void Initialize(Dictionary<int, GameObject> instanceIDMap)
    {
        instanceIDToObjectMap = instanceIDMap;
    }

    public void Execute()
    {
        int ragdollCount = ragdollInstanceIDs.Length;

        if (ragdollCount >= maxRagdolls)
        {
            Debug.Log($"Number of ragdolls found: {ragdollCount}");

            // Find the oldest ragdoll based on creation time
            float oldestTime = float.MaxValue;
            int oldestIndex = -1;

            for (int i = 0; i < ragdollCount; i++)
            {
                if (instanceIDToObjectMap.TryGetValue(ragdollInstanceIDs[i], out GameObject ragdoll))
                {
                    RagdollData ragdollData = ragdoll.GetComponent<RagdollData>();
                    if (ragdollData != null)
                    {
                        float creationTime = ragdollData.creationTime;

                        if (creationTime < oldestTime)
                        {
                            oldestTime = creationTime;
                            oldestIndex = i;
                        }
                    }
                }
            }

            // Delete the oldest ragdoll
            if (oldestIndex != -1)
            {
                int oldestInstanceID = ragdollInstanceIDs[oldestIndex];
                ragdollInstanceIDs.RemoveAtSwapBack(oldestIndex);

                if (instanceIDToObjectMap.TryGetValue(oldestInstanceID, out GameObject oldestRagdoll))
                {
                    instanceIDToObjectMap.Remove(oldestInstanceID);
                    Object.Destroy(oldestRagdoll);
                    Debug.Log("Oldest ragdoll deleted.");
                }
            }

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

    public NativeList<int> ragdollInstanceIDs;
    public bool shouldCleanup;
    private Dictionary<int, GameObject> instanceIDToObjectMap;

    private void OnEnable()
    {
        ragdollInstanceIDs = new NativeList<int>(Allocator.Persistent);
        instanceIDToObjectMap = new Dictionary<int, GameObject>();
        InvokeRepeating(nameof(CheckRagdolls), 0f, checkInterval);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(CheckRagdolls));
        CleanupRagdolls(); // Ensure to clean up ragdolls before disabling
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
                instanceIDToObjectMap[instanceID] = ragdolls[i];

                RagdollData ragdollData = ragdolls[i].GetComponent<RagdollData>();
                if (ragdollData == null)
                {
                    ragdollData = ragdolls[i].AddComponent<RagdollData>();
                }
                ragdollData.creationTime = Time.time;

                Debug.Log("Ragdoll object detected.");
            }
        }

        if (ragdollCount >= maxRagdolls || shouldCleanup)
        {
            CleanupRagdolls();
        }

        Debug.Log($"Time until the next check: {checkInterval:F2} seconds.");
    }

    private void CleanupRagdolls()
    {
        Debug.Log("Cleanup initiated.");

        RagdollCleanupJob cleanupJob = new RagdollCleanupJob
        {
            ragdollInstanceIDs = ragdollInstanceIDs,
            maxRagdolls = maxRagdolls,
            shouldCleanup = shouldCleanup
        };

        // Initialize the instanceIDToObjectMap in the job
        cleanupJob.Initialize(instanceIDToObjectMap);

        cleanupJob.Execute(); // Manually execute the cleanup job

        Debug.Log("Cleanup executed.");
    }
}

