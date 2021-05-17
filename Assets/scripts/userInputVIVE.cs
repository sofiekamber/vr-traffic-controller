using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class userInputVIVE : MonoBehaviour
{
    [SerializeField]
    private userAction _userAction; // take the actions we already defined

    
    [SerializeField]
    GameObject controllerLeft; //left hand will be for stop+go

    [SerializeField]
    GameObject controllerRight; // right hand will be for direction

    void Update()

    {
        Vector3 posControllerLeft = controllerLeft.transform.position; // get the postion of the left controller

        Quaternion rotControllerRight = controllerRight.transform.rotation; // get the rotation of the right controller, has to be Quaternion

        // define thresholds for raising/lowering controllers
        double stop_height = 1.8;
        double go_height = 0.5;

        // for rotation see https://answers.unity.com/questions/1332337/how-do-i-get-accurate-steamvr-vive-controller-x-ax.html
       
        // convert to euler

        float rotControllerRightEuler = (rotControllerRight.y + 1) * 180;


        //Debug.Log(posControllerLeft.y);
        //Debug.Log(rotControllerRightEuler);
       

        // for now: if controller is raised above 1.8m, the cars go, if lowered below 0.4m they stop 
        // direction: took some angles that seemed to make sense for the direction. needs some testing

        // Up Lane
        if (posControllerLeft.y > go_height && rotControllerRightEuler > 150 && rotControllerRightEuler < 210)
        {
              _userAction.UpGo();
        }

        if (posControllerLeft.y < stop_height && rotControllerRightEuler > 150 && rotControllerRightEuler < 210)
        {
            _userAction.UpStop();
        }

        // Right Lane
        if (posControllerLeft.y > go_height && rotControllerRightEuler > 240 && rotControllerRightEuler < 300)
        {
            _userAction.RightGo();
        }

        if (posControllerLeft.y <  stop_height && rotControllerRightEuler > 240 && rotControllerRightEuler < 300)
        {
            _userAction.RightStop();
        }

        // Down Lane (values for rotation between 330 and 30)
        if (posControllerLeft.y > go_height && (rotControllerRightEuler > 330 && rotControllerRightEuler < 360 || rotControllerRightEuler > 0 && rotControllerRightEuler < 30))
        {
            _userAction.DownGo();
        }

        if (posControllerLeft.y < stop_height && (rotControllerRightEuler > 330 && rotControllerRightEuler < 360 || rotControllerRightEuler > 0 && rotControllerRightEuler < 30))
        {
            _userAction.DownStop();
        }

        // Left Lane
        if (posControllerLeft.y > go_height && rotControllerRightEuler > 60 && rotControllerRightEuler < 120)
        {
            _userAction.LeftGo();
        }

        if (posControllerLeft.y < stop_height && rotControllerRightEuler > 60 && rotControllerRightEuler < 120)
        {
            _userAction.LeftStop();
        }

    }
}
