using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Managing;
using UnityEngine.UI;

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

    PredictionMotor ship;

    private void Start()
    {
        ship = transform.root.GetComponent<PredictionMotor>();
    }

    private void Update()
    {
        foreach (PredictionMotor ship in FindObjectsOfType<PredictionMotor>())
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

        foreach (GameObject targetPre in targets)
        {
            Target target = targetPre.GetComponent<Target>();

            if (target.GetComponent<Target>().targetShip != null)
            {
                
                Renderer renderer = target.targetShip.shipRenderer;
                if (renderer == null)
                {
                    continue;
                }

                

                bool vis = renderer.isVisible;
                if(target.targetShip == null )
                {
                    //find whether the ship is dead so it works on client.
                    //currently the PredictionMotor does not deactivate, the main body and its children do
                    //meaning this chunk of code doesn't register the ship as dead
                    vis = false;
                }

                var heading = target.targetShip.transform.position - transform.position;
                float dot = Vector3.Dot(heading, transform.forward);
                print(dot);
                if (dot < 0)
                {
                    vis = false;
                }



                target.GetComponent<Image>().enabled = vis;
                target.distanceDisplay.gameObject.SetActive(vis);
                

                target.distanceDisplay.text = Vector3.Distance(ship.transform.position, target.targetShip.transform.position).ToString("0.00") + "m";

                if (!gameObject.activeInHierarchy)
                    gameObject.SetActive(true);

                Vector3 shipPosition = shipCam.WorldToScreenPoint(target.GetComponent<Target>().targetShip.gameObject.transform.position);
                

                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), shipPosition, shipCam, out Vector2 localPoint);

                target.GetComponent<RectTransform>().localPosition = localPoint;
            }

        }
    }

    

        //for each ship in the scene
        //set target location to ship's screen space position
}
