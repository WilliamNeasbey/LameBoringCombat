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
    MeshRenderer changeThisColour;
    private Material materialToChange;

    [SerializeField]
    private string customPrefsKey = "DefaultKey"; // Default key or specify your own default key.

    private void Start()
    {
        materialToChange = changeThisColour.material;

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

        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;

        for(int y = 0; y < svTexture.height; y++)
        {
            for(int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        UpdateOutputImage();

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

        // Update the material color (optional).
        UpdateMaterialColor(hexColor);
    }



    private void UpdateMaterialColor(string hexColor)
    {
        Color newColor;

        if (ColorUtility.TryParseHtmlString("#" + hexColor, out newColor))
        {
            // Update the material color here.
            // Assuming you have a materialToChange variable for the object.
            if (materialToChange != null && materialToChange.HasProperty("_Color"))
            {
                materialToChange.SetColor("_Color", newColor);
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
