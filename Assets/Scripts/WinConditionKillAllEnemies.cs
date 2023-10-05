using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinConditionKillAllEnemies : MonoBehaviour
{
    public GameObject[] objectsToMonitor;
    public string sceneToLoad;

    private bool isChecking = false;

    private void Update()
    {
        // Check if all objects have been destroyed
        bool allDestroyed = true;
        foreach (var obj in objectsToMonitor)
        {
            if (obj != null)
            {
                allDestroyed = false;
                break;
            }
        }

        if (allDestroyed && !isChecking)
        {
            isChecking = true;
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        // Wait for a short delay before loading the scene
        yield return new WaitForSeconds(2.0f); // Adjust the delay time as needed

        // Load the new scene
        SceneManager.LoadScene(sceneToLoad);
    }
}
