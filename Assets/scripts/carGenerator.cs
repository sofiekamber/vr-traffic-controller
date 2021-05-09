using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

[Serializable]
public struct spawnAndPaths
{
    public Transform spawnPoint;
    public List<Path> paths;
}

[Serializable]
public struct Path
{
    public Transform path_transform;
    public float intersectionMaxSpeed;
}

public class carGenerator : MonoBehaviour
{
    [SerializeField]
    float minTimeBetweenCars;
    [SerializeField]
    int spawnChancePercentPerSecond;
    [SerializeField]
    List<spawnAndPaths> spawnPointsAndPaths;
    [SerializeField]
    GameObject carPrefab;

    float timeCounter = 0f;
    int checkCounter = 0;

    Random r = new System.Random();

    void Start()
    {
       
    }
   
    void Update()
    {
        if (checkSpawn())
            spawnCar();
    }

    bool checkSpawn()
    {
        //sum up time
        timeCounter += Time.deltaTime;

        //only spawn car if minimum Time between car spawn is exceeded
        if (timeCounter > minTimeBetweenCars){
            //car spawn only occurs once per second
            if ((timeCounter - minTimeBetweenCars) > checkCounter){
                checkCounter++;

                //check for spawn
                if (r.Next(0, 101) < spawnChancePercentPerSecond)
                    return true;
            }
        }

        return false;
    }

    void spawnCar()
    {
        int spawn = r.Next(0, spawnPointsAndPaths.Count);

        if (!checkSpawnCollision(spawnPointsAndPaths[spawn].spawnPoint.transform.position, 4f)){
            //only spawn when no collision is detected
            int path = r.Next(0, spawnPointsAndPaths[spawn].paths.Count);

            //create new car
            GameObject newCar = Instantiate(carPrefab, spawnPointsAndPaths[spawn].spawnPoint.transform.position, Quaternion.Euler(0f, -90f * spawn, 0f));
            carAI newCarClass = newCar.GetComponent<carAI>();
            //set car variables
            newCarClass.path = spawnPointsAndPaths[spawn].paths[path].path_transform;
            newCarClass.direction = (Direction)path;
            newCarClass.intersectionMaxSpeed = spawnPointsAndPaths[spawn].paths[path].intersectionMaxSpeed;

            //reset spawn counter
            timeCounter = 0f;
            checkCounter = 0;
        }
    }

    bool checkSpawnCollision(Vector3 position, float radius)
    {
        //get all colliding objects at spawn with certain radius 
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);

        foreach (var hitCollider in hitColliders){
            //check if collision object is a car
            if (hitCollider.gameObject.name == "chassis_Cube")
                return true;
        }

        return false;
    }
}
