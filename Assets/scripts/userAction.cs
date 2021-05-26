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
    }
    public void UpStop()
    {
        lane_stop[0] = true;
    }

    // Actions for LEFT lane
    public void LeftGo()
    {
        lane_stop[1] = false;
    }
    public void LeftStop()
    {
        lane_stop[1] = true;
    }

    // Actions for DOWN lane
    public void DownGo()
    {
        lane_stop[2] = false;
    }

    public void DownStop()
    {
        lane_stop[2] = true;
    }

     // Actions for RIGHT lane
    public void RightGo()
    {
        lane_stop[3] = false;
    }
    public void RightStop()
    {
        lane_stop[3] = true;
    }

}
