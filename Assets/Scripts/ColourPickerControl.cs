using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ColourPickerControl : MonoBehaviour
{

    public float currentHue, currentsat, currentVal;

    [SerializeField]
    private RawImage hueImage, satValImage, outputImage;

    [SerializeField]
    private Slider hueSlider;

    [SerializeField]
    private TMP_InputField hexInputFeild;

    private Texture2D hueTexture, svTexture, outputTexture;

    [SerializeField]
    private SkinnedMeshRenderer[] changeTheseColours; // Change this line

    private Material[] materialsToChange;

    private Material materialToChange;
   

    [SerializeField]
    private string customPrefsKey = "DefaultKey"; // Default key or specify your own default key.

    private void Start()
    {
        // Assuming you have SkinnedMeshRenderer components in the array.
        if (changeTheseColours != null && changeTheseColours.Length > 0)
        {
            // Assign the materials from the first SkinnedMeshRenderer in the array.
            materialsToChange = changeTheseColours[0].sharedMaterials;
        }
        else
        {
            // Handle the case where no SkinnedMeshRenderers are found.
            Debug.LogError("No SkinnedMeshRenderers found in the array.");
            // You may want to provide a fallback behavior or error handling here.
        }

        // The rest of your Start() method remains the same.
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();
        UpdateOutputImage();
    }


    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        for(int i =0; i < hueTexture.height; i++)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height, 1, 1f)); 
            
        }

        hueTexture.Apply();
        currentHue = 0;

        hueImage.texture = hueTexture;
    }

    private void CreateSVImage()
    {
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for(int y = 0; y < svTexture.height; y++)
        {
            for(int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
                
            }
        }

        svTexture.Apply();
        currentsat = 0;
        currentVal = 0;

        satValImage.texture = svTexture;

    }

    private void CreateOutputImage()
    {
        outputTexture = new Texture2D(1, 16);
        outputTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "OutputTexture";

        Color currentColour = Color.HSVToRGB(currentHue, currentsat, currentVal);

        for(int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColour);
        }

        outputTexture.Apply();

        outputImage.texture = outputTexture;

    }

    private void UpdateOutputImage()
    {
        Color currentColour = Color.HSVToRGB(currentHue, currentsat, currentVal);

        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColour);
        }

        outputTexture.Apply();

        hexInputFeild.text = ColorUtility.ToHtmlStringRGB(currentColour);

        // Check if materialToChange is assigned and has a "_Color" property.
        if (materialToChange != null && materialToChange.HasProperty("_Color"))
        {
            materialToChange.SetColor("_Color", currentColour);
        }
    }


    public void SetSV(float S, float V)
    {
        currentsat = S;
        currentVal = V;

        // Create an array of Color objects for each material.
        Color[] colors = new Color[materialsToChange.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.HSVToRGB(currentHue, currentsat, currentVal);
        }

        UpdateOutputImage();

        // Call the function to update the material colors with the array of Color objects.
        UpdateMaterialColor(colors);
    }

    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;

        for (int y = 0; y < svTexture.height; y++)
        {
            for (int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        // Create an array of Color objects for each material.
        Color[] colors = new Color[materialsToChange.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.HSVToRGB(currentHue, currentsat, currentVal);
        }

        UpdateOutputImage();

        // Call the function to update the material colors with the array of Color objects.
        UpdateMaterialColor(colors);
    }

    public void OnTextInput()
    {
        if(hexInputFeild.text.Length <6) { return; }

        Color newCol;

        if (ColorUtility.TryParseHtmlString("#" + hexInputFeild.text, out newCol))
            Color.RGBToHSV(newCol, out currentHue, out currentsat, out currentVal);

        hueSlider.value = currentHue;

        hexInputFeild.text = "";

        UpdateOutputImage();

    }

    /*
    private void SaveColorToPlayerPrefs(string prefsKey, Color color)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        PlayerPrefs.SetString(prefsKey, hexColor);
        PlayerPrefs.Save();
        //PlayerPrefs.SetString("ShirtColor", "#FFFFFF"); // Example color value.
        Debug.Log("After saving PlayerPrefs");

    }
    */

    public void OnConfirmButtonPressed()
    {
        // Get the hex color from the input field.
        string hexColor = hexInputFeild.text;

        // Remove the "#" symbol if it exists.
        hexColor = hexColor.Replace("#", "");

        // Determine the PlayerPrefs key based on the part (e.g., pants, shirt, hair).
        string prefsKey = customPrefsKey; // Assign the correct PlayerPrefs key for the part.
        Debug.Log("Using PlayerPrefs key: " + prefsKey);

        // Format the hex color string with "#" and uppercase letters.
        hexColor = "#" + hexColor.ToUpper();

        // Save the hex color to PlayerPrefs with the custom key.
        PlayerPrefs.SetString(prefsKey, hexColor);
        PlayerPrefs.Save();

        // Log the hex color being saved.
        Debug.Log("Saved Hex Color to PlayerPrefs: " + prefsKey + ", " + hexColor);

        // Convert the hexColor string to a Color object.
        Color newColor;
        if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
        {
            // Call the function to update the material colors with the new Color object.
            UpdateMaterialColor(new Color[] { newColor });
        }
    }




    private void UpdateMaterialColor(Color[] colors)
    {
        // Loop through each SkinnedMeshRenderer in the array.
        for (int i = 0; i < changeTheseColours.Length; i++)
        {
            SkinnedMeshRenderer renderer = changeTheseColours[i];

            if (renderer != null)
            {
                Material[] rendererMaterials = renderer.sharedMaterials;

                // Check if the renderer has the same number of materials as the colors array.
                if (rendererMaterials.Length == colors.Length)
                {
                    // Update each material's color with the corresponding Color from the array.
                    for (int j = 0; j < rendererMaterials.Length; j++)
                    {
                        Material material = rendererMaterials[j];
                        if (material != null && material.HasProperty("_Color"))
                        {
                            material.SetColor("_Color", colors[j]);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Materials count does not match colors count for SkinnedMeshRenderer " + i);
                }
            }
        }
    }





    private void LoadColorFromPlayerPrefs(string prefsKey)
    {
        if (PlayerPrefs.HasKey(prefsKey))
        {
            string hexColor = PlayerPrefs.GetString(prefsKey);
            Color loadedColor;
            if (ColorUtility.TryParseHtmlString(hexColor, out loadedColor))
            {
                materialToChange.SetColor("_Color", loadedColor);
            }
        }
    }

    /*
    // Call this method when initializing an object to load its color.
    public void InitializeWithPlayerPrefsKey(string prefsKey)
    {
        // Load the color based on the PlayerPrefs key for the part.
        LoadColorFromPlayerPrefs(prefsKey);
    }
    */
}
