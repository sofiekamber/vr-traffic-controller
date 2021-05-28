/* SceneHandler.cs*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class UserInputMenu : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;

    public Button NewGameButton;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
    if (e.target.name == "Newgame")
        {
            Debug.Log("Button was clicked");
            NewGameButton.onClick.Invoke();
        }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
    if (e.target.name == "NewGame")
        {
            Debug.Log("Button was entered");

        }
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {

        if (e.target.name == "NewGame")
        {
            Debug.Log("Button was exited");
        }
    }
}