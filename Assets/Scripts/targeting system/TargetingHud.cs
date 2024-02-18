using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Managing;
using UnityEngine.UI;
using UnityEngine.Assertions.Must;
using GameKit.Utilities;

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
    List<GameObject> toRemove = new List<GameObject>();

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


        foreach (GameObject targetPrefab in targets)
        {
            Target target = targetPrefab.GetComponent<Target>();

            if (target.GetComponent<Target>().targetShip != null)
            {

                Renderer renderer = target.targetShip.shipRenderer;
                if (renderer == null)
                {
                    target.distanceDisplay.gameObject.SetActive(false);

                    continue;
                }



                bool vis = renderer.isVisible;

                



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
                float dot = Vector3.Dot(transform.forward.normalized, heading.normalized);
                int dotInt = (int)(dot*100f);



                /*float dotR = Vector3.Dot(heading.normalized, transform.right.normalized);
                int dotIntR = (int)(dotR*100f);

                float dotU = Vector3.Dot(heading.normalized, transform.up.normalized);
                int dotIntU = (int)(dotU*100f);
                print("dotint r: " + dotIntR);
                print("dotint u: " + dotIntU);*/
                print("dotint f: " + dotInt + " " + ship.name) ;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), shipPosition, shipCam, out Vector2 localPoint);
                float validCanvasSizeX = canvas.GetComponent<RectTransform>().rect.max.x + target._renderer.GetComponent<RectTransform>().rect.x;
                float validCanvasSizeY = canvas.GetComponent<RectTransform>().rect.max.y + target._renderer.GetComponent<RectTransform>().rect.y;
                localPoint.x = Mathf.Clamp(localPoint.x, -validCanvasSizeX, validCanvasSizeX);
                localPoint.y = Mathf.Clamp(localPoint.y, -validCanvasSizeY, validCanvasSizeY);
                if (Mathf.Abs(localPoint.x ) >= validCanvasSizeX + target._renderer.GetComponent<RectTransform>().rect.x || Mathf.Abs(localPoint.y ) >= validCanvasSizeY+ target._renderer.GetComponent<RectTransform>().rect.y)
                {
                    target._renderer.sprite = target._arrow;
                }
                else
                {
                    target._renderer.sprite = target._square;
                }

                if (dotInt == 0)
                {
                    var vectorToTarget = target.targetShip.transform.position - transform.position;
                    float dotR = Vector3.Dot(transform.right.normalized, vectorToTarget.normalized);
                    int dotIntR = (int)(dotR * 100f);
                print("dotint r: " + dotIntR + " " + ship.name) ;

                    if (dotIntR > 0)
                    {
                        localPoint.x = validCanvasSizeX;

                    }if (dotIntR < 0)
                    {
                        localPoint.x = -validCanvasSizeX;

                    }
                    
                    var vectorToTargetU = target.targetShip.transform.position - transform.position;
                    float dotU = Vector3.Dot(transform.up.normalized, vectorToTarget.normalized);
                    int dotIntU = (int)(dotU * 100f);
                print("dotint u: " + dotIntU + " " + ship.name) ;

                    if (dotIntU > 0)
                    {
                        localPoint.y = validCanvasSizeY;

                    }if (dotIntU < 0)
                    {
                        localPoint.y = -validCanvasSizeY;

                    }

                    /*if (target.gameObject.activeInHierarchy)
                        target.gameObject.SetActive(false);*/
                }/*
                else
                {
                    if(!target.gameObject.activeInHierarchy)
                        target.gameObject.SetActive(true);
                }*/

                if (dotInt < 0)
                {
                    localPoint = -localPoint;
                    
                }





                target.GetComponent<RectTransform>().localPosition = localPoint;
            }
            else
            {
                target.gameObject.SetActive(false);
                toRemove.Add(target.gameObject);

            }

        }
        foreach(GameObject removee in toRemove)
        {
            targets.Remove(removee);
        }
        toRemove.Clear();



    }
    
}
