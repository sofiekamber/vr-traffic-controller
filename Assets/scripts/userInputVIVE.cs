using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class userInputVIVE : MonoBehaviour
{
    [SerializeField]
    private userAction _userAction;

    public SteamVR_Action_Boolean Go_Cars = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "CarsGO");
    public SteamVR_Action_Boolean Stop_Cars = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "CarsSTOP");
    void Update()
    {
        if (Go_Cars.GetState(SteamVR_Input_Sources.Any))
        {
            _userAction.UpGo();
        }

        if (Stop_Cars.GetState(SteamVR_Input_Sources.Any))
        {
            _userAction.UpStop();
        }
    }
}
