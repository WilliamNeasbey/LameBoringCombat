using UnityEngine;

public class ObjectColourChanger : MonoBehaviour
{
    public Material[] shirtMaterials; // Array of materials for shirts.
    public Material[] pantsMaterials; // Array of materials for pants.
    const string ShirtColorPrefsKey = "ShirtColor";
    const string PantsColorPrefsKey = "PantsColor";

    private void LoadColorFromPlayerPrefs(string prefsKey, Material[] materials)
    {
        if (PlayerPrefs.HasKey(prefsKey))
        {
            string hexColor = PlayerPrefs.GetString(prefsKey);
            Color loadedColor;
            if (ColorUtility.TryParseHtmlString(hexColor, out loadedColor))
            {
                foreach (Material material in materials)
                {
                    material.SetColor("_Color", loadedColor);
                }
            }
            else
            {
                Debug.LogError("Failed to parse color from PlayerPrefs for key: " + prefsKey);
            }
        }
        else
        {
            Debug.LogWarning("PlayerPrefs key not found: " + prefsKey);
        }
    }


    // Call this method when initializing the object to load its color for shirts.
    public void InitializeShirtWithPlayerPrefsKey(string prefsKey)
    {
        LoadColorFromPlayerPrefs(prefsKey, shirtMaterials);
    }

    // Call this method when initializing the object to load its color for pants.
    public void InitializePantsWithPlayerPrefsKey(string prefsKey)
    {
        LoadColorFromPlayerPrefs(prefsKey, pantsMaterials);
    }

    private void Start()
    {
        string loadedColor = PlayerPrefs.GetString(ShirtColorPrefsKey);
        Color parsedColor;
        if (ColorUtility.TryParseHtmlString(loadedColor, out parsedColor))
        {
            foreach (Material material in shirtMaterials)
            {
                material.SetColor("_Color", parsedColor);
            }
        }
        else
        {
            Debug.LogError("Failed to parse color from PlayerPrefs for key: " + ShirtColorPrefsKey);
        }

        // Load and apply shirt and pants colors when the object spawns.
        Debug.Log("Awake: Loading and applying colors...");
        InitializeShirtWithPlayerPrefsKey(ShirtColorPrefsKey);
        InitializePantsWithPlayerPrefsKey(PantsColorPrefsKey);
        Debug.Log("Awake: Colors loaded and applied.");
    }
}
