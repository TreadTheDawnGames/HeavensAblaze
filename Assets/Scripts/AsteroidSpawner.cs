using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public class AsteroidSpawner : NetworkBehaviour
{

    public int numToSpawn = 20;
    public Transform minBounds, maxBounds;
    public float sizeRange = 2f;

    public List<GameObject> asteroids = new List<GameObject>();
    

    public override void OnStartServer()
    {
        base.OnStartServer();

        for (int i = 0; i < numToSpawn; i++)
        {
            int randomNum = Random.Range(0, asteroids.Count);
            GameObject asteroid = asteroids[randomNum];

            float rX = Random.Range(minBounds.position.x, maxBounds.position.x);
            float rY = Random.Range(minBounds.position.y, maxBounds.position.y);
            float rZ = Random.Range(minBounds.position.z, maxBounds.position.z);
                                                                   
            Vector3 randomLocation = new Vector3(rX, rY, rZ);

            float size = Random.Range(1, sizeRange);
            

            asteroid.transform.localScale = new Vector3(size,size,size);

            GameObject spawn = Instantiate(asteroid, randomLocation, Random.rotation);
            Spawn(spawn);
        }




    }



}
