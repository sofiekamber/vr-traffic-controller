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
    public List<Transform> paths;
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
        if (timeCounter > minTimeBetweenCars)
        {
            //car spawn only occurs once per second
            if ((timeCounter - minTimeBetweenCars) > checkCounter)
            {
                checkCounter++;

                //check for spawn
                if (r.Next(0, 101) < spawnChancePercentPerSecond)
                {
                    timeCounter = 0f;
                    checkCounter = 0;
                    return true;
                }

            }
        }

        return false;
    }

    void spawnCar()
    {
        int spawn = r.Next(0, spawnPointsAndPaths.Count);
        int path = r.Next(0, spawnPointsAndPaths[spawn].paths.Count);

        GameObject newCar = Instantiate(carPrefab, spawnPointsAndPaths[spawn].spawnPoint.transform.position, Quaternion.Euler(0f, -90f*spawn, 0f));
        carAI newCarClass = newCar.GetComponent<carAI>();
        newCarClass.path = spawnPointsAndPaths[spawn].paths[path];
        newCarClass.direction = (Direction) path;
    }
}
