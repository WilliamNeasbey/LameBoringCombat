using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnCollision : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // Name of the scene to load
    [SerializeField] private Transform player;


    private void OnTriggerEnter(Collider other)
    {
        
            // Load the specified scene
            SceneManager.LoadScene(sceneToLoad);
        
    }
}
