using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public float timeBetweenWaves = 10f;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI enemiesLeftText;
    public GameObject enemyPrefab; // Reference to the enemy prefab

    private int currentRound = 0;
    private int enemiesLeftInWave = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private float countdown = 2f;
    private bool isWaveActive = false;

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
        // Instantiate the enemy prefab at your spawn point
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        // You can handle your enemy spawning logic here
        return enemy;
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
}
