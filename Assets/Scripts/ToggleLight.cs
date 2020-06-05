
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ToggleLight : MonoBehaviour
{
    Light2D light2d;


    void Awake()
    {
        light2d = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("ToggleLight"))
        {
            light2d.enabled = !light2d.enabled;
        }

        
        
    }
}
