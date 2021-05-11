using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class userAction : MonoBehaviour
{
    public static bool[] lane_stop = { false, false, false, false };


    // Actions for UP lane
    public void UpGo()
    {
        lane_stop[0] = false;
        Debug.Log("Lane 1 go");
    }
    public void UpStop()
    {
        lane_stop[0] = true;
        Debug.Log("Lane 1 stop");
    }

    // Actions for LEFT lane
    public void LeftGo()
    {
        lane_stop[1] = false;
        Debug.Log("Lane 2 go");
    }
    public void LeftStop()
    {
        lane_stop[1] = true;
        Debug.Log("Lane 2 stop");
    }

    // Actions for DOWN lane
    public void DownGo()
    {
        lane_stop[2] = false;
        Debug.Log("Lane 3 go");
    }

    public void DownStop()
    {
        lane_stop[2] = true;
        Debug.Log("Lane 3 stop");
    }

     // Actions for RIGHT lane
    public void RightGo()
    {
        lane_stop[3] = false;
        Debug.Log("Lane 4 go");
    }
    public void RightStop()
    {
        lane_stop[3] = true;
        Debug.Log("Lane 4 stop");
    }

}
