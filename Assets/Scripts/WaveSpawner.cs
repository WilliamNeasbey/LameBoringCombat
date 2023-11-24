using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 10f;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI enemiesLeftText;
    public GameObject[] enemyPrefabs; // Array of enemy prefabs

    private int currentRound = 0;
    private int enemiesLeftInWave = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private float countdown = 2f;
    private bool isWaveActive = false;

    private int startingEnemyHealth = 30;

    void Start()
    {
        roundText.text = "Round: " + currentRound;
        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        if (isWaveActive)
        {
            // Check for destroyed enemies
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                if (activeEnemies[i] == null)
                {
                    HandleEnemyDeath();
                    activeEnemies.RemoveAt(i);
                }
            }
            return;
        }

        if (enemiesLeftInWave == 0)
        {
            if (countdown <= 0f)
            {
                StartCoroutine(SpawnWave());
                countdown = timeBetweenWaves;
                return;
            }
            countdown -= Time.deltaTime;
        }
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(2f); // Give some initial delay

        while (true) // Infinite waves
        {
            currentRound++;
            roundText.text = "Round: " + currentRound;
            UpdateEnemyHealthForRound();
            yield return SpawnWave();
            
        }
        

        
    }

    IEnumerator SpawnWave()
    {
        isWaveActive = true;

        // Set the number of enemies to be spawned for the current wave
        enemiesLeftInWave = currentRound; // Adjust the number of enemies as needed
        enemiesLeftText.text = "Enemies Left: " + enemiesLeftInWave;

        for (int i = 0; i < enemiesLeftInWave; i++)
        {
            GameObject enemy = SpawnEnemy();
            activeEnemies.Add(enemy);
        }

        while (activeEnemies.Count > 0)
        {
            yield return null;
        }

        isWaveActive = false;
    }

    GameObject SpawnEnemy()
    {
        // Randomly select an enemy prefab and a spawn point
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Transform spawnPoint = GetActiveSpawnPoint();
        if (spawnPoint != null)
        {
            // Instantiate the enemy prefab at the chosen spawn point
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            EnemyScript enemyScript = enemy.GetComponent<EnemyScript>(); // Get the EnemyScript component

            if (enemyScript != null)
            {
                // Update the enemy's health when spawning, considering round 5 onwards
                if (currentRound >= 5)
                {
                    float newHealth = startingEnemyHealth + ((currentRound - 5) * 10);
                    enemyScript.UpdateHealth(newHealth);
                }
                else
                {
                    enemyScript.UpdateHealth(startingEnemyHealth); // Set default health for rounds below 5
                }
            }

            return enemy;
        }
        return null;
    }



    Transform GetActiveSpawnPoint()
    {
        // Randomly select an active spawn point
        List<Transform> activeSpawnPoints = new List<Transform>();
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.gameObject.activeSelf)
            {
                activeSpawnPoints.Add(spawnPoint);
            }
        }

        if (activeSpawnPoints.Count > 0)
        {
            return activeSpawnPoints[Random.Range(0, activeSpawnPoints.Count)];
        }
        return null;
    }

    // This method is called when an enemy dies
    public void HandleEnemyDeath()
    {
        enemiesLeftInWave--;

        enemiesLeftText.text = "Enemies Left: " + enemiesLeftInWave;

        if (enemiesLeftInWave <= 0)
        {
            // Wave completed, you can proceed to the next wave or perform other actions here
        }
    }

    void UpdateEnemyHealthForRound()
    {
        //the spawnenemy function actually sets the enemies health now
        if (currentRound >= 5) // Check if the current round is greater than or equal to 5
        {
            // Calculate new health based on the current round or wave
            float newHealth = startingEnemyHealth + ((currentRound - 5) * 10); // Offset by 5 rounds

            // Find all enemies and update their health
            EnemyScript[] enemies = GameObject.FindObjectsOfType<EnemyScript>();
            foreach (EnemyScript enemy in enemies)
            {
                enemy.UpdateHealth(newHealth);
            }
        }
    }



}

