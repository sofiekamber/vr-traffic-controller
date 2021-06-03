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

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        Button btn = e.target.GetComponent<Button>();
        if (e.target.name.StartsWith("btn_") && btn.interactable)
        {
            btn.onClick.Invoke();
        }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        Button btn = e.target.GetComponent<Button>();
        if (e.target.name.StartsWith("btn_") && btn.interactable)
        {
            btn.GetComponent<Image>().color = Color.gray;
        } else if (e.target.name.StartsWith("btn_"))
        {
            btn.GetComponent<Image>().color = Color.white;
        }
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        Button btn = e.target.GetComponent<Button>();
        if (e.target.name.StartsWith("btn_") && btn.interactable)
        {
            btn.GetComponent<Image>().color = Color.white;
        }
    }
}