using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public static bool IsDayLight = true;

    public Button dayButton;

    // Update is called once per frame
    void Update()
    {
        IsDayLight = !dayButton.interactable;
    }
}
