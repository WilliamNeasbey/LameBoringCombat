using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISceneLoader : MonoBehaviour
{
    public string sceneName; // The name of the scene you want to load (set in the Inspector)

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
