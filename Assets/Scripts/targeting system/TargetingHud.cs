using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Managing;

public class TargetingHud : NetworkBehaviour
{
    [SerializeField]
    GameObject targetPrefab;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    Camera shipCam;

    NetworkManager manager;

    List<GameObject> targets = new List<GameObject>();
    List<PredictionMotor> ships = new List<PredictionMotor>();

    private void Start()
    {
        
    }

    private void Update()
    {
        foreach(PredictionMotor ship in FindObjectsOfType<PredictionMotor>())
        {
            if (ship.transform != transform.root)
            {
                if (!ships.Contains(ship))
                {
                    ships.Add(ship);
                    GameObject target = Instantiate(targetPrefab, canvas.transform, false);
                    target.GetComponent<Target>().targetShip = ship;
                    targets.Add(target);
                }
            }
        }

        foreach (GameObject target in targets)
        {
            if (target.GetComponent<Renderer>().isVisible)
            {
                gameObject.SetActive(true);

                Vector3 shipPosition = shipCam.WorldToScreenPoint(target.GetComponent<Target>().targetShip.gameObject.transform.position);
                print("Ship position: " + shipPosition);
                print("target transform position: " + target.transform.position);

                target.transform.position = shipPosition;
            }
            else
            {
                gameObject.SetActive(false);

            }

        }
    }

    

        //for each ship in the scene
        //set target location to ship's screen space position
}
