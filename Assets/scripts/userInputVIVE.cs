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

    [SerializeField]
    GameObject contrleft;
 
    void Update()

    {
        Vector3 posHMD = contrleft.transform.position;
        Debug.Log(posHMD);

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
