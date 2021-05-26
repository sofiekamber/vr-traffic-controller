using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Light>().enabled = OptionsMenu.IsDayLight;
    }
}
