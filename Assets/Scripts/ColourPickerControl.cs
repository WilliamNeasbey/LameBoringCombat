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
}
