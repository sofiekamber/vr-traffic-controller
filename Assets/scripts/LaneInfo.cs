using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneInfo : MonoBehaviour
{
    [SerializeField]
    Material laneClosed;

    [SerializeField]
    Material laneOpen;

    List<GameObject> lanes = new List<GameObject>();

    void Start()
    {
        foreach (Transform lane in transform)
            lanes.Add(lane.gameObject);
    }

    void Update()
    {
        for (int i = 0; i < lanes.Count; i++)
        {
            lanes[i].GetComponent<MeshRenderer>().material = userAction.lane_stop[i] ? laneClosed : laneOpen;
        }
    }
}
