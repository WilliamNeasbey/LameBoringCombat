using UnityEngine;

public class ObjectColourChanger : MonoBehaviour
{
    public Material[] shirtMaterials; // Array of materials for shirts.
    public Material[] pantsMaterials; // Array of materials for pants.
    public Material[] glovesMaterials; // Array of materials for gloves.
    public Material[] hairMaterials; // Array of materials for hair.
    public Material[] shoesMaterials; // Array of materials for shoes.

    const string ShirtColorPrefsKey = "ShirtColor";
    const string PantsColorPrefsKey = "PantsColor";
    const string GlovesColorPrefsKey = "GlovesColor";
    const string HairColorPrefsKey = "HairColor";
    const string ShoesColorPrefsKey = "ShoesColor";

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

    // Call this method when initializing the object to load its color for gloves.
    public void InitializeGlovesWithPlayerPrefsKey(string prefsKey)
    {
        LoadColorFromPlayerPrefs(prefsKey, glovesMaterials);
    }

    // Call this method when initializing the object to load its color for hair.
    public void InitializeHairWithPlayerPrefsKey(string prefsKey)
    {
        LoadColorFromPlayerPrefs(prefsKey, hairMaterials);
    }

    // Call this method when initializing the object to load its color for shoes.
    public void InitializeShoesWithPlayerPrefsKey(string prefsKey)
    {
        LoadColorFromPlayerPrefs(prefsKey, shoesMaterials);
    }

    private void Start()
    {
        // Load and apply colors when the object spawns.
        Debug.Log("Awake: Loading and applying colors...");
        InitializeShirtWithPlayerPrefsKey(ShirtColorPrefsKey);
        InitializePantsWithPlayerPrefsKey(PantsColorPrefsKey);
        InitializeGlovesWithPlayerPrefsKey(GlovesColorPrefsKey);
        InitializeHairWithPlayerPrefsKey(HairColorPrefsKey);
        InitializeShoesWithPlayerPrefsKey(ShoesColorPrefsKey);
        Debug.Log("Awake: Colors loaded and applied.");
    }
}
