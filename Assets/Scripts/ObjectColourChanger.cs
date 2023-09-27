using UnityEngine;

public class ObjectColourChanger : MonoBehaviour
{
    public Material[] shirtMaterials; // Array of materials for shirts.
    public Material[] pantsMaterials; // Array of materials for pants.
    public Material[] glovesMaterials; // Array of materials for gloves.
    public Material[] hairMaterials; // Array of materials for hair.
    public Material[] shoesMaterials; // Array of materials for shoes.
    public Material[] hatMaterials; // Array of materials for hats.
    public Material[] backBlingMaterials; // Array of materials for back bling.
    public Material[] leftHandMaterials; // Array of materials for left hand.
    public Material[] rightHandMaterials; // Array of materials for right hand.
    public Material[] pantsAccessoryMaterials; // Array of materials for pants accessory.
    public Material[] leftShoeMaterials; // Array of materials for left shoe.
    public Material[] rightShoeMaterials; // Array of materials for right shoe.


    const string ShirtColorPrefsKey = "ShirtColor";
    const string PantsColorPrefsKey = "PantsColor";
    const string GlovesColorPrefsKey = "GlovesColor";
    const string HairColorPrefsKey = "HairColor";
    const string ShoesColorPrefsKey = "ShoesColor";
    const string HatColorPrefsKey = "HatColor"; // New PlayerPrefs key for hat color.
    const string BackBlingColorPrefsKey = "BackBlingColor"; // PlayerPrefs key for back bling color.
    const string LeftHandColorPrefsKey = "LeftHandColor"; // PlayerPrefs key for left hand color.
    const string RightHandColorPrefsKey = "RightHandColor"; // PlayerPrefs key for right hand color.
    const string PantsAccessoryColorPrefsKey = "PantsAccessoryColor"; // PlayerPrefs key for pants accessory color.
    const string LeftShoeColorPrefsKey = "LeftShoeColor"; // PlayerPrefs key for left shoe color.
    const string RightShoeColorPrefsKey = "RightShoeColor"; // PlayerPrefs key for right shoe color.

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

    // Call this method when initializing the object to load its color for hats.
    public void InitializeHatWithPlayerPrefsKey(string prefsKey)
    {
        LoadColorFromPlayerPrefs(prefsKey, hatMaterials);
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
        InitializeHatWithPlayerPrefsKey(HatColorPrefsKey);
        Debug.Log("Awake: Colors loaded and applied.");
    }
}
