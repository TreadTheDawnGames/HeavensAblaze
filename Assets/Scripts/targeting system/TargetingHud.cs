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
    public Canvas canvas;

    [SerializeField]
    Camera shipCam;

    NetworkManager manager;

    List<GameObject> targets = new List<GameObject>();
    List<PredictionMotor> ships = new List<PredictionMotor>();
    List<GameObject> toRemove = new List<GameObject>();

    PredictionMotor ship;

    public bool hideTargets = false;

    public float distanceMultiplier = 0f;
    float rightLeftDistance = -14f;

    private void Start()
    {
        ship = transform.root.GetComponent<PredictionMotor>();
    }

    private void Update()
    {

        //make this an event that happens when a ship joins to get it out of the Update loop
        foreach (PredictionMotor ship in FindObjectsOfType<PredictionMotor>())
        {
            if (ship.transform != transform.root)
            {
                if (!ships.Contains(ship))
                {
                    ships.Add(ship);
                    GameObject target = Instantiate(targetPrefab, canvas.transform, false);
                    target.GetComponent<Target>().targetShip = ship.GetComponentInChildren<MainBody>() ;

                    targets.Add(target);
                }
            }
        }


        foreach (GameObject targetPrefab in targets)
        {
            Target target = targetPrefab.GetComponent<Target>();

            if (target.GetComponent<Target>().targetShip != null)
            {
                //fix hud hiding

                

                Renderer renderer = target.targetShip.GetComponent<Renderer>();
                if (renderer == null)
                {
                    target.distanceDisplay.gameObject.SetActive(false);

                    continue;
                }





                target.distanceDisplay.text = Vector3.Distance(ship.transform.position, target.targetShip.transform.position).ToString("0.00") + "m";

                if (!gameObject.activeInHierarchy)
                    gameObject.SetActive(true);

                Vector3 shipPosition = shipCam.WorldToScreenPoint(target.GetComponent<Target>().targetShip.gameObject.transform.position);
                var heading = target.targetShip.transform.position - transform.position;
                float dot = Vector3.Dot(transform.forward.normalized, heading.normalized);
                int dotInt = (int)(dot * 100f);




                print("dotint f: " + dotInt + " " + ship.name);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), shipPosition, shipCam, out Vector2 localPoint);

                float validCanvasSizeX = canvas.GetComponent<RectTransform>().rect.max.x + target._renderer.GetComponent<RectTransform>().rect.x;
                float validCanvasSizeY = canvas.GetComponent<RectTransform>().rect.max.y + target._renderer.GetComponent<RectTransform>().rect.y;

                localPoint.x = Mathf.Clamp(localPoint.x, -validCanvasSizeX, validCanvasSizeX);
                localPoint.y = Mathf.Clamp(localPoint.y, -validCanvasSizeY, validCanvasSizeY);

                


                //special case 
                if (dotInt == 0)
                {
                    var vectorToTarget = target.targetShip.transform.position - transform.position;
                    float dotR = Vector3.Dot(transform.right.normalized, vectorToTarget.normalized);
                    int dotIntR = (int)(dotR * 100f);
                    print("dotint r: " + dotIntR + " " + ship.name);

                    if (dotIntR > 0)
                    {
                        localPoint.x = validCanvasSizeX;

                    }
                    if (dotIntR < 0)
                    {
                        localPoint.x = -validCanvasSizeX;

                    }

                    var vectorToTargetU = target.targetShip.transform.position - transform.position;
                    float dotU = Vector3.Dot(transform.up.normalized, vectorToTarget.normalized);
                    int dotIntU = (int)(dotU * 100f);
                    print("dotint u: " + dotIntU + " " + ship.name);
                    
                    if (dotIntU > 0)
                    {
                        localPoint.y = validCanvasSizeY;

                    }
                    if (dotIntU < 0)
                    {
                        localPoint.y = -validCanvasSizeY;

                    }
                    

                    /*if (target.gameObject.activeInHierarchy)
                        target.gameObject.SetActive(false);*/
                }


                

                    float extraSpace = -Mathf.Clamp(distanceMultiplier * (localPoint.x) - (0.5f * localPoint.x), -14f, 14f) ;
                print(extraSpace);
                if (dotInt < 0)
                {

                    if (Mathf.Abs(localPoint.y) < validCanvasSizeY)
                    {
                        localPoint.x = validCanvasSizeX * (localPoint.x < 0 ? 1 : -1);



                        //scooch text right
                        //extraSpace = (target.distanceDisplay.rectTransform.rect.width-(target.arrow.rect.width*distanceMultiplier)) * (localPoint.x < 0 ? 1 : -1);

                        target.distanceDisplayTransform.localPosition = target.topAnchor.localPosition + new Vector3((extraSpace<0?rightLeftDistance:-rightLeftDistance), 0f);

                    }
                    else
                    {
                        //y=mx+b
                        if (localPoint.y < 0)
                            target.distanceDisplayTransform.localPosition = target.bottomAnchor.localPosition + new Vector3(extraSpace, 0f); 
                        else
                            target.distanceDisplayTransform.localPosition = target.topAnchor.localPosition + new Vector3(extraSpace, 0f); 

                        localPoint.x = -localPoint.x;
                    }


                    localPoint.y = -localPoint.y;

                }
                else
                {
                    if (target._renderer.sprite == target.targetSprite)
                    {
                        target.distanceDisplayTransform.localPosition = target.topAnchor.localPosition;
                    }
                    else
                    {
                        target.distanceDisplayTransform.localPosition = target.topAnchor.localPosition + new Vector3(-extraSpace, 0f);

                    }
                }
                if (Mathf.Abs(localPoint.x) >= validCanvasSizeX + target._renderer.GetComponent<RectTransform>().rect.x || Mathf.Abs(localPoint.y) >= validCanvasSizeY + target._renderer.GetComponent<RectTransform>().rect.y)
                {
                    target._renderer.sprite = target.arrowSprite;
                }
                else
                {
                    target._renderer.sprite = target.targetSprite;
                }

                Vector3 rotation = new Vector3();

                
                if (target._renderer.sprite == target.arrowSprite)
                {
                    
                    float angle = Vector2.Angle(Vector2.right, localPoint);


                    if(localPoint.y<0f)
                    {
                        angle = -angle;
                    }

                        //target.distanceDisplayTransform.position = target.bottomAnchor.position;


                    rotation.z = angle;
                    Debug.DrawLine(Vector2.right, localPoint, Color.green,5f,false);
                }

                target.transform.localPosition = localPoint;

                target.arrow.localRotation = Quaternion.Euler(rotation);



            }
            else
            {
                target.gameObject.SetActive(false);
                toRemove.Add(target.gameObject);

            }

        }
        if (toRemove.Count > 0)
        {
            foreach (GameObject removee in toRemove)
            {
                targets.Remove(removee);
                Destroy(removee);
            }
            toRemove.Clear();
        }



    }
    
}
