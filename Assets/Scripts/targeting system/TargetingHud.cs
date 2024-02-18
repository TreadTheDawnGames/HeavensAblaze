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

    public bool hideTargets = false;

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
                if (target.targetShip == null)
                {
                    //find whether the ship is dead so it works on client.
                    //currently the PredictionMotor does not deactivate, the main body and its children do
                    //meaning this chunk of code doesn't register the ship as dead
                    vis = false;
                }



                if (hideTargets)
                {
                    vis = false;
                }



                /*                target.GetComponent<Image>().enabled = vis;
                                target.distanceDisplay.gameObject.SetActive(vis);
                */

                target.distanceDisplay.text = Vector3.Distance(ship.transform.position, target.targetShip.transform.position).ToString("0.00") + "m";

                if (!gameObject.activeInHierarchy)
                    gameObject.SetActive(true);

                Vector3 shipPosition = shipCam.WorldToScreenPoint(target.GetComponent<Target>().targetShip.gameObject.transform.position);
                var heading = target.targetShip.transform.position - transform.position;
                float dot = Vector3.Dot(heading, transform.forward);
                

                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), shipPosition, shipCam, out Vector2 localPoint);
                if (dot < 0)
                {
                    localPoint = -localPoint;
                    //find a way to clamp the localPoint to the canvas edges
                    target._renderer.sprite = target._arrow;
                }
                else
                {
                    target._renderer.sprite = target._square;
                }


                target.GetComponent<RectTransform>().localPosition = localPoint;
            }

        }



    }
    
}
