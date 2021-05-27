using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    public Material daylightSkybox;
    public Material nightSkybox;
    public GameObject streetlights;

    // Start is called before the first frame update
    void Start()
    {
        // enable directional light when daylight is chosen
        GetComponent<Light>().enabled = OptionsMenu.IsDayLight;

        // set skybox accordingly
        RenderSettings.skybox = OptionsMenu.IsDayLight ? daylightSkybox : nightSkybox;

        // turn on street lights when there's no daylight
        streetlights.SetActive(!OptionsMenu.IsDayLight);
    }
}
