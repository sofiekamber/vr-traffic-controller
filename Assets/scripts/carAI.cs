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

public enum Direction
{
    Straight,
    Right,
    Left
}

[Serializable]
public struct Blinker
{
    public GameObject modelRight;
    public GameObject modelLeft;
    public Material materialOn;
    public Material materialOff;
}

public class carAI : MonoBehaviour
{
    public Transform path;
    public Direction direction;
    public float intersectionMaxSpeed;

    [SerializeField]
    float maxSteerAngle = 40.0f;
    [SerializeField]
    float maxVelocity = 50.0f;
    [SerializeField]
    int brakingStrenght = 100000; //just some random try and error number
    [SerializeField]
    int accelerationStrenght = 10000; //just some random try and error number

    [SerializeField]
    Vector3 centerOfMass;
    [SerializeField]
    List<Wheel> wheels;
    [SerializeField]
    GameObject carModel;
    [SerializeField]
    Blinker blinker;

    Rigidbody carRB;

    List<Transform> nodes;
    int chosenPath;
    int currentNode = 0;

    float blinkTime = 0;
    bool isBlinkerOn = false;

    void Start()
    {
        //get all nodes from path
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        foreach (var pathTransform in pathTransforms){
            if (pathTransform != path.transform)
                nodes.Add(pathTransform);
        }

        //Set center of mass for better physics (prevent flipping)
        carRB = GetComponent<Rigidbody>();
        carRB.centerOfMass = centerOfMass;

        ChooseRandomColor();
    }

    void FixedUpdate()
    {
        wheelTurn();
        wheelRotation();
        moveCar();
        checkWaypoins();
        Blink();
    }

    private void ChooseRandomColor()
    {
        Material[] materials = carModel.GetComponent<MeshRenderer>().materials;

        // HSV: choose saturation in the upper half, not too bright
        materials[2].color = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0f, 0.75f);
        // we have to reassign the whole array
        carModel.GetComponent<MeshRenderer>().materials = materials;
    }

    private void wheelTurn()
    {
        //get vector between car position and target node
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);

        //calculate desired steering angle
        float steerAngle = relativeVector.x / relativeVector.magnitude * maxSteerAngle;

        foreach (var wheel in wheels){
            //set steering angle for front wheels (lerp for smooth transition)
            if (wheel.axel == Axel.Front)
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, steerAngle, 0.5f);
        }
    }

    private void wheelRotation()
    {
        Quaternion rotation;
        Vector3 position;

        //set rotation for all wheels
        foreach (var wheel in wheels){

            //luckily, the wheelcollider is doing all the work for us :).
            wheel.collider.GetWorldPose(out position, out rotation);

            //assign position and rotation to wheel
            wheel.model.transform.position = position;
            wheel.model.transform.rotation = rotation;
        }
    }

    private void moveCar()
    {
        //get torque
        float torque = getTorque();

        //set torque of each wheel
        foreach (var wheel in wheels){
            if(torque > 0){
                //accelerate
                wheel.collider.motorTorque = torque;
                wheel.collider.brakeTorque = 0;
            }
            else{
                //brake
                wheel.collider.motorTorque = 0;
                wheel.collider.brakeTorque = torque * -1;
            }
        }
            
    }

    private float getTorque()
    {
        float carVelocity = carRB.velocity.magnitude * 3.6f;

        //check if there is another car in front of me
        Vector3 forwardVector = transform.rotation * Vector3.forward;
        RaycastHit hit;
        Ray forwardRay = new Ray(transform.position, forwardVector);

        if (Physics.Raycast(forwardRay, out hit)){
            //hit has to have a rigidbody, otherwise ignore it
            if(hit.rigidbody != null){
                //calculate relative velocity in m/s between car in front and myself
                float relativeVelocity = carRB.velocity.magnitude - hit.rigidbody.velocity.magnitude;

                //calculate braking distance (speed in km/h / 10)^2 - according to my VKU ;)
                float brakingDistance = (float)Math.Pow(((relativeVelocity * 3.6f) / 10), 2);

                //we want a security distance, so we add some value to the brakingDistance
                if (brakingDistance + 10 >= hit.distance)
                    return -brakingStrenght * Time.deltaTime;
            }
        }

        //check max speed for turning at the intersection
        if(currentNode == 0 && carVelocity > intersectionMaxSpeed){
            //get distance between first waypoint and actual position
            float distance = Vector3.Distance(transform.position, nodes[currentNode].position);

            //calculate breaking distance (did not find a formula for breaking not to zero,
            //so just use the same formula here (feel free to improve :D ))
            float brakingDistance = (float)Math.Pow(((carRB.velocity.magnitude * 3.6f) / 10), 2);

            //but we dont add a security distance here
            if (brakingDistance >= distance)
                return -brakingStrenght * Time.deltaTime;
        }

        //check max speed limit
        if (carVelocity > maxVelocity)
            return 0;

        return accelerationStrenght * Time.deltaTime;
    }


    private void checkWaypoins()
    {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 1f){
            if (currentNode != nodes.Count - 1)
                //set next waypoint
                currentNode++;
            else
                //or kill when target node is reached
                Destroy(this.gameObject);
               
        }
    }

    private void Blink()
    {
        // switch material in a certain rate
        if (blinkTime >= 0.5f){
            Material otherMaterial = isBlinkerOn ? blinker.materialOff : blinker.materialOn;

            if (direction == Direction.Left)
                blinker.modelLeft.GetComponent<Renderer>().material = otherMaterial;
            else if (direction == Direction.Right)
                blinker.modelRight.GetComponent<Renderer>().material = otherMaterial;

            isBlinkerOn = !isBlinkerOn;
            blinkTime = 0;
        }
        blinkTime += Time.deltaTime;
    }
}