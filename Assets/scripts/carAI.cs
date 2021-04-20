using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public enum Axel
{
    Front,
    Rear
}

[Serializable]
public struct Wheel
{
    public GameObject model;
    public WheelCollider collider;
    public Axel axel;
}

public class carAI : MonoBehaviour
{
    public Transform path;

    [SerializeField]
    float maxSteerAngle = 40.0f;
    [SerializeField]
    Vector3 centerOfMass;
    [SerializeField]
    List<Wheel> wheels;
    [SerializeField]
    GameObject carModel;

    Rigidbody carRB;

    List<Transform> nodes;
    int chosenPath;
    int currentNode = 0;

    void Start()
    {
        //get all nodes from path
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        foreach (var pathTransform in pathTransforms)
        {
            if (pathTransform != path.transform)
                nodes.Add(pathTransform);
        }

        //Set center of mass for better physics (prevent flipping)
        carRB = GetComponent<Rigidbody>();
        carRB.centerOfMass = centerOfMass;

        ChooseRandomColor();
    }

    private void ChooseRandomColor()
    {
        Material[] materials = carModel.GetComponent<MeshRenderer>().materials;
        // HSV: choose saturation in the upper half, not too bright
        materials[2].color = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0f, 0.75f);
        // we have to reassign the whole array
        carModel.GetComponent<MeshRenderer>().materials = materials;
    }

    void FixedUpdate()
    {
        wheelTurn();
        wheelRotation();
        moveCar();
        checkWaypoins();
    }

    void wheelTurn()
    {
        //get vector between car position and target node
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);

        //calculate desired steering angle
        float steerAngle = relativeVector.x / relativeVector.magnitude * maxSteerAngle;

        foreach (var wheel in wheels)
        {
            //set steering angle for front wheels (lerp for smooth transition)
            if (wheel.axel == Axel.Front)
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, steerAngle, 0.5f);
        }
    }

    void wheelRotation()
    {
        //set rotation for all wheels
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;

            //luckily, the wheelcollider is doing all the work for us :).
            wheel.collider.GetWorldPose(out position, out rotation);

            //assign position and rotation to wheel
            wheel.model.transform.position = position;
            wheel.model.transform.rotation = rotation;
        }
    }

    void moveCar()
    {
        //set torque of each wheel
        foreach (var wheel in wheels)
            wheel.collider.motorTorque = 10000 * Time.deltaTime;
    }

    void checkWaypoins()
    {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 1f)
        {
            if (currentNode != nodes.Count - 1)
                //set next waypoint
                currentNode++;
            else
                //or kill when target node is reached
                Destroy(this.gameObject);
               
        }
    }
}