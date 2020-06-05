
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class LightEffector : MonoBehaviour
{

    private int textureLength = 16;
    private Texture2D cameraTexture;
    private Camera objectCamera = null;
    private Color32[] pixelArray = null;
    public float brightness;
    


    void Awake()
    {
        //Retrieve camera component
        objectCamera = GetComponentInChildren<Camera>();
        
    }

    


    void Update()
    {
        //Get the texture from the camera's view
        GetCameraTexture(); //This method is likely too expensive

        //Calculate the brightness shined on the sprite from texture
        CalculateBrightness();

        Debug.Log(brightness);

    }

    private void CalculateBrightness()
    {
        //Get array of pixels from camera texture
        pixelArray = cameraTexture.GetPixels32();

        float brightnessTotal = 0;
        foreach (Color32 p in pixelArray)
        {
            brightnessTotal += Luminance(p);

        }
        // print(brightnessTotal);
        brightness = (brightnessTotal / pixelArray.Length) * 10;
    }

    private float Luminance(Color c)
    {
        return (0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b);
    }

    private void GetCameraTexture()
    {
        //Make a new empty texture
        cameraTexture = new Texture2D(textureLength, textureLength, TextureFormat.RGB24, false);

        //Initialize RenderTexture
        RenderTexture rt = new RenderTexture(textureLength, textureLength, 24);
        //Pair camera with render texture
        objectCamera.targetTexture = rt;
        objectCamera.Render();
        RenderTexture.active = rt;

        //Read pixels, store information in cameraTexture
        cameraTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        //Memory Management
        objectCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
    }
}
