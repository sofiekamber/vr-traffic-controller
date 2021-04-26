using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class userInputKeys : MonoBehaviour
{
    [SerializeField]
    private userAction _userAction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) && (Input.GetKey(KeyCode.G)))
        {
            _userAction.UpGo();
        }

        if (Input.GetKey(KeyCode.UpArrow) && (Input.GetKey(KeyCode.S)))
        {
            _userAction.UpStop();
        }

        if (Input.GetKey(KeyCode.DownArrow) && (Input.GetKey(KeyCode.G)))
        {
            _userAction.DownGo();
        }

        if (Input.GetKey(KeyCode.DownArrow) && (Input.GetKey(KeyCode.S)))
        {
            _userAction.DownStop();
        }

        if (Input.GetKey(KeyCode.RightArrow) && (Input.GetKey(KeyCode.G)))
        {
            _userAction.RightGo();
        }

        if (Input.GetKey(KeyCode.RightArrow) && (Input.GetKey(KeyCode.S)))
        {
            _userAction.RightStop();
        }

        if (Input.GetKey(KeyCode.LeftArrow) && (Input.GetKey(KeyCode.G)))
        {
            _userAction.LeftGo();
        }

        if (Input.GetKey(KeyCode.LeftArrow) && (Input.GetKey(KeyCode.S)))
        {
            _userAction.LeftStop();
        }


    }
}
